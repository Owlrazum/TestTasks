using UnityEngine;

[CreateAssetMenu(fileName = "PlayerParameters", menuName = "Game/PlayerParams")]
public class PlayerParamsSO : ScriptableObject
{
    [SerializeField]
    private float _moveSpeed = 1;

    [SerializeField]
    private float _dashDistance = 1;

    [SerializeField]
    private float _dashHitColorSwitchTime = 1;

    public float MoveSpeed
    {
        get { return _moveSpeed; }
        set { _moveSpeed = value; }
    }
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
}