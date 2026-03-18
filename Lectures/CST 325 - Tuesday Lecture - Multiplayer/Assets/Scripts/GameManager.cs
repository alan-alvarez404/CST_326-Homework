using UnityEngine;
using System.Collections.Generic;

public class GameManager : MonoBehaviour
{
    public enum GameState { Lobby, Racing, Finished, Over }

    [Header("References")]
    public NetworkClient networkClient;
    public PlayerController playerController;
    public Transform localPlayer;

    [Header("Race")]
    public Vector3 startPosition = new Vector3(0, 0.5f, 0);

    // -- State --
    public GameState state { get; private set; } = GameState.Lobby;
    bool _localReady = false;
    bool _localFinished = false;
    float _raceStartTime;
    float _localFinishTime;
    HashSet<string> _readyPlayers = new();
    List<ScoreEntry> _scores = new();

    // -- Obstacles --
    List<ObstacleRuntime> _obstacles = new();

    struct ScoreEntry
    {
        public string name;
        public float time;
    }

    struct ObstacleRuntime
    {
        public Transform pivot;
        public int dir; // 1 or -1
    }

    // =========================================================
    // Called by NetworkClient when a GAME: message arrives
    // =========================================================
    public void OnGameMessage(string payload)
    {
        // payload examples: "STATE:lobby", "STATE:race", "SCORES:a=1.23,b=4.56"
        if (payload.StartsWith("STATE:"))
        {
            var s = payload.Substring(6);
            switch (s)
            {
                case "lobby":
                    EnterLobby();
                    break;
                case "race":
                    EnterRace();
                    break;
                case "reset":
                    OnReset();
                    break;
                case "over":
                    state = GameState.Over;
                    break;
            }
        }
        else if (payload.StartsWith("COURSE:"))
        {
            BuildCourse(payload.Substring(7));
        }
        else if (payload.StartsWith("ROT:"))
        {
            float angle = float.Parse(payload.Substring(4));
            ApplyRotation(angle);
        }
        else if (payload.StartsWith("SCORES:"))
        {
            ParseScores(payload.Substring(7));
        }
    }

    // Called by NetworkClient when a remote player sends READY
    public void OnPlayerReady(string id)
    {
        _readyPlayers.Add(id);
    }

    // Called by FinishLine when local player crosses it
    public void OnLocalPlayerFinished()
    {
        if (state != GameState.Racing || _localFinished) return;
        _localFinished = true;
        _localFinishTime = Time.time - _raceStartTime;
        state = GameState.Finished;
        networkClient.SendGameMessage($"{networkClient.playerID}:FINISH");
    }

    // =========================================================
    // State transitions
    // =========================================================
    void EnterLobby()
    {
        state = GameState.Lobby;
        _localReady = false;
        _localFinished = false;
        _readyPlayers.Clear();
        _scores.Clear();
        if (playerController != null) playerController.enabled = true;
    }

    void EnterRace()
    {
        state = GameState.Racing;
        _localFinished = false;
        _raceStartTime = Time.time;
        _scores.Clear();
        if (playerController != null) playerController.enabled = true;
        // Teleport to start
        localPlayer.position = startPosition;
    }

    // =========================================================
    // Course obstacles
    // =========================================================
    void BuildCourse(string data)
    {
        ClearCourse();

        // data = "x,sx,sz,pivot,dir|x,sx,sz,pivot,dir|..."
        var entries = data.Split('|');
        foreach (var entry in entries)
        {
            var f = entry.Split(',');
            if (f.Length != 5) continue;

            float z    = float.Parse(f[0]);
            float sx   = float.Parse(f[1]);
            float sz   = float.Parse(f[2]);
            string piv = f[3];              // C, L, or R
            int dir    = int.Parse(f[4]);

            SpawnObstacle(z, sx, sz, piv, dir);
        }
    }

    void SpawnObstacle(float z, float sx, float sz, string pivot, int dir)
    {
        float pivotZ = 0f;
        float cubeOffsetZ = 0f;
        const float half = 0.5f; // half of 1-unit wide course

        switch (pivot)
        {
            case "L": pivotZ = -half; cubeOffsetZ =  half; break;
            case "R": pivotZ =  half; cubeOffsetZ = -half; break;
            default:  pivotZ = 0f;    cubeOffsetZ = 0f;    break;
        }

        // Parent empty sits at the pivot point
        var pivotObj = new GameObject($"Obstacle_{z}");
        pivotObj.transform.position = new Vector3(pivotZ, 0.5f, z);

        // Child cube is offset from pivot so it sweeps across the course
        var cube = GameObject.CreatePrimitive(PrimitiveType.Cube);
        cube.transform.SetParent(pivotObj.transform);
        cube.transform.localPosition = new Vector3(0f, 0f, cubeOffsetZ);
        cube.transform.localScale = new Vector3(sx, 1f, sz);

        var rend = cube.GetComponent<Renderer>();
        if (rend != null) rend.material.color = Color.red;

        _obstacles.Add(new ObstacleRuntime { pivot = pivotObj.transform, dir = dir });
    }

    void ApplyRotation(float angle)
    {
        foreach (var obs in _obstacles)
            obs.pivot.rotation = Quaternion.Euler(0f, angle * obs.dir, 0f);
    }

    void ClearCourse()
    {
        foreach (var obs in _obstacles)
        {
            if (obs.pivot != null) Destroy(obs.pivot.gameObject);
        }
        _obstacles.Clear();
    }

    void OnReset()
    {
        state = GameState.Lobby;
        _localReady = false;
        _localFinished = false;
        _readyPlayers.Clear();
        _scores.Clear();
        ClearCourse();
        localPlayer.position = startPosition;
    }

    void ParseScores(string data)
    {
        // format: "alice=12.34,bob=15.67"
        _scores.Clear();
        var entries = data.Split(',');
        foreach (var entry in entries)
        {
            var kv = entry.Split('=');
            if (kv.Length == 2 && float.TryParse(kv[1], out float t))
                _scores.Add(new ScoreEntry { name = kv[0], time = t });
        }
    }

    // =========================================================
    // Input
    // =========================================================
    void Update()
    {
        if (state == GameState.Lobby && !_localReady && Input.GetKeyDown(KeyCode.R))
        {
            _localReady = true;
            _readyPlayers.Add(networkClient.playerID);
            networkClient.SendGameMessage($"{networkClient.playerID}:READY");
        }
    }

    // =========================================================
    // UI (OnGUI — no Canvas setup needed)
    // =========================================================
    void OnGUI()
    {
        switch (state)
        {
            case GameState.Lobby:
                DrawLobbyUI();
                break;
            case GameState.Racing:
                DrawRacingUI();
                break;
            case GameState.Finished:
                DrawFinishedUI();
                break;
            case GameState.Over:
                DrawOverUI();
                break;
        }
    }

    void DrawLobbyUI()
    {
        GUIStyle big = new GUIStyle(GUI.skin.label) { fontSize = 24, alignment = TextAnchor.UpperCenter };
        GUIStyle med = new GUIStyle(GUI.skin.label) { fontSize = 18, alignment = TextAnchor.UpperCenter };

        float cx = Screen.width / 2f;

        GUI.Label(new Rect(cx - 200, 20, 400, 40), "LOBBY", big);

        if (_localReady)
            GUI.Label(new Rect(cx - 200, 60, 400, 30), "You are READY! Waiting for start...", med);
        else
            GUI.Label(new Rect(cx - 200, 60, 400, 30), "Press R to Ready Up", med);

        // Ready list
        float y = 100;
        foreach (var p in _readyPlayers)
        {
            GUI.Label(new Rect(cx - 100, y, 200, 25), $"{p}  [READY]", med);
            y += 25;
        }
    }

    void DrawRacingUI()
    {
        GUIStyle timer = new GUIStyle(GUI.skin.label) { fontSize = 28, alignment = TextAnchor.UpperRight };
        float elapsed = Time.time - _raceStartTime;
        GUI.Label(new Rect(Screen.width - 220, 10, 200, 40), $"{elapsed:F1}s", timer);
    }

    void DrawFinishedUI()
    {
        GUIStyle big = new GUIStyle(GUI.skin.label) { fontSize = 24, alignment = TextAnchor.UpperCenter };
        GUIStyle med = new GUIStyle(GUI.skin.label) { fontSize = 18, alignment = TextAnchor.UpperCenter };

        float cx = Screen.width / 2f;
        GUI.Label(new Rect(cx - 200, 20, 400, 40), $"FINISHED!  {_localFinishTime:F2}s", big);
        GUI.Label(new Rect(cx - 200, 60, 400, 30), "Waiting for everyone...", med);
        DrawScoreboard(100);
    }

    void DrawOverUI()
    {
        GUIStyle big = new GUIStyle(GUI.skin.label) { fontSize = 28, alignment = TextAnchor.UpperCenter };
        float cx = Screen.width / 2f;
        GUI.Label(new Rect(cx - 200, 20, 400, 40), "RACE OVER!", big);
        DrawScoreboard(70);
    }

    void DrawScoreboard(float startY)
    {
        GUIStyle heading = new GUIStyle(GUI.skin.label) { fontSize = 20, alignment = TextAnchor.UpperCenter, fontStyle = FontStyle.Bold };
        GUIStyle row = new GUIStyle(GUI.skin.label) { fontSize = 18, alignment = TextAnchor.UpperCenter };

        float cx = Screen.width / 2f;
        GUI.Label(new Rect(cx - 150, startY, 300, 30), "Scoreboard", heading);

        float y = startY + 35;
        for (int i = 0; i < _scores.Count; i++)
        {
            GUI.Label(new Rect(cx - 150, y, 300, 25), $"{i + 1}. {_scores[i].name}  -  {_scores[i].time:F2}s", row);
            y += 28;
        }
    }
}
