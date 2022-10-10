using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Assertions;
using Mirror;

public enum GameState
{
    Offline,
    Room,
    Gameplay
}

public class GameController : NetworkBehaviour
{
    public static Action<Dictionary<int, Player>> ActionServerGameStart;

    public static Action<PlayerCharacter> EventLocalPlayerCharacterSpawned;
    public static Action EventLocalPlayerCharacterDespawned;

    public static Func<Camera> GetRenderingCamera;
    public static Action<PlayerState> EventStateChanged;

    public static Action LocalPlayerScoredPoint;

    [SyncVar]
    private GameState _state;
    public GameState GetState()
    {
        return _state;
    }

    [SerializeField]
    private GameObject _playerCharacterPrefab;

    private Dictionary<int, Player> _players; // server-only

    private void Awake()
    {
        DontDestroyOnLoad(gameObject);
    }

    public override void OnStartServer()
    {  
        ActionServerGameStart += ServerGameStart;
        NetworkController.EventServerSceneChanged += OnServerSceneChanged;
    }

    public override void OnStopServer()
    { 
        ActionServerGameStart -= ServerGameStart;
        NetworkController.EventServerSceneChanged -= OnServerSceneChanged;
    }

    [Server]
    private void ServerGameStart(Dictionary<int, Player> players)
    {
        _players = players;
        NetworkController.ActionServerChangeScene(SceneType.Online);
    }

    [Server]
    private void OnServerSceneChanged(SceneType sceneType)
    {
        if (sceneType == SceneType.Online)
        {
            ServerSpawnPlayerCharacters();
        }
    }

    [Server]
    private void ServerSpawnPlayerCharacters()
    {
        foreach (var kv in _players)
        {
            Player player = kv.Value;
            Transform spawnPosition = NetworkController.ActionGetStartPosition();
            GameObject playerCharacterGb = Instantiate(_playerCharacterPrefab, spawnPosition.position, Quaternion.identity);
            NetworkServer.Spawn(playerCharacterGb);
            NetworkIdentity playerCharacterId = playerCharacterGb.GetComponent<NetworkIdentity>();
            player.ServerAssignCharacter(playerCharacterId);
        }
    }
}

