using UnityEngine;

public class RollState : IState
{
    private PlayerStateMachine _player;
    private readonly int _roll = Animator.StringToHash("Rolling");

    public RollState(PlayerStateMachine player)
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

        _player.StaminaChange(_player.RollStamina);
        _player.PlayTargetAniClip(_roll, 0.2f);
        _player.ActiveInvisible(true);
    }

    public void UpdateLogic()
    {
        _player.ForwardMove(_player.RollSpeed);
        AnimatorStateInfo stateInfo = _player.Animator.GetCurrentAnimatorStateInfo(0);
        if (stateInfo.IsName("Rolling") && stateInfo.normalizedTime >= 0.9f)
        {
            _player.ChangeState(new WalkState(_player));
        }
    }

    public void Exit()
    {
        _player.ActiveInvisible(false);
    }
}