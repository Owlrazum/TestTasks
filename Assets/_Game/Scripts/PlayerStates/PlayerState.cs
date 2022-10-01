using UnityEngine;
public abstract class PlayerState
{
    protected PlayerStatesController _statesController;

    public PlayerState(PlayerParamsSO playerParams, PlayerStatesController statesController)
    {
        _gravity = playerParams.Gravity;
        _statesController = statesController;
    }

    public virtual void OnEnter(){}
    public virtual void OnExit(){}

    public virtual PlayerState ReactToCommand(PlayerCommand command)
    {
        return null;
    }

    public virtual PlayerState ProcessState()
    {
        return null;
    }

    protected float _currentVerticalSpeed;
    protected float _gravity;
    protected void CheckGravityMove()
    { 
        if (!_statesController.Player.isGrounded)
        {
            _currentVerticalSpeed += _gravity;
            _statesController.Player.Move(Vector3.down * _currentVerticalSpeed * Time.deltaTime);
        }
        else
        {
            _currentVerticalSpeed = 0;
        }
    }
}