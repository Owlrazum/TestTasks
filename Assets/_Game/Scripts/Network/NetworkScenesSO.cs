using UnityEngine;
using Mirror;

[CreateAssetMenu(fileName = "NetworkScenesSO", menuName = "Game/NetworkScenes")]
public class NetworkScenesSO : ScriptableObject
{
    [Scene]
    public string OfflineScene;
    [Scene]
    public string RoomScene;
    [Scene]
    public string OnlineScene;
}