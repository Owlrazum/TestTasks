using UnityEngine;

public class CameraController
{
    private const float HorizontalMouseSpeed = 90;
    private const float VerticalMouseSpeed = 90;

    private const float VerticalOffset = 0;

    private const float MinRotAngle = 0;
    private const float MaxRotAngle = 70;

    private const float MaxInputDelta = 3;

    private const string HorizontalMouseAxisName = "Mouse X";
    private const string VerticalMouseAxisName = "Mouse Y";

    private Camera _camera;
    private Transform _followTarget;

    private float _horizontalRotationAngle;
    private float _verticalRotationAngle;
    private Vector3 _followDelta;
    public CameraController(Camera camera, Transform followTarget)
    {
        _camera = camera;
        _followTarget = followTarget;
        Vector3 followPos = followTarget.position + Vector3.up * VerticalOffset;
        _followDelta = camera.transform.position - followPos;

        Cursor.lockState = CursorLockMode.Confined;
    }

    public Vector3 AdjustInputDirection(Vector3 inputDirection)
    {
        inputDirection = _camera.transform.rotation * inputDirection;
        inputDirection.y = 0;
        return inputDirection.normalized;
    }

    public void Update()
    {
        float h = Input.GetAxis(HorizontalMouseAxisName);
        float v = Input.GetAxis(VerticalMouseAxisName);
        h = Mathf.Clamp(h, -MaxInputDelta, MaxInputDelta);
        v = Mathf.Clamp(v, -MaxInputDelta, MaxInputDelta);

        _horizontalRotationAngle += h * HorizontalMouseSpeed * Time.deltaTime;
        if (_horizontalRotationAngle < 0)
        {
            _horizontalRotationAngle = 360 + _horizontalRotationAngle;
        }
        else if (_horizontalRotationAngle > 360)
        {
            _horizontalRotationAngle = _horizontalRotationAngle - 360;
        }

        _verticalRotationAngle += v * VerticalMouseSpeed * Time.deltaTime;
        _verticalRotationAngle = Mathf.Clamp(_verticalRotationAngle, MinRotAngle, MaxRotAngle);

        Quaternion rot = Quaternion.Euler(_verticalRotationAngle, _horizontalRotationAngle, 0);
        Vector3 followOrbitDelta = rot * _followDelta;

        _camera.transform.position = _followTarget.position + followOrbitDelta;
        _camera.transform.rotation = Quaternion.LookRotation(_followTarget.position - _camera.transform.position);
    }
}