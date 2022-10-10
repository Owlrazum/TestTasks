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

    private GameState _state;
    public GameState GetState()
    {
        return _state;
    }

    [SerializeField]
    private GameObject _playerCharacterPrefab;

    private Dictionary<int, Player> _players; // server-only
    private int _readyPlayerCount;

    private void Awake()
    {
        DontDestroyOnLoad(gameObject);
    }

    public override void OnStartServer()
    {
        _readyPlayerCount = 0;

        ActionServerGameStart += ServerGameStart;
        NetworkController.EventServerSceneChanged += OnServerSceneChanged;
    }

    public override void OnStopServer()
    { 
        ActionServerGameStart -= ServerGameStart;
        NetworkController.EventServerSceneChanged -= OnServerSceneChanged;
    }

    public override void OnStartClient()
    { 
        NetworkController.EventClientSceneChanged += OnClientSceneChanged;
    }

    public override void OnStopClient()
    { 
        NetworkController.EventClientSceneChanged -= OnClientSceneChanged;
    }

    [Server]
    private void ServerGameStart(Dictionary<int, Player> players)
    {
        _players = players;
        NetworkController.ActionServerChangeScene(SceneType.Online);
    }

    [Server]
    private void OnServerSceneChanged(SceneType newSceneType)
    {
        UpdateState(newSceneType);
    }

    [Client]
    private void OnClientSceneChanged(SceneType newSceneType)
    {
        UpdateState(newSceneType);
        if (_state == GameState.Gameplay)
        {
            CmdOnClientLoadedGameplayScene();
        }
    }

    private void UpdateState(SceneType newSceneType)
    { 
        _state = newSceneType switch
        {
            SceneType.Offline => GameState.Offline,
            SceneType.Room => GameState.Room,
            SceneType.Online => GameState.Gameplay,
            _ => throw new System.ArgumentException("Unknown scene type")
        };
    }

    [Command(requiresAuthority = false)]
    private void CmdOnClientLoadedGameplayScene()
    {

        _readyPlayerCount++;
        if (_readyPlayerCount == _players.Count)
        {
            ServerSpawnPlayerCharacters();
            _readyPlayerCount = 0;
        }

        Assert.IsTrue(_readyPlayerCount < _players.Count);
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

