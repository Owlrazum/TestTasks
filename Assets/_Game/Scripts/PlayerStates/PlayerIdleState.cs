public class PlayerIdleState : PlayerState
{
    public PlayerIdleState(PlayerParamsSO playerParams, PlayerStatesController statesController) 
    : base (playerParams, statesController)
    {
    }

    public override void OnEnter()
    {
        _statesController.IdleAnimation();
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

    public override PlayerState ProcessState()
    {
        CheckGravityMove();
        return null;
    }

    public override string ToString()
    {
        return "IdleState";
    }
}
