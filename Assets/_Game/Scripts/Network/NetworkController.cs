using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Assertions;
using Mirror;

public enum SceneType
{
    Offline,
    Room,
    Online
}

public class NetworkController : NetworkManager
{
    [Header("NetworkController")]
    [SerializeField]
    private NetworkScenesSO _scenesToUse;

    [SerializeField]
    private GameController _gameController;

    private Dictionary<int, NetworkConnection> _playerConnections;

    public override void Awake()
    {
        base.Awake();

        // because this field are redundant
        offlineScene = _scenesToUse.OfflineScene;
        onlineScene = _scenesToUse.RoomScene;

        _playerConnections = new Dictionary<int, NetworkConnection>(4);

        NetworkDelegatesContainer.StartServer += StartServer;
        NetworkDelegatesContainer.StartHost += StartHost;
        NetworkDelegatesContainer.StartClient += StartClient;

        NetworkDelegatesContainer.UpdateNetworkAddress += UpdateNetworkAddress;

        Assert.IsTrue(NetworkDelegatesContainer.StartServer.GetInvocationList().Length == 1);
    }

    public override void OnDestroy()
    {
        NetworkDelegatesContainer.StartServer -= StartServer;
        NetworkDelegatesContainer.StartHost -= StartHost;
        NetworkDelegatesContainer.StartClient -= StartClient;


        NetworkDelegatesContainer.UpdateNetworkAddress -= UpdateNetworkAddress;

        Assert.IsTrue(NetworkDelegatesContainer.StartServer == null);
    }

    private void UpdateNetworkAddress(string address)
    {
        networkAddress = address;
    }

    public override void OnStartServer()
    {
        base.OnStartServer();
        GameDelegatesContainer.ServerEventSceneSwitch?.Invoke(SceneType.Room);
    }

    public override void OnServerSceneChanged(string sceneName)
    {
        if (sceneName == _scenesToUse.OnlineScene)
        {
            GameDelegatesContainer.ServerSpawnPlayerCharacters(startPositions, _playerConnections);
        }
    }

    public override void OnServerConnect(NetworkConnectionToClient conn)
    {
        if (numPlayers >= maxConnections)
        {
            conn.Disconnect();
            return;
        }

        
        if (_gameController.GetState() == GameState.Gameplay)
        {
            conn.Disconnect();
            return;
        }

        _playerConnections.Add(conn.connectionId, conn);
    }

    public override void OnServerDisconnect(NetworkConnectionToClient conn)
    {
        base.OnServerDisconnect(conn);

        _playerConnections.Remove(conn.connectionId);
    }
}