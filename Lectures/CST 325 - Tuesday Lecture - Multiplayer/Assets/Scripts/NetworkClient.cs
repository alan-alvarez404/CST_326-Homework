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

    // --- internals ---
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
        _writer.WriteLine($"{playerID}:{p.x:F2},{p.y:F2},{p.z:F2}");

        // 2. Drain incoming messages
        while (_incoming.TryDequeue(out var msg))
            ApplyMessage(msg);
    }

    void ApplyMessage(string msg)
    {
        // format: "playerID:x,z"
        var parts = msg.Split(':');
        if (parts.Length != 2) return;

        var id = parts[0];
        if (id == playerID) return; // ignore our own reflection

        var coords = parts[1].Split(',');
        if (coords.Length != 3) return;
        float x = float.Parse(coords[0]);
        float y = float.Parse(coords[1]);
        float z = float.Parse(coords[2]);

        // Spawn or move the remote player's cube
        if (!_remotes.TryGetValue(id, out var obj))
        {
            obj = Instantiate(remotePrefab);
            obj.name = id;
            AddNameLabel(obj, id);
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