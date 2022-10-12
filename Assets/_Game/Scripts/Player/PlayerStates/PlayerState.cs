using UnityEngine;
public abstract class PlayerState
{
    protected PlayerStatesController _statesController;
    protected float _currentVerticalSpeed;
    protected float _gravity;

    public PlayerState(PlayerParamsSO playerParams, PlayerStatesController statesController)
    {
        _gravity = playerParams.Gravity;
        _statesController = statesController;
    }

    public virtual void OnEnter()
    {
        _currentVerticalSpeed = 0;
    }
    public virtual void OnExit(){}

    public virtual PlayerState ReactToCommand(PlayerCommand command)
    {
        return null;
    }

    public virtual PlayerState ProcessState()
    {
        return null;
    }

    protected void CheckGravityMove()
    { 
        if (!_statesController.Player.isGrounded)
        {
            _currentVerticalSpeed += _gravity * Time.deltaTime;
            _statesController.Player.Move(Vector3.down * _currentVerticalSpeed * Time.deltaTime);
        }
        else
        {
            _currentVerticalSpeed = 0;
        }
    }
}