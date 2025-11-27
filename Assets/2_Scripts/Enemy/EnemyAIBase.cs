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
    BackMove
}

public abstract class EnemyAIBase : MonoBehaviour
{
    [Header("Settings")] public Transform player;
    public float attackRange = 3f;
    public AttackData[] attackData;

    protected Dictionary<N, BtNode> commonNodes = new Dictionary<N, BtNode>();

    protected NavMeshAgent _agent;
    protected BtNode _rootNode;
    protected Animator _animator;
    protected RootMotionHandler _rootMotionHandler;

    public virtual void ConstructBehaviorTree()
    {
        #region 노드들

        commonNodes[N.Strafe] = new Leaf_Strafe(transform, player, _agent, _animator, 2f);
        commonNodes[N.Chase] = new Leaf_Chase(transform, player, _animator, _agent);
        commonNodes[N.CheckAttackRange] = new Leaf_CheckAttackRange(transform, player, attackRange);
        commonNodes[N.Wait] = new Leaf_Wait(_animator, _agent);
        commonNodes[N.Cleaner] = new Leaf_Cleaner(_animator, _agent);
        commonNodes[N.BackMove] = new Leaf_BackMove(_animator, _agent, transform, player);

        #endregion
    }
}