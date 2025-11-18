using UnityEngine;

public class AttackState : IState
{
    private PlayerStateMachine _player;
    private readonly int _attack1 = Animator.StringToHash("Attack1");
    private readonly int _attack2 = Animator.StringToHash("Attack2");
    private readonly int _attack3 = Animator.StringToHash("Attack3");

    private bool _animationPlayed = false;
    private bool _2AnimationPlayed = false;

    private bool _secondAttackReady = false;
    private bool _thirdAttackReady = false;

    public AttackState(PlayerStateMachine player)
    {
        _player = player;
    }

    public void Enter()
    {
        if (_player.NoStamina)
        {
            _player.ChangeState(new WalkState(_player));
            return;
        }

        _player.StaminaChange(_player.AttackStamina);
        _player.PlayTargetAniClip(_attack1, 0f);
        _player.OnLMBAction += SecondAttackReady;
        _player.OnLMBAction += ThirdAttackReady;
    }


    public void UpdateLogic()
    {
        _player.ForwardMove(0.5f);
        AnimatorStateInfo stateInfo = _player.Animator.GetCurrentAnimatorStateInfo(0);

        if (stateInfo.shortNameHash == _attack3 && stateInfo.normalizedTime >= 0.9f)
        {
            _player.ChangeState(new WalkState(_player));
        }

        if (stateInfo.shortNameHash == _attack2 && stateInfo.normalizedTime >= 0.9f)
        {
            if (_thirdAttackReady && _2AnimationPlayed == false)
            {
                _player.StaminaChange(_player.AttackStamina);
                _player.PlayTargetAniClip(_attack3, 0.2f);
                _2AnimationPlayed = true;
            }
            else if (_thirdAttackReady == false)
            {
                _player.ChangeState(new WalkState(_player));
            }
        }

        if (stateInfo.shortNameHash == _attack1 && stateInfo.normalizedTime >= 0.9f)
        {
            if (_secondAttackReady && _animationPlayed == false)
            {
                _player.StaminaChange(_player.AttackStamina);
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
        _player.OnLMBAction -= SecondAttackReady;
        _player.OnLMBAction -= ThirdAttackReady;
        _secondAttackReady = false;
        _thirdAttackReady = false;
    }


    private void SecondAttackReady(bool isPressed)
    {
        if (isPressed && _player.NoStamina == false)
        {
            _secondAttackReady = true;
        }
    }

    private void ThirdAttackReady(bool isPressed)
    {
        if (isPressed && _animationPlayed && _player.NoStamina == false)
        {
            _thirdAttackReady = true;
        }
    }
}