using UnityEngine;
using UnityEngine.Assertions;

public class PlayerStatesController
{
    public PlayerIdleState IdleState { get; set; }
    public PlayerMoveState MoveState { get; set; }
    public PlayerDashState DashState { get; set; }

    public CharacterController Player { get; set; }
    private PlayerState _currentState;
    private PlayerAnimator _animator;

    public PlayerStatesController(PlayerParamsSO playerParams, CharacterController controller, PlayerAnimator animator)
    {
        Player = controller;
        
        IdleState = new PlayerIdleState(playerParams, this);
        MoveState = new PlayerMoveState(playerParams, this);
        DashState = new PlayerDashState(playerParams, this);

        _animator = animator;
        _currentState = IdleState;
    }

    public void IdleAnimation()
    {
        _animator.Idle();
    }

    public void MoveAnimation()
    {
        _animator.Move();
    }

    public void DashAnimation()
    {
        _animator.Dash();
    }

    public void ReactToCommand(PlayerCommand command)
    {
        // Each state should delegate command to new state if necessary.
        PlayerState newState = _currentState.ReactToCommand(command);
        if (newState != null)
        {
            _currentState.OnExit();
            newState.OnEnter();
            _currentState = newState;

            GameDelegatesContainer.EventStateChanged?.Invoke(newState);
        }
    }

    public void Update()
    {
        PlayerState newState = _currentState.ProcessState();
        if (newState != null)
        {
            _currentState.OnExit();
            newState.OnEnter();
            _currentState = newState;
            
            GameDelegatesContainer.EventStateChanged?.Invoke(newState);
        }
    }
}