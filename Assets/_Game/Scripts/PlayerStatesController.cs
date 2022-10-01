using UnityEngine;
using UnityEngine.Assertions;

public class PlayerStatesController
{
    public PlayerIdleState IdleState { get; set; }
    public PlayerMoveState MoveState { get; set; }
    public PlayerDashState DashState { get; set; }

    private PlayerState _currentState;

    public PlayerStatesController(CharacterController controller)
    {
        IdleState = new PlayerIdleState(this);
        MoveState = new PlayerMoveState(controller, this);
        DashState = new PlayerDashState(this);

        _currentState = IdleState;
    }

    public void ReactToCommand(PlayerCommand command)
    {
        // Each state should delegate command to new state if necessary.
        PlayerState newState = _currentState.ReactToCommand(command);
        if (newState != null)
        {
            _currentState = newState;
        }
    }

    public void Update()
    {
        PlayerState newState = _currentState.ProcessState();
        if (newState != null)
        {
            _currentState = newState;
        }
    }
}