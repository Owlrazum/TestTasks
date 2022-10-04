using UnityEngine;

public class PlayerIdleState : PlayerState
{
    private float _deceleration;
    private float _stoppingSpeed;
    private Vector3 _stoppingDirection;

    public PlayerIdleState(PlayerParamsSO playerParams, PlayerStatesController statesController)
    : base(playerParams, statesController)
    {
        _deceleration = playerParams.Deceleration;
    }

    public override void OnEnter()
    {
        base.OnEnter();
        _statesController.IdleAnimation();
    }

    public void SetStoppingVelocity(Vector3 direction, float speed)
    {
        _stoppingDirection = direction;
        _stoppingSpeed = speed;
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
                return null;
            default:
                throw new System.ArgumentException($"Unknown type of command: {command}");
        }
    }

    private PlayerState ReactToMoveCommand(PlayerMoveCommand moveCommand)
    {
        _statesController.MoveState.ReactToCommand(moveCommand);
        return _statesController.MoveState;
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
        if (_stoppingSpeed > 0)
        {
            _stoppingSpeed -= _deceleration * Time.deltaTime;
            _statesController.Player.Move(_stoppingDirection * _stoppingSpeed * Time.deltaTime);
        }
        return null;
    }

    public override string ToString()
    {
        return "IdleState";
    }
}
