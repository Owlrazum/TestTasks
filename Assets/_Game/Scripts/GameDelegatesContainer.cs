using System;

using UnityEngine;

public static class GameDelegatesContainer
{
    public static Func<Camera> GetRenderingCamera;
    public static Action<PlayerState> EventStateChanged;
}