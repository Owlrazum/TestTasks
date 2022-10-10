using System;
using UnityEngine;

[RequireComponent(typeof(Camera))]
public class CameraHolder : MonoBehaviour
{
    public static Func<Camera> FuncGetCamera;
    private Camera _camera;
    private void Awake()
    {
        TryGetComponent(out _camera);
        FuncGetCamera = GetCamera;
    }

    private void OnDestroy()
    {
        FuncGetCamera = null;
    }

    private Camera GetCamera()
    {
        return _camera;
    }
}