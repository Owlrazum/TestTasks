using UnityEngine;

[RequireComponent(typeof(Camera))]
public class CameraHolder : MonoBehaviour
{
    private Camera _camera;
    private void Awake()
    {
        TryGetComponent(out _camera);
        GameDelegatesContainer.GetRenderingCamera += GetCamera;
    }

    private void OnDestroy()
    { 
        GameDelegatesContainer.GetRenderingCamera -= GetCamera;
    }

    private Camera GetCamera()
    {
        return _camera;
    }
}