using UnityEngine;

public interface IState
{
    public void Enter();
    public void UpdateLogic();
    public void Exit();
}