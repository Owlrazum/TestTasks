using System;
using UnityEngine;
using UnityEngine.Assertions;
using Mirror;

public class NetworkController : NetworkManager
{
    [Header("NetworkController")]
    [SerializeField]
    private NetworkScenesSO _scenesToUse;

    [SerializeField]
    private GameController _gameController;

    public static Action ActionStartServer;
    public static Action ActionStartHost;
    public static Action ActionStartClient;

    public static Action<string> ActionUpdateNetworkAddress;

    public static Action<SceneType> ActionServerChangeScene;
    public static Func<Transform> ActionGetStartPosition; // sub
    public static Action<SceneType> EventServerSceneChanged; // pub

    public override void Awake()
    {
        base.Awake();

        ActionStartServer += StartServerAndSwitchScene;
        ActionStartHost += StartHostAndSwitchScene;
        ActionStartClient += StartClientAndDestroyRedundantGameController;

        ActionUpdateNetworkAddress += UpdateNetworkAddress;

        ActionServerChangeScene += ServerChangeSceneByType;
        ActionGetStartPosition += GetStartPosition;

        // Could use singleton instead, but prefer to use static delegates because they make it easier to
        // expose specific functionality for me. 
        // Below is a way to ensure that only one NetworkController instance is existing.
        Assert.IsTrue(ActionStartServer.GetInvocationList().Length == 1, "There are more than one NetworkControllers!");
    }

    public override void OnDestroy()
    {
        ActionStartServer -= StartServerAndSwitchScene;
        ActionStartHost -= StartHostAndSwitchScene;
        ActionStartClient -= StartClientAndDestroyRedundantGameController;

        ActionUpdateNetworkAddress -= UpdateNetworkAddress;

        ActionServerChangeScene -= ServerChangeSceneByType;
        ActionGetStartPosition -= GetStartPosition;

        Assert.IsTrue(ActionStartServer == null,  "There are more than one NetworkControllers!");
    }

    private void UpdateNetworkAddress(string address)
    {
        networkAddress = address;
    }

    private void StartServerAndSwitchScene()
    {
        StartServer();
        ServerChangeSceneByType(SceneType.Room);
    }

    private void StartHostAndSwitchScene()
    { 
        StartHost();
        ServerChangeSceneByType(SceneType.Room);
    }

    private void StartClientAndDestroyRedundantGameController()
    {
        Destroy(_gameController);
        StartClient();
    }

    private void ServerChangeSceneByType(SceneType sceneType)
    {
        ServerChangeScene(_scenesToUse.GetName(sceneType));
    }

    public override void OnServerSceneChanged(string sceneName)
    {
        EventServerSceneChanged?.Invoke(_scenesToUse.GetType(sceneName));
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
    }
}