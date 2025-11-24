using UnityEngine;
using System.Collections.Generic;
using UnityEngine.AI; // NavMeshAgent 사용 가정

public class MonsterAI : MonoBehaviour
{
    [Header("Settings")] public Transform player;
    public float attackRange = 2f;
    public float detectRange = 10f;
    public AttackData[] combo1;
    private NavMeshAgent _agent;
    private BtNode _rootNode;
    private Animator _animator;


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
        // 매 프레임 트리를 평가하여 행동 결정
        if (_rootNode != null)
            _rootNode.Evaluate();
    }

    private void ConstructBehaviorTree()
    {
        //노드들
        BtNode strafeAction = new Leaf_Strafe(transform, player, _agent, _animator, 2f);
        BtNode chase = new Leaf_Chase(transform, player, _animator, _agent, attackRange);
        BtNode checkAttackRange =
            new Leaf_CheckAttackRange(transform, player, attackRange, _animator);
        BtNode wait = new Leaf_Wait(_animator);

        List<BtNode> comboNodes = new List<BtNode>();
        foreach (var data in combo1)
        {
            comboNodes.Add(new Leaf_PerformAttack(transform, player, this, _animator, data));
        }

        SequenceWithMemory comboSequece
            = new SequenceWithMemory(new List<BtNode>()
                { comboNodes[0], comboNodes[1], comboNodes[2] });
        // 공격 범위 밖일 때, 추적 또는 회피 행동을 확률적으로 선택하는 노드
        Selector_Random outOfRangeAction = new Selector_Random(new List<BtNode>() { chase, wait, strafeAction });
        Selector_Random attackOrStrafeAction = new Selector_Random(new List<BtNode>() { comboSequece, strafeAction });
        SequenceWithMemory inRangeAction =
            new SequenceWithMemory(new List<BtNode>() { checkAttackRange, attackOrStrafeAction });
        // 최상위 셀렉터:
        // 1. comboAction을 시도합니다. (comboAction이 내부적으로 거리 체크를 수행)
        // 2. comboAction이 실패하면(거리가 멀면), outOfRangeAction을 실행합니다.
        Selector rootSelector = new Selector(new List<BtNode>() { inRangeAction, outOfRangeAction });

        //실행
        _rootNode = rootSelector;
    }
}