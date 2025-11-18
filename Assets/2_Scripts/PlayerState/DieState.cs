using UnityEngine;

public class DieState : IState
{
    private PlayerStateMachine _player;
    private readonly int _die = Animator.StringToHash("Die");

    public DieState(PlayerStateMachine player)
    {
        _player = player;
    }

    public void Enter()
    {
        _player.PlayTargetAniClip(_die,0.2f);
    }

    public void UpdateLogic()
    {
    }

    public void Exit()
    {
    }
}