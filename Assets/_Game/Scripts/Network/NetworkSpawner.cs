using UnityEngine;
using Mirror;

[RequireComponent(typeof(NetworkController))]
public class NetworkSpawner : MonoBehaviour
{
    [SerializeField]
    private GameController _gameControllerPrefab;

    [SerializeField]
    private NetworkRoom _networkRoomPrefab;

    [SerializeField]
    private PlayerCharacter _playerCharacterPrefab;

    private NetworkController _networkController;
    private bool _isLocalPlayerCreated;
    private void Awake()
    {
        _networkController = GetComponent<NetworkController>();
        bool arePrefabsAddedToSpawnList =
            _networkController.spawnPrefabs.Contains(_gameControllerPrefab.gameObject) &&
            _networkController.spawnPrefabs.Contains(_networkRoomPrefab.gameObject) &&
            _networkController.spawnPrefabs.Contains(_playerCharacterPrefab.gameObject);
        
        if (!arePrefabsAddedToSpawnList)
        {
            Debug.LogError("Prefabs for NetworkSpawner should be added to spawn list of networkController");
        }
    }

    public void OnServerSceneChanged(SceneType sceneType)
    {
        switch (sceneType)
        { 
            case SceneType.Room:
                SpawnPrefab(_networkRoomPrefab.gameObject);
                SpawnPrefab(_gameControllerPrefab.gameObject);
                break;
            case SceneType.Online:
                SpawnPlayerCharacters();
                break;
            case SceneType.Offline:
                return;
            default:
                throw new System.ArgumentException("Unknonw type of scene for networkSpawner");
        }
    }

    private void SpawnPrefab(GameObject prefab)
    {
        GameObject toSpawn = Instantiate(prefab);
        NetworkServer.Spawn(toSpawn);
    }

    private void SpawnPlayerCharacters()
    { 
        // NetworkServer.Spawn(_playerCharacterPrefab);
    }

    private void OnDestroy()
    { 

    }
}