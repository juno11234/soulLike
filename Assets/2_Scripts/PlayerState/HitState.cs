using UnityEngine;

public class HitState : IState
{
    private PlayerStateMachine _player;
    private readonly int _hit = Animator.StringToHash("GetHit");

    public HitState(PlayerStateMachine player)
    {
        _player = player;
    }

    public void Enter()
    {
        _player.PlayTargetAniClip(_hit, 0f);
    }

    public void UpdateLogic()
    {
        AnimatorStateInfo stateInfo = _player.Animator.GetCurrentAnimatorStateInfo(0);
        if (stateInfo.IsName("GetHit") && stateInfo.normalizedTime >= 0.9f)
        {
            _player.ChangeState(new WalkState(_player));
        }
    }

    public void Exit()
    {
    }
}