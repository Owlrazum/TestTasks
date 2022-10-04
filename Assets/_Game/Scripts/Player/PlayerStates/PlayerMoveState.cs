using UnityEngine;
public class PlayerMoveState : PlayerState
{
    private readonly float _acceleration;
    private readonly float _maxSpeed;
    private readonly float _rotationSpeedDeg;

    private Vector3 _direction;
    private float _currentSpeed;
    private bool _shouldStartAtMaxSpeed;

    public PlayerMoveState(PlayerParamsSO playerParams, PlayerStatesController statesController)
     : base (playerParams, statesController)
    {
        _acceleration = playerParams.Acceleration;
        _maxSpeed = playerParams.MaxMoveSpeed;
        _rotationSpeedDeg = playerParams.RotationSpeedDeg;
    }

    public override void OnEnter()
    {
        base.OnEnter();
        _statesController.MoveAnimation();
        if (_shouldStartAtMaxSpeed)
        {
            _currentSpeed = _maxSpeed;
        }
        else
        { 
            _currentSpeed = 0;
        }
    }

    public override PlayerState ReactToCommand(PlayerCommand command)
    {
        switch (command)
        { 
            case PlayerMoveCommand moveCommand:
                return ReactToMoveCommand(moveCommand);
            case PlayerDashCommand dashCommand:
                return ReactToDashCommand(dashCommand);
            case null:
                if (_currentSpeed > 0)
                {
                    _statesController.IdleState.SetStoppingVelocity(_direction, _currentSpeed);
                }
                return _statesController.IdleState;
            default:
                throw new System.ArgumentException($"Unknown type of command: {command}");
        }
    }

    private PlayerState ReactToMoveCommand(PlayerMoveCommand moveCommand)
    {
        _direction = moveCommand.Direction;

        _shouldStartAtMaxSpeed = moveCommand.ShouldStartAtMaxSpeed;
        return null;
    }

    private PlayerState ReactToDashCommand(PlayerDashCommand dashCommand)
    {
        dashCommand.Direction = _statesController.Player.transform.forward;
        _statesController.DashState.ReactToCommand(dashCommand);
        return _statesController.DashState;
    }

    public override PlayerState ProcessState()
    {
        CheckGravityMove();

        _currentSpeed += _acceleration * Time.deltaTime;
        _currentSpeed = Mathf.Clamp(_currentSpeed, 0, _maxSpeed);

        _statesController.Player.Move(_direction * _currentSpeed * Time.deltaTime);
        Quaternion playerRotation = _statesController.Player.transform.rotation;
        Quaternion targetRotation = Quaternion.LookRotation(_direction, Vector3.up);
        _statesController.Player.transform.rotation = 
            Quaternion.RotateTowards(playerRotation, targetRotation, _rotationSpeedDeg * Time.deltaTime);
        return null;
    }

    public override string ToString()
    {
        return "MoveState";
    }
}