using UnityEngine;

[CreateAssetMenu(fileName = "PlayerParameters", menuName = "Game/PlayerParams")]
public class PlayerParamsSO : ScriptableObject
{
    [SerializeField]
    private float _dashDistance = 1;

    [SerializeField]
    private float _dashHitColorSwitchTime = 1;

    public float DashDistance
    {
        get { return _dashDistance; }
        set { _dashDistance = value; }
    }
    public float DashHitColorSwitchTime
    {
        get { return _dashHitColorSwitchTime; }
        set { _dashHitColorSwitchTime = value; }
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
        set { _gravity = value; }
    }
    public float MaxMoveSpeed
    {
        get { return _maxMoveSpeed; }
        set { _maxMoveSpeed = value; }
    }
    public float RotationSpeedDeg
    {
        get { return _rotationSpeedDeg; }
        set { _rotationSpeedDeg = value; }
    }
    public float Acceleration
    {
        get { return _acceleration; }
        set { _acceleration = value; }
    }
    public float Deceleration
    {
        get { return _deceleration; }
        set { _deceleration = value; }
    }
    public float DashSpeed
    {
        get { return _dashSpeed; }
        set { _dashSpeed = value; }
    }

    [SerializeField]
    private float _animationTransitionDuration = 0.3f;
    public float AnimationTranstitionDuration
    {
        get { return _animationTransitionDuration; }
        set { _animationTransitionDuration = value; }
    }
}