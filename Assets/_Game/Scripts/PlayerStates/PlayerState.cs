public abstract class PlayerState
{
    protected PlayerStatesController _statesController;

    public PlayerState(PlayerStatesController statesController)
    {
        _statesController = statesController;
    }

    public virtual void OnEnter()
    { 

    }

    public virtual void OnExit()
    { 

    }

    public virtual PlayerState ReactToCommand(PlayerCommand command)
    {
        return null;
    }

    public virtual PlayerState ProcessState()
    {
        return null;
    }
}