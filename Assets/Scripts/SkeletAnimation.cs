using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Animator))]
public class SkeletAnimation : MonoBehaviour
{
    [SerializeField]
    private Animator animator;
    private float idleToMove;
    private void Start()
    {
        idleToMove = 0;
    }
    public enum AnimationState
    {
        IdleSlowWalkBlend,
        IdleWalkBlend,
        IdleRunBlend,
        PickingUp
    };
    private AnimationState animationStateData;
    public AnimationState animationState
    {
        get { return animationStateData; }
        set
        {
            animationStateData = value;
            switch (animationStateData)
            {
                case AnimationState.IdleSlowWalkBlend:
                    animator.CrossFade("Base Layer.BS_IdleToSlowWalk", 0.1f);
                    animator.SetFloat("IdleToMove", idleToMove);
                    break;
                case AnimationState.IdleWalkBlend:
                    animator.CrossFade("Base Layer.BS_IdleToWalk", 0.1f);
                    animator.SetFloat("IdleToMove", idleToMove);
                    break;
                case AnimationState.IdleRunBlend:
                    animator.CrossFade("Base Layer.BS_IdleToRun", 0.1f);
                    animator.SetFloat("IdleToMove", idleToMove);
                    break;
                case AnimationState.PickingUp:
                    animator.CrossFade("Base Layer.PickUp", 0.1f);
                    //animator.SetFloat("IdleToRun", idleToRun);
                    break;
            }
        }
    }
    public void UpdateBlendData(float idleToMove_Arg)
    {
        idleToMove = idleToMove_Arg;
        animator.SetFloat("IdleToMove", idleToMove);
    }
}
