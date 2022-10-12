using System;
using UnityEngine;
using UnityEngine.Assertions;
using Mirror;

[RequireComponent(typeof(NetworkSpawner))]
public class NetworkController : NetworkManager
{
    [Header("NetworkController")]
    [SerializeField]
    private NetworkScenesSO _scenesToUse;

    public static Action ActionStartServer;
    public static Action ActionStartHost;
    public static Action ActionStartClient;

    public static Action<string> ActionUpdateNetworkAddress;

    public static Action<SceneType> ActionServerChangeScene;
    public static Func<Transform> ActionGetStartPosition;

    public static Action<SceneType> EventServerSceneChanged; // pub
    public static Action<SceneType> EventClientSceneChanged; // pub

    private NetworkSpawner _spawner;

    public SceneType CurrentScene; // no syncVar because it is not networkBehaviour

    public SceneType ClientLoadingScene;

    public override void Awake()
    {
        base.Awake();

        ActionStartServer += StartServerAndSwitchScene;
        ActionStartHost += StartHostAndSwitchScene;
        ActionStartClient += StartClient;

        ActionUpdateNetworkAddress += UpdateNetworkAddress;

        ActionServerChangeScene += ServerChangeSceneByType;
        ActionGetStartPosition += GetStartPosition;

        // Could use singleton instead, but prefer to use static delegates because they make it easier to
        // expose specific functionality for me. 
        // Below is a way to ensure that only one NetworkController instance is existing.
        Assert.IsTrue(ActionStartServer.GetInvocationList().Length == 1, "There are more than one NetworkControllers!");

        _spawner = GetComponent<NetworkSpawner>();
    }

    public override void OnDestroy()
    {
        base.OnDestroy();

        ActionStartServer -= StartServerAndSwitchScene;
        ActionStartHost -= StartHostAndSwitchScene;
        ActionStartClient -= StartClient;

        ActionUpdateNetworkAddress -= UpdateNetworkAddress;

        ActionServerChangeScene -= ServerChangeSceneByType;
        ActionGetStartPosition -= GetStartPosition;

        Assert.IsTrue(ActionStartServer == null, "There are more than one NetworkControllers!");
    }

    private void UpdateNetworkAddress(string address)
    {
        networkAddress = address;
    }

    private void StartServerAndSwitchScene()
    {
        if (NetworkServer.active)
        {
            Debug.LogWarning("Server already started.");
            return;
        }

        mode = NetworkManagerMode.ServerOnly;
        SetupServer();
        // omit the spawn part and requires scene change part.

        ServerChangeSceneByType(SceneType.Room);
    }

    private void StartHostAndSwitchScene()
    {
         if (NetworkServer.active)
        {
            Debug.LogWarning("Server already started.");
            return;
        }

        mode = NetworkManagerMode.Host;
        SetupServer();
        finishStartHostPending = true;

        ServerChangeSceneByType(SceneType.Room);
    }

    private void ServerChangeSceneByType(SceneType sceneType)
    {
        ServerChangeScene(_scenesToUse.GetSceneName(sceneType));
    }

    public override void OnServerSceneChanged(string sceneName)
    {
        SceneType newSceneType = _scenesToUse.GetSceneType(sceneName);
        _spawner.OnServerSceneChanged(newSceneType);
        CurrentScene = newSceneType;
        EventServerSceneChanged?.Invoke(newSceneType);
    }

    public override void OnClientChangeScene(string newSceneName, SceneOperation sceneOperation, bool customHandling)
    {
        base.OnClientChangeScene(newSceneName, sceneOperation, customHandling);

        ClientLoadingScene = _scenesToUse.GetSceneType(newSceneName);
    }

    public override void OnClientSceneChanged()
    {
        if (!NetworkClient.ready)
        { 
            NetworkClient.Ready();
        }
        
        CurrentScene = ClientLoadingScene;
        EventClientSceneChanged?.Invoke(CurrentScene);
    }

    public override void OnServerConnect(NetworkConnectionToClient conn)
    {
        if (numPlayers >= maxConnections)
        {
            conn.Disconnect();
            return;
        }

        if (GameController.FuncServerGetState != null) // The GameController is spawned
        { 
            if (GameController.FuncServerGetState() == GameState.Gameplay)
            {
                conn.Disconnect();
                Debug.Log("You are not allowed to connect to existing game.");
                return;
            }
        }
    }

    public override void OnClientConnect()
    {
        NetworkClient.Ready();
        NetworkClient.AddPlayer();
    }
}