using UnityEngine;

public class PlayerDashState : PlayerState
{
    private readonly float _dashDistance;
    private readonly float _dashSpeed;

    private bool _isDashing;
    private Vector3 _direction;
    private float _remainingDistance;

    public PlayerDashState(PlayerParamsSO playerParams, PlayerStatesController statesControlller) 
    : base(playerParams, statesControlller)
    {
        _dashDistance = playerParams.DashDistance;
        _dashSpeed = playerParams.DashSpeed;
    }

    public override void OnEnter()
    {
        _statesController.DashAnimation();
        _remainingDistance = _dashDistance;
    }

    public override PlayerState ReactToCommand(PlayerCommand command)
    {
        if (_isDashing)
        {
            if (command is PlayerMoveCommand moveCommand &&
            _remainingDistance <= 0)
            {
                _isDashing = false;
                moveCommand.ShouldStartAtMaxSpeed = true;
                return _statesController.MoveState;
            }
            else
            { 
                return null;
            }
        }

        if (command is PlayerDashCommand dashCommand)
        {
            _isDashing = true;
            _direction = dashCommand.Direction;
        }

        return null;
    }

    public override PlayerState ProcessState()
    {
        if (_remainingDistance <= 0)
        {
            _isDashing = false;
            return _statesController.IdleState;
        }

        Vector3 movement = _direction * _dashSpeed * Time.deltaTime;
        _remainingDistance -= movement.magnitude;
        _statesController.Player.Move(movement);
        _statesController.Player.transform.rotation = Quaternion.LookRotation(_direction, Vector3.up);
        return null;
    }

    public override string ToString()
    {
        return "DashState";
    }
}