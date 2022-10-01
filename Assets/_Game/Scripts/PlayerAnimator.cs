using UnityEngine;

public class PlayerAnimator
{
    private Animator _animator;
    private PlayerParamsSO _playerParams;

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
        _animator.CrossFade(IdleStateName, _playerParams.AnimationTranstitionDuration, 0);
    }

    public void Move()
    { 
        _animator.CrossFade(MoveStateName, _playerParams.AnimationTranstitionDuration, 0);
    }

    public void Dash()
    { 
        _animator.CrossFade(DashStateName, _playerParams.AnimationTranstitionDuration, 0);
    }
}