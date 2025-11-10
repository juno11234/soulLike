using UnityEngine;

public class AttackState : IState
{
    private PlayerStateMachine _player;
    private readonly int _attack1 = Animator.StringToHash("Attack1");
    private readonly int _attack2 = Animator.StringToHash("Attack2");

    private bool _secondAttackReady = false;
    private bool _animationPlayed = false;

    public AttackState(PlayerStateMachine player)
    {
        _player = player;
    }

    public void Enter()
    {
        _player.PlayTargetAniClip(_attack1, 0f);
        _player.OnLMBAction += SecondAttack;
    }

    public void UpdateLogic()
    {
        _player.ForwardMove(0.5f);
        AnimatorStateInfo stateInfo = _player.Animator.GetCurrentAnimatorStateInfo(0);
        if (stateInfo.IsName("Attack2") && stateInfo.normalizedTime >= 0.9f)
        {
            _player.ChangeState(new WalkState(_player));
        }

        if (stateInfo.IsName("Attack1") && stateInfo.normalizedTime >= 0.9f)
        {
            if (_secondAttackReady && _animationPlayed == false)
            {
                _player.PlayTargetAniClip(_attack2, 0.2f);
                _animationPlayed = true;
            }
            else if (_secondAttackReady == false)
            {
                _player.ChangeState(new WalkState(_player));
            }
        }
    }

    public void Exit()
    {
        _player.OnLMBAction -= SecondAttack;
        _secondAttackReady = false;
    }

    private void SecondAttack(bool isPressed)
    {
        if (isPressed)
        {
            _secondAttackReady = true;
        }
    }
}