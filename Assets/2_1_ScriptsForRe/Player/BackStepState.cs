using UnityEngine;

public class BackStepState : IState
{
    private PlayerStateMachine _player;
    private readonly int _backStep = Animator.StringToHash("Backstep");

    public BackStepState(PlayerStateMachine player)
    {
        _player = player;
    }

    public void Enter()
    {
        _player.PlayTargetAniClip(_backStep,0f);
    }

    public void UpdateLogic()
    {
        _player.Backstep(_player.BackstepSpeed);
        AnimatorStateInfo stateInfo = _player.Animator.GetCurrentAnimatorStateInfo(0);
        if (stateInfo.IsName("Backstep") && stateInfo.normalizedTime >= 0.9f)
        {
            _player.ChangeState(new WalkState(_player));
        }
    }

    public void Exit()
    {
    }
}