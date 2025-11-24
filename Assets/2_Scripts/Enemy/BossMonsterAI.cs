using UnityEngine;
using System.Collections.Generic;
using UnityEngine.AI; // NavMeshAgent 사용 가정

public class BossMonsterAI : MonoBehaviour
{
    [Header("Settings")] public Transform player;
    public float attackRange = 3f;
    public float skillRange = 5f;
    public AttackData[] combo1;
    public AttackData[] skillData;

    public bool secondPhase = false;
    public bool skillCool = false;
    public float skillTimer = 0f;

    private RootMotionHandler _rootMotionHandler;
    private NavMeshAgent _agent;
    private BtNode _rootNode;
    private Animator _animator;


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

    private void ConstructBehaviorTree()
    {
        #region 노드들

        BtNode checkSkillAble = new Leaf_CheckSkillAble(this);
        BtNode strafeAction = new Leaf_Strafe(transform, player, _agent, _animator, 2f);
        BtNode chase = new Leaf_Chase(this, transform, player, _animator, _agent, attackRange, skillRange);
        BtNode checkAttackRange =
            new Leaf_CheckAttackRange(transform, player, attackRange, _animator, _agent);
        BtNode checkSkillRange = new Leaf_CheckAttackRange(transform, player, skillRange, _animator, _agent);
        BtNode wait = new Leaf_Wait(_animator);

        #endregion

        #region 공격 관련

        List<BtNode> comboNodes = new List<BtNode>();
        foreach (var data in combo1)
        {
            comboNodes.Add(new Leaf_PerformAttack(transform, player, this, _animator, data,
                false, _rootMotionHandler));
        }

        List<BtNode> skillNodes = new List<BtNode>();
        foreach (var data in skillData)
        {
            skillNodes.Add(new Leaf_PerformAttack(transform, player, this, _animator, data,
                true, _rootMotionHandler));
        }

        Sequence_Memory fullComboSequence
            = new Sequence_Memory(new List<BtNode>()
            {
                checkAttackRange, comboNodes[0], comboNodes[1], comboNodes[2]
            });

        Sequence_Memory halfComboSequence
            = new Sequence_Memory(new List<BtNode>()
            {
                checkAttackRange, comboNodes[0], comboNodes[1]
            });

        Selector_Random comboOrStrafeAction =
            new Selector_Random(new List<BtNode>()
            {
                fullComboSequence, halfComboSequence, strafeAction
            });
        Selector_Random skillRangeAction = new Selector_Random(new List<BtNode>()
        {
            skillNodes[0], skillNodes[1]
        });
        Sequence_Memory skillUseSequence = new Sequence_Memory(new List<BtNode>()
        {
            checkSkillRange, checkSkillAble, skillRangeAction
        });

        Selector skillOrCombo = new Selector(new List<BtNode>()
        {
            skillUseSequence, comboOrStrafeAction
        });

        #endregion

        // 공격 범위 밖일 때, 추적 또는 회피 행동을 확률적으로 선택하는 노드
        Selector_Random outOfRangeAction = new Selector_Random(new List<BtNode>() { chase, wait });


        Sequence_Memory inRangeAction =
            new Sequence_Memory(new List<BtNode>() { outOfRangeAction, skillOrCombo });

        //실행
        _rootNode = skillOrCombo;
    }
}