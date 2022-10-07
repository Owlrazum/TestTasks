using System;
using System.Collections.Generic;

using UnityEngine;

using Mirror;

public static class GameDelegatesContainer
{
    public static Action<SceneType> ServerEventSceneSwitch;

    public static Func<Camera> GetRenderingCamera;
    public static Action<PlayerState> EventStateChanged;

    public static Action<List<Transform>, Dictionary<int, NetworkConnection>> ServerSpawnPlayerCharacters;

    public static Action LocalPlayerScoredPoint;
    public static Action LocalGameWon;
}