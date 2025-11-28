using UnityEngine;
using System.Collections.Generic;
using UnityEngine.AI; // NavMeshAgent 사용 가정
using N = CommonNodes;

public class BossMonsterAI : EnemyAIBase
{
    public float skillRange = 5f;
    public AttackData[] skillData;

    [Header("Debug")] public bool secondPhase = false;
    public bool skillCool = false;
    public float skillTimer = 0f;

    private void Awake()
    {
        _agent = GetComponent<NavMeshAgent>();
        _animator = GetComponentInChildren<Animator>();
        _rootMotionHandler = _animator.gameObject.GetComponent<RootMotionHandler>();
    }

    private void Start()
    {
        ConstructBehaviorTree();
    }

    private void Update()
    {
        if (IsDead) return;
        
        if (skillCool)
        {
            skillTimer += Time.deltaTime;
        }

        if (skillTimer >= 15f)
        {
            skillCool = false;
            skillTimer = 0f;
        }

        // 매 프레임 트리를 평가하여 행동 결정
        if (_rootNode != null)
            _rootNode.Evaluate();
    }


    protected override void ConstructBehaviorTree()
    {
        base.ConstructBehaviorTree();

        #region 노드들

        BtNode checkSkillAble = new Leaf_CheckSkillAble(this);
        BtNode checkSkillRange = new Leaf_CheckAttackRange(transform, player, skillRange);

        #endregion

        Sequence_Memory backMoveAndWait = new Sequence_Memory(new List<BtNode>()
        {
            nodeDict[N.BackMove], nodeDict[N.Wait]
        });
        Sequence_Memory backWalkCheck = new Sequence_Memory(new List<BtNode>()
        {
            nodeDict[N.CheckAttackRange], nodeDict[N.Cleaner], backMoveAndWait
        });
        Sequence_Memory innerStrafeSequence = new Sequence_Memory(new List<BtNode>()
        {
            nodeDict[N.CheckAttackRange], nodeDict[N.Cleaner], nodeDict[N.Strafe]
        });

        #region 공격 관련

        List<BtNode> attackNodes = new List<BtNode>();
        foreach (var data in attackData)
        {
            attackNodes.Add(new Leaf_PerformAttack(transform, player, _animator, data,
                false, _rootMotionHandler, 3f));
        }

        List<BtNode> skillNodes = new List<BtNode>();
        foreach (var data in skillData)
        {
            skillNodes.Add(new Leaf_PerformAttack(transform, player, _animator, data,
                true, _rootMotionHandler, 3f));
        }

        Sequence_Memory fullComboSequence = new Sequence_Memory(new List<BtNode>()
        {
            nodeDict[N.CheckAttackRange], nodeDict[N.Cleaner],
            attackNodes[0], attackNodes[1], attackNodes[2]
        });

        Sequence_Memory halfComboSequence = new Sequence_Memory(new List<BtNode>()
        {
            nodeDict[N.CheckAttackRange], nodeDict[N.Cleaner],
            attackNodes[0], attackNodes[1]
        });

        Selector_Random comboOrStrafeAction = new Selector_Random(new List<BtNode>()
        {
            fullComboSequence, halfComboSequence, innerStrafeSequence, backWalkCheck
        });

        Selector_Random skillRangeAction = new Selector_Random(new List<BtNode>()
        {
            skillNodes[0], skillNodes[1]
        });
        Sequence_Memory skillUseSequence = new Sequence_Memory(new List<BtNode>()
        {
            checkSkillRange, checkSkillAble, nodeDict[N.Cleaner], skillRangeAction
        });

        Selector skillOrCombo = new Selector(new List<BtNode>()
        {
            skillUseSequence, comboOrStrafeAction
        });

        #endregion

        Sequence_Memory outOfRangeAction = new Sequence_Memory(new List<BtNode>()
        {
            nodeDict[N.Chase], nodeDict[N.Wait]
        });


        Selector walkCheck = new Selector(new List<BtNode>()
        {
            nodeDict[N.CheckAttackRange], outOfRangeAction
        });


        Selector_Random randomOutAction = new Selector_Random(new List<BtNode>()
        {
            walkCheck, nodeDict[N.Strafe]
        });

        Selector actions = new Selector(new List<BtNode>()
        {
            skillOrCombo, randomOutAction,
        });

        //실행
        _rootNode = actions;
    }
}