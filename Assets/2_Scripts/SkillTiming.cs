using UnityEngine;

public class SkillTiming : StateMachineBehaviour
{
    [SerializeField] private int firstIndex;
    [SerializeField] private int secondIndex;
    [Range(0f, 1f)] public float startNormalizedTime = 0f;

    private bool _passStartTime;

    [Range(0f, 1f)] public float endNormalizedTime = 0f;

    private bool _passEndTime;

    private ISkillAble _self;

    // OnStateEnter is called when a transition starts and the state machine starts to evaluate this state
    override public void OnStateEnter(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        _passStartTime = false;
        _passEndTime = false;
        _self = animator.gameObject.GetComponentInParent<ISkillAble>();
    }

    // OnStateUpdate is called on each Update frame between OnStateEnter and OnStateExit callbacks
    override public void OnStateUpdate(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    {
        if (_passStartTime == false && startNormalizedTime < stateInfo.normalizedTime)
        {
            _self.UseSkillFirstTiming(firstIndex);
            _passStartTime = true;
        }

        if (_passEndTime == false && endNormalizedTime < stateInfo.normalizedTime)
        {
            _self.UseSkillSecondTiming(secondIndex);
            _passEndTime = true;
        }
    }
    // OnStateMove is called right after Animator.OnAnimatorMove()
    //override public void OnStateMove(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    //{
    //    // Implement code that processes and affects root motion
    //}

    // OnStateIK is called right after Animator.OnAnimatorIK()
    //override public void OnStateIK(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
    //{
    //    // Implement code that sets up animation IK (inverse kinematics)
    //}
}