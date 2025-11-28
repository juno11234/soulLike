using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using N = CommonNodes;

public enum CommonNodes
{
    Strafe,
    Chase,
    CheckAttackRange,
    Wait,
    Cleaner,
    BackMove,
    WatchPlayer,
    Patrol,
}

public abstract class EnemyAIBase : MonoBehaviour
{
    [Header("Settings")] public Transform player;
    public float attackRange = 3f;
    public AttackData[] attackData;
    public float viewAngle;
    public float viewDistance;
    public Transform[] patrolPoint;
    public bool findPlayer;

    protected bool IsDead;
    protected Dictionary<N, BtNode> nodeDict = new Dictionary<N, BtNode>();

    protected NavMeshAgent _agent;
    protected BtNode _rootNode;
    protected Animator _animator;
    protected RootMotionHandler _rootMotionHandler;

    protected virtual void ConstructBehaviorTree()
    {
        #region 노드들

        nodeDict[N.Strafe] = new Leaf_Strafe(transform, player, _agent, _animator, 2f);
        nodeDict[N.Chase] = new Leaf_Chase(transform, player, _animator, _agent);
        nodeDict[N.CheckAttackRange] = new Leaf_CheckAttackRange(transform, player, attackRange);
        nodeDict[N.Wait] = new Leaf_Wait(_animator, _agent);
        nodeDict[N.Cleaner] = new Leaf_Cleaner(_animator, _agent);
        nodeDict[N.BackMove] = new Leaf_BackMove(_animator, _agent, transform, player);
        nodeDict[N.WatchPlayer] = new Leaf_WatchPlayer(player, transform, viewAngle, viewDistance, this);
        nodeDict[N.Patrol] = new Leaf_Patrol(patrolPoint, _agent, _animator);

        #endregion
    }

    public void Dead()
    {
        IsDead = true;
        _agent.enabled = false;
    }
}