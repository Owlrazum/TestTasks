public class PlayerIdleState : PlayerState
{
    public PlayerIdleState(PlayerStatesController statesController) : base (statesController)
    {
    }

    public override PlayerState ReactToCommand(PlayerCommand command)
    {
        switch (command)
        { 
            case PlayerMoveCommand moveCommand:
                _statesController.MoveState.ReactToCommand(moveCommand);
                return _statesController.MoveState;
            case PlayerDashCommand dashCommand:
                _statesController.DashState.ReactToCommand(dashCommand);
                return _statesController.DashState;
            case null:
                return null;
            default:
                throw new System.ArgumentException($"Unknown type of command: {command}");
        }
    }

    public override string ToString()
    {
        return "IdleState";
    }
}
