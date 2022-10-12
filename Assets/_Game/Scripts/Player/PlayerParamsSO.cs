using UnityEngine;

[CreateAssetMenu(fileName = "PlayerParameters", menuName = "Game/PlayerParams")]
public class PlayerParamsSO : ScriptableObject
{
    [SerializeField]
    private float _dashDistance = 1;

    [SerializeField]
    private float _dashHitColorSwitchTime = 3;

    public float DashDistance
    {
        get { return _dashDistance; }
    }
    public float DashHitColorSwitchTime
    {
        get { return _dashHitColorSwitchTime; }
    }

    [SerializeField]
    private float _gravity = 9.81f;
    [SerializeField]
    private float _maxMoveSpeed = 1;
    [SerializeField]
    private float _rotationSpeedDeg = 360;
    [SerializeField]
    private float _acceleration = 3;
    [SerializeField]
    private float _deceleration = 5;
    [SerializeField]
    private float _dashSpeed = 5;

    public float Gravity
    {
        get { return _gravity; }
    }
    public float MaxMoveSpeed
    {
        get { return _maxMoveSpeed; }
    }
    public float RotationSpeedDeg
    {
        get { return _rotationSpeedDeg; }
    }
    public float Acceleration
    {
        get { return _acceleration; }
    }
    public float Deceleration
    {
        get { return _deceleration; }
    }
    public float DashSpeed
    {
        get { return _dashSpeed; }
    }


    [SerializeField]
    private Vector3 _initialCameraOffset = new Vector3(0, 1, -5);
    public Vector3 InitialCameraOffset 
    {
        get { return _initialCameraOffset; }
    }
    
    [SerializeField]
    private float _animationTransitionDuration = 0.3f;
    public float AnimationTranstitionDuration
    {
        get { return _animationTransitionDuration; }
    }

    [SerializeField]
    private Material _defaultMaterial;
    public Material DefaultMaterial
    {
        get { return _defaultMaterial; }
    }

    [SerializeField]
    private Material _invincibleMaterial;
    public Material InvincibleMaterial
    {
        get { return _invincibleMaterial; }
    }
}