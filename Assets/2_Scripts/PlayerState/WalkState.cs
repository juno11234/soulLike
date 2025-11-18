using System;
using UnityEngine;

public class WalkState : IState
{
    private PlayerStateMachine _player;
    private float _timer = 0f;
    private float _staminaTimer = 0f;

    public WalkState(PlayerStateMachine player)
    {
        _player = player;
    }

    public void Enter()
    {
        _player.OnLMBAction += EnterAttackState;
    }

    public void UpdateLogic()
    {
        if (_player.IsGrounded() == false)
        {
            _player.ChangeState(new FallState(_player));
        }

        if (_player.SpaceBarPressed)
        {
            _timer += Time.deltaTime;
        }

        if (_timer > 0.3f && _player.SpaceBarPressed)
        {
            _player.ChangeState(new SprintState(_player));
        }
        else if (_timer is > 0f and < 0.3f && _player.SpaceBarPressed == false)
        {
            if (_player.MoveAmount > 0)
            {
                _player.ChangeState(new RollState(_player));
            }
            else
            {
                _player.ChangeState(new BackStepState(_player));
            }
        }

        _player.Movement(_player.WalkSpeed);
        _player.HandleMove_Ani(_player.MoveAmount, 0f, false);
    }

    public void Exit()
    {
        _timer = 0f;
        _staminaTimer = 0f;
        _player.OnLMBAction -= EnterAttackState;
    }

    private void EnterAttackState(bool isPressed)
    {
        if (isPressed)
        {
            _player.ChangeState(new AttackState(_player));
        }
    }
}