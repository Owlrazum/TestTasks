using UnityEngine;

public class PlayerAnimator
{
    private Animator _animator;
    private PlayerParamsSO _playerParams;

    /// "Pascal Case. The use of SCREAMING_CAPS is discouraged. This is a large change from earlier conventions. 
    /// Most developers now realize that in using SCREAMING_CAPS they betray more implementation than is necessary."
    /// https://en.wikibooks.org/wiki/C_Sharp_Programming/Naming
    private const int IdleState = 0;
    private const int MoveState = 1;
    private const int DashState = 2;

    private const string IdleStateName = "Base Layer.Idle01";
    private const string MoveStateName = "Base Layer.Run";
    private const string DashStateName = "Base Layer.Dash";

    public PlayerAnimator(Animator animator, PlayerParamsSO playerParams)
    {
        _animator = animator;
        _playerParams = playerParams;
    }

    public void Idle()
    {
        _animator.SetInteger("State", IdleState);
    }

    public void Move()
    { 
        _animator.SetInteger("State", MoveState);
    }

    public void Dash()
    { 
        _animator.SetInteger("State", DashState);
    }
}