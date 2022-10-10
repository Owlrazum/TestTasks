using UnityEngine;
using Mirror;

public enum SceneType
{
    Offline,
    Room,
    Online
}

[CreateAssetMenu(fileName = "NetworkScenesSO", menuName = "Game/NetworkScenes")]
public class NetworkScenesSO : ScriptableObject
{
    [Scene]
    public string OfflineScene;
    [Scene]
    public string RoomScene;
    [Scene]
    public string OnlineScene;

    public string GetSceneName(SceneType sceneType)
    {
        return sceneType switch
        {
            SceneType.Offline => OfflineScene,
            SceneType.Room => RoomScene,
            SceneType.Online => OnlineScene,
            _ => throw new System.ArgumentException($"Unknown scene type: {sceneType}!")
        };
    }

    public SceneType GetSceneType(string sceneName)
    {
        if (sceneName == OfflineScene)
        {
            return SceneType.Offline;
        }
        else if (sceneName == RoomScene)
        {
            return SceneType.Room;
        }
        else if (sceneName == OnlineScene)
        {
            return SceneType.Online;
        }

        throw new System.ArgumentException($"Unknown scene name: {sceneName}!");
    }
}