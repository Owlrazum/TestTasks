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
    [SyncVar]
    private GameState _state;

    [SerializeField]
    private GameObject _playerCharacterPrefab;

    private void Awake()
    {
        DontDestroyOnLoad(gameObject);
    }

    public override void OnStartServer()
    {
        GameDelegatesContainer.ServerEventSceneSwitch += OnServerSceneSwitch;
        GameDelegatesContainer.ServerSpawnPlayerCharacters += ServerSpawnPlayerCharacters;
    }

    public override void OnStopServer()
    { 
        GameDelegatesContainer.ServerEventSceneSwitch -= OnServerSceneSwitch;
        GameDelegatesContainer.ServerSpawnPlayerCharacters -= ServerSpawnPlayerCharacters;
    }

    public GameState GetState()
    {
        return _state;
    }

    [Server]
    private void OnServerSceneSwitch(SceneType sceneType)
    {
        _state = sceneType switch
        {
            SceneType.Offline => GameState.Offline,
            SceneType.Room => GameState.Room,
            SceneType.Online => GameState.Gameplay,
            _ => throw new System.ArgumentException("Unknown type of sceneType")
        };
    }

    [Server]
    private void ServerSpawnPlayerCharacters(List<Transform> spawnPositions, Dictionary<int, NetworkConnection> owners)
    {
        spawnPositions.Shuffle();
        int spawnIndexer = 0;
        foreach (var keyValue in owners)
        {
            NetworkConnection owner = keyValue.Value;
            GameObject playerCharacter = Instantiate(_playerCharacterPrefab, spawnPositions[spawnIndexer++].position, Quaternion.identity);
            NetworkServer.Spawn(playerCharacter);
        }
    }
}

