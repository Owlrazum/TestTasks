using System;
using System.Collections;
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
    public static Func<GameState> FuncServerGetState;

    public static Action<Dictionary<int, Player>> ActionServerGameStart; // pub is NetworkRoom.
    public static Action<Dictionary<int, Player>> EventServerGameStarted; // pub is GameController

    public static Action<Player> ServerPlayerIncreasedScore;
    public static Action<Player> EventServerGameEnded;
    public static Action EventServerMatchRestarted;

    private const int WinScoresAmount = 3;

    private GameState _state;
    public GameState GetState()
    {
        return _state;
    }

    [SerializeField]
    private GameObject _playerCharacterPrefab;

    [SerializeField]
    private float _gameEndPauseTime = 5;

    private Dictionary<int, Player> _players; // server-only, with playerIndex as key
    private Dictionary<int, int> _scores; // server-only, with playerIndex as key
    private int _readyPlayerCount;

    private void Awake()
    {
        DontDestroyOnLoad(gameObject);
    }

    public override void OnStartServer()
    {
        _readyPlayerCount = 0;

        FuncServerGetState += GetState;

        ActionServerGameStart += ServerGameStart;
        ServerPlayerIncreasedScore += OnPlayerIncreasedScore;
        NetworkController.EventServerSceneChanged += OnServerSceneChanged;
    }

    public override void OnStopServer()
    {
        FuncServerGetState -= GetState;

        ActionServerGameStart -= ServerGameStart;
        ServerPlayerIncreasedScore -= OnPlayerIncreasedScore;
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
        _scores = new Dictionary<int, int>();
        foreach (var kv in _players)
        {
            _scores.Add(kv.Key, 0);
        }

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
            EventServerGameStarted?.Invoke(_players);
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

    [Server]
    private void OnPlayerIncreasedScore(Player player)
    {
        _scores[player.Index]++;
        if (_scores[player.Index] >= WinScoresAmount)
        {
            EndGame(winner: player);
        }
    }

    private void EndGame(Player winner)
    {
        Debug.Log("ServerGameEnded!");
        EventServerGameEnded?.Invoke(winner);
        StartCoroutine(GameEndPause());
    }

    private IEnumerator GameEndPause()
    {
        yield return new WaitForSeconds(_gameEndPauseTime);
        ServerRestartMatch();
        EventServerMatchRestarted?.Invoke();
    }

    [Server]
    private void ServerRestartMatch()
    {
        foreach (var kv in _players)
        {
            Player player = kv.Value;
            Transform spawnPosition = NetworkController.ActionGetStartPosition();
            player.ServerRespawnCharacter(spawnPosition.position);
        }

        int[] keys = new int[_scores.Keys.Count];
        _scores.Keys.CopyTo(keys, 0); // As a quick implementation of each value update in dictionary. Could use references for playerScore instead of int, but there is existing already in the PlayerScoresUI, which would then be needed to be moved to separate class.

        for (int i = 0; i < keys.Length; i++)
        {
            _scores[keys[i]] = 0;
        }
    }
}

