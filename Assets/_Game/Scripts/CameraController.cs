using UnityEngine;

public class CameraController
{
    private Camera _camera;
    public CameraController(Camera camera)
    {
        _camera = camera;
    }

    public Vector3 AdjustInputDirection(Vector3 inputDirection)
    {
        inputDirection = _camera.transform.rotation * inputDirection;
        inputDirection.y = 0;
        return inputDirection.normalized;
    }
}