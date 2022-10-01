using UnityEngine;

public class PlayerDashState : PlayerState
{
    private bool _isDashing;
    private Vector3 _direction;
    private float _distance;
    public PlayerDashState(PlayerStatesController statesControlller) : base(statesControlller)
    { 
        
    }

    public override PlayerState ReactToCommand(PlayerCommand command)
    {
        if (_isDashing)
        {
            return null;
        }

        if (command is PlayerDashCommand dashCommand)
        { 
            _isDashing = true;
            _direction = dashCommand.Direction;
            _distance = dashCommand.Distance;
        }

        return null;
    }

    public override PlayerState ProcessState()
    {
        if (_distance <= 0)
        {
            return _statesController.IdleState;
        }

        return null;
    }

    public override string ToString()
    {
        return "DashState";
    }
}