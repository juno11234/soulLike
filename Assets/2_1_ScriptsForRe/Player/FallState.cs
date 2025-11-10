using UnityEngine;

public class FallState : IState
{
    private PlayerStateMachine _player;
    private readonly int _land = Animator.StringToHash("Land");
    private readonly int _fall = Animator.StringToHash("Falling");

    public FallState(PlayerStateMachine player)
    {
        _player = player;
    }

    public void Enter()
    {
        _player.PlayTargetAniClip(_fall);
    }

    public void UpdateLogic()
    {
        if (_player.IsGrounded())
        {
            _player.ChangeState(new WalkState(_player));
        }
    }

    public void Exit()
    {
        _player.PlayTargetAniClip(_land);
    }
}