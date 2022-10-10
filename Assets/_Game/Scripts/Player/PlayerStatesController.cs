using System;
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

    public Action<PlayerState> EventStateChanged;

    public PlayerStatesController(PlayerParamsSO playerParams, CharacterController controller, PlayerAnimator animator)
    {
        Player = controller;

        IdleState = new PlayerIdleState(playerParams, this);
        MoveState = new PlayerMoveState(playerParams, this);
        DashState = new PlayerDashState(playerParams, this);

        _animator = animator;
        _currentState = IdleState;
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

            EventStateChanged?.Invoke(newState);
        }
    }

    public void OnHit(ControllerColliderHit hit, out PlayerCharacter otherPlayer)
    {
        if (_currentState is PlayerDashState dashState)
        {
            bool found = hit.collider.TryGetComponent(out otherPlayer);
            Assert.IsTrue(found && otherPlayer != null || !found);
            return;
        }

        otherPlayer = null;
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

    public void Update()
    {
        PlayerState newState = _currentState.ProcessState();
        if (newState != null)
        {
            _currentState.OnExit();
            newState.OnEnter();
            _currentState = newState;

            EventStateChanged?.Invoke(newState);
        }
    }
}