using UnityEngine;
public class PlayerMoveState : PlayerState
{
    private Vector3 _direction;
    private float _speed;
    private CharacterController _controller;
    public PlayerMoveState(CharacterController controller, PlayerStatesController statesController) : base (statesController)
    {
        _controller = controller;
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
                return _statesController.IdleState;
            default:
                throw new System.ArgumentException($"Unknown type of command: {command}");
        }
    }

    private PlayerState ReactToMoveCommand(PlayerMoveCommand moveCommand)
    {
        _direction = moveCommand.Direction;
        _speed = moveCommand.Speed;
        return null;
    }

    private PlayerState ReactToDashCommand(PlayerDashCommand dashCommand)
    {
        _statesController.DashState.ReactToCommand(dashCommand);
        return _statesController.DashState;
    }

    public override PlayerState ProcessState()
    {
        Vector3 tranlation = _direction * _speed * Time.deltaTime;
        _controller.Move(tranlation);
        return null;
    }

    public override string ToString()
    {
        return "MoveState";
    }
}