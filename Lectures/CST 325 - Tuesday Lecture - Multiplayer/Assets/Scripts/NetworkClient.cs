using System.Collections.Concurrent;
using System.Net.Sockets;
using System.IO;
using System.Threading;
using UnityEngine;
using System.Collections.Generic;

public class NetworkClient : MonoBehaviour
{
    [Header("Connection")]
    public string host = "127.0.0.1";
    public int port = 9000;
    public string playerID = "player1"; // set uniquely per instance

    [Header("Scene")]
    public GameObject remotePrefab;     // the cube prefab
    public Transform localPlayer;       // your own character/cube

    [Header("Game")]
    public GameManager gameManager;

    // --- internals ---
    string _clientId;
    TcpClient _tcp;
    StreamWriter _writer;
    Thread _readThread;
    ConcurrentQueue<string> _incoming = new();
    Dictionary<string, GameObject> _remotes = new();

    void Start()
    {
        _tcp = new TcpClient(host, port);
        var stream = _tcp.GetStream();
        _writer = new StreamWriter(stream) { AutoFlush = true };
        _clientId = System.Guid.NewGuid().ToString("N"); // The "N" format specifier (for "digits only")

        _readThread = new Thread(() =>
        {
            var reader = new StreamReader(stream);
            while (true)
            {
                var line = reader.ReadLine(); // blocks until a message arrives
                if (line != null) _incoming.Enqueue(line);
            }
        });
        _readThread.IsBackground = true;
        _readThread.Start();
    }

    void Update()
    {
        // 1. Send our position
        var p = localPlayer.position;
        _writer.WriteLine($"{_clientId}|{playerID}:{p.x:F2},{p.y:F2},{p.z:F2}");

        // 2. Drain incoming messages
        while (_incoming.TryDequeue(out var msg))
            ApplyMessage(msg);
    }

    public void SendGameMessage(string msg)
    {
        _writer.WriteLine(msg);
    }

    void ApplyMessage(string msg)
    {
        // Split into sender and payload (limit 2 so "GAME:STATE:race" works)
        var parts = msg.Split(new[] { ':' }, 2);
        if (parts.Length != 2) return;

        var sender = parts[0];
        var payload = parts[1];

        var senderParts = sender.Split('|');
        if (senderParts.Length != 2) return;
        var id = senderParts[0];
        var displayName = senderParts.Length > 1 ? senderParts[1] : id;

        // Game controller messages
        if (id == "GAME")
        {
            if (gameManager != null) gameManager.OnGameMessage(payload);
            return;
        }

        if (id == _clientId) return; // ignore our own reflection

        // Remote player ready
        if (payload == "READY")
        {
            if (gameManager != null) gameManager.OnPlayerReady(displayName);
            return;
        }

        // Remote player finished (handled by GameController scoring)
        if (payload == "FINISH") return;

        // Position update: "x,y,z"
        var coords = payload.Split(',');
        if (coords.Length != 3) return;
        float x = float.Parse(coords[0]);
        float y = float.Parse(coords[1]);
        float z = float.Parse(coords[2]);

        // Spawn or move the remote player's cube
        if (!_remotes.TryGetValue(id, out var obj))
        {
            obj = Instantiate(remotePrefab);
            obj.name = displayName;
            AddNameLabel(obj, displayName);
            _remotes[id] = obj;
        }
        obj.transform.position = new Vector3(x, y, z);
    }

    void AddNameLabel(GameObject cube, string labelText)
    {
        var labelObj = new GameObject("Label");
        labelObj.transform.SetParent(cube.transform);
        labelObj.transform.localPosition = new Vector3(0, 1.2f, 0);

        var tm = labelObj.AddComponent<TextMesh>();
        tm.text = labelText;
        tm.characterSize = 0.15f;
        tm.anchor = TextAnchor.MiddleCenter;
        tm.alignment = TextAlignment.Center;
        tm.fontSize = 48;
        tm.color = Color.white;

        labelObj.AddComponent<Billboard>();
    }

    void OnDestroy()
    {
        _readThread?.Abort();
        _tcp?.Close();
    }
}