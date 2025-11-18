using UnityEngine;

public class SprintState : IState
{
    private PlayerStateMachine _player;

    public SprintState(PlayerStateMachine player)
    {
        _player = player;
    }

    public void Enter()
    {
        
    }

    public void UpdateLogic()
    {
        if (_player.NoStamina)
        {
            _player.ChangeState(new WalkState(_player));
            return;
        }
        
        if (_player.SpaceBarPressed == false)
        {
            _player.ChangeState(new WalkState(_player));
        }

        _player.Movement(_player.SprintSpeed);
        _player.HandleMove_Ani(_player.MoveAmount, 0f, true);
    }

    public void Exit()
    {
    }
}