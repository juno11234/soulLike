using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using N = CommonNodes;

public class SkeletonAI : EnemyAIBase
{

    private void Awake()
    {
        _agent = GetComponent<NavMeshAgent>();
        _animator = GetComponentInChildren<Animator>();
    }

    private void Start()
    {
        ConstructBehaviorTree();
    }

    private void Update()
    {
        if (_rootNode != null)
            _rootNode.Evaluate();
    }

    public override void ConstructBehaviorTree()
    {
        base.ConstructBehaviorTree();

        Sequence_Memory backMoveAndWait = new Sequence_Memory(new List<BtNode>()
        {
            commonNodes[N.BackMove], commonNodes[N.Wait]
        });
        Sequence_Memory backWalkCheck = new Sequence_Memory(new List<BtNode>()
        {
            commonNodes[N.CheckAttackRange], commonNodes[N.Cleaner], backMoveAndWait
        });
        Sequence_Memory innerStrafeSequence = new Sequence_Memory(new List<BtNode>()
        {
            commonNodes[N.CheckAttackRange], commonNodes[N.Cleaner], commonNodes[N.Strafe]
        });

        #region 공격 행동

        List<BtNode> attackNodes = new List<BtNode>();
        foreach (var data in attackData)
        {
            attackNodes.Add(new Leaf_PerformAttack(transform, player, _animator, data,
                false, _rootMotionHandler, 1f));
        }

        Sequence_Memory fullComboSequence = new Sequence_Memory(new List<BtNode>()
        {
            commonNodes[N.CheckAttackRange], commonNodes[N.Cleaner],
            attackNodes[0], attackNodes[1], attackNodes[2]
        });
        Sequence_Memory halfComboSequence = new Sequence_Memory(new List<BtNode>()
        {
            commonNodes[N.CheckAttackRange], commonNodes[N.Cleaner],
            attackNodes[0], attackNodes[1]
        });
        Selector_Random comboOrStrafeAction = new Selector_Random(new List<BtNode>()
        {
            fullComboSequence, halfComboSequence, innerStrafeSequence, backWalkCheck
        });

        #endregion

        Sequence_Memory outOfRangeAction = new Sequence_Memory(new List<BtNode>()
        {
            commonNodes[N.Chase], commonNodes[N.Wait]
        });
        Selector walkCheck = new Selector(new List<BtNode>()
        {
            commonNodes[N.CheckAttackRange], outOfRangeAction
        });
        Selector_Random randomOutAction = new Selector_Random(new List<BtNode>()
        {
            walkCheck, commonNodes[N.Strafe]
        });
        Selector actions = new Selector(new List<BtNode>()
        {
            comboOrStrafeAction, randomOutAction,
        });

        _rootNode = actions;
    }
}