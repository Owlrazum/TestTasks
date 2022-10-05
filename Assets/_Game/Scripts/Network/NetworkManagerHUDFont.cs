// vis2k: GUILayout instead of spacey += ...; removed Update hotkeys to avoid
// confusion if someone accidentally presses one.
using UnityEngine;
using Mirror;

[RequireComponent(typeof(NetworkManager))]
public class NetworkManagerHUDFont : MonoBehaviour
{
    NetworkManager manager;

    public int FontSize = 50;
    public int OffsetX;
    public int OffsetY;

    private void Awake()
    {
        manager = GetComponent<NetworkManager>();
    }

    private GUIStyle _buttonStyle;
    private GUIStyle _textFieldStyle;
    private GUIStyle _labelStyle;

    private bool inititalized;

    private void OnGUI()
    {
        if (!inititalized)
        {
            _buttonStyle = new GUIStyle("button");
            _buttonStyle.fontSize = FontSize;

            _textFieldStyle = new GUIStyle("textField");
            _textFieldStyle.fontSize = FontSize;

            _labelStyle = new GUIStyle("label");
            _labelStyle.fontSize = FontSize;

            inititalized = true;
        }
        // GUILayout.BeginArea(new Rect(10 + offsetX, 40 + offsetY, 215, 9999));
        GUILayout.BeginArea(new Rect(10 + OffsetX, 40 + OffsetY, 615, 1399));
        if (!NetworkClient.isConnected && !NetworkServer.active)
        {
            StartButtons();
        }
        else
        {
            StatusLabels();
        }

        // client ready
        if (NetworkClient.isConnected && !NetworkClient.ready)
        {
            if (GUILayout.Button("Client Ready", _buttonStyle))
            {
                NetworkClient.Ready();
                if (NetworkClient.localPlayer == null)
                {
                    NetworkClient.AddPlayer();
                }
            }
        }

        StopButtons();

        GUILayout.EndArea();
    }

    private void StartButtons()
    {
        if (!NetworkClient.active)
        {
            // Server + Client
            if (Application.platform != RuntimePlatform.WebGLPlayer)
            {
                if (GUILayout.Button("Host (Server + Client)", _buttonStyle))
                {
                    manager.StartHost();
                }
            }

            // Client + IP
            GUILayout.BeginHorizontal();
            if (GUILayout.Button("Client", _buttonStyle))
            {
                manager.StartClient();
            }
            // This updates networkAddress every frame from the TextField
            manager.networkAddress = GUILayout.TextField(manager.networkAddress, _textFieldStyle);
            GUILayout.EndHorizontal();

            // Server Only
            if (Application.platform == RuntimePlatform.WebGLPlayer)
            {
                // cant be a server in webgl build
                GUILayout.Box("(  WebGL cannot be server  )");
            }
            else
            {
                if (GUILayout.Button("Server Only", _buttonStyle)) manager.StartServer();
            }
        }
        else
        {
            // Connecting
            GUILayout.Label($"Connecting to {manager.networkAddress}..");
            if (GUILayout.Button("Cancel Connection Attempt", _buttonStyle))
            {
                manager.StopClient();
            }
        }
    }

    private void StatusLabels()
    {
        // host mode
        // display separately because this always confused people:
        //   Server: ...
        //   Client: ...
        if (NetworkServer.active && NetworkClient.active)
        {
            GUILayout.Label($"<b>Host</b>: running via {Transport.activeTransport}", _labelStyle);
        }
        // server only
        else if (NetworkServer.active)
        {
            GUILayout.Label($"<b>Server</b>: running via {Transport.activeTransport}", _labelStyle);
        }
        // client only
        else if (NetworkClient.isConnected)
        {
            GUILayout.Label($"<b>Client</b>: connected to {manager.networkAddress} via {Transport.activeTransport}", _labelStyle);
        }
    }

    private void StopButtons()
    {
        // stop host if host mode
        if (NetworkServer.active && NetworkClient.isConnected)
        {
            if (GUILayout.Button("Stop Host", _buttonStyle))
            {
                manager.StopHost();
            }
        }
        // stop client if client-only
        else if (NetworkClient.isConnected)
        {
            if (GUILayout.Button("Stop Client", _buttonStyle))
            {
                manager.StopClient();
            }
        }
        // stop server if server-only
        else if (NetworkServer.active)
        {
            if (GUILayout.Button("Stop Server", _buttonStyle))
            {
                manager.StopServer();
            }
        }
    }
}
