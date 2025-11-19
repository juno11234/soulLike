using UnityEngine;
using System.Collections.Generic;
using UnityEngine.AI; // NavMeshAgent 사용 가정

public class MonsterAI : MonoBehaviour
{
    [Header("Settings")]
    public Transform player;
    public float attackRange = 2f;
    public float detectRange = 10f;
    public List<Transform> patrolPoints;

    private NavMeshAgent agent;
    private BtNode rootNode;

    // 상태 공유를 위한 간단한 데이터 변수들
    private int currentPatrolIndex = 0;

    private void Awake()
    {
        agent = GetComponent<NavMeshAgent>();
    }

    private void Start()
    {
        ConstructBehaviorTree();
    }

    private void Update()
    {
        // 매 프레임 트리를 평가하여 행동 결정
        if(rootNode != null)
            rootNode.Evaluate();
    }

    private void ConstructBehaviorTree()
    {
        // 1. 행동 노드 정의 (Leaf Nodes)
        // 람다 식이나 별도의 클래스로 만들 수 있습니다. 여기서는 이해를 위해 별도 클래스로 분리하는 대신
        // MonsterActionNode라는 커스텀 노드를 예시로 보여드립니다.
        // 실제 구현에서는 각각을 class CheckAttackRange : Node { ... } 처럼 만드는 것이 OOP 원칙에 더 부합합니다.
        
        BtNode checkAttackRange = new Leaf_CheckAttackRange(transform, player, attackRange);
        BtNode attackAction = new Leaf_Attack(player);
        
        BtNode checkDetectRange = new Leaf_CheckDetectRange(transform, player, detectRange);
        BtNode chaseAction = new Leaf_Chase(agent, player);
        
        BtNode patrolAction = new Leaf_Patrol(agent, patrolPoints);

        // 2. 트리 구조 조립
        
        // 공격 시퀀스: 사거리 내에 있으면(Check) -> 공격한다(Action)
        Sequence attackSequence = new Sequence(new List<BtNode> { checkAttackRange, attackAction });

        // 추적 시퀀스: 감지 범위 내에 있으면(Check) -> 쫓아간다(Action)
        Sequence chaseSequence = new Sequence(new List<BtNode> { checkDetectRange, chaseAction });

        // 3. 최상위 Selector (우선순위 결정)
        // 공격 가능하면 공격 -> 아니면 추적 -> 아니면 순찰
        rootNode = new Selector(new List<BtNode> { attackSequence, chaseSequence, patrolAction });
    }
}

// --- 아래는 Leaf 노드 구현 예시입니다 (한 파일에 넣거나 분리 가능) ---

public class Leaf_CheckAttackRange : BtNode
{
    private Transform _self;
    private Transform _target;
    private float _range;

    public Leaf_CheckAttackRange(Transform self, Transform target, float range)
    {
        _self = self; _target = target; _range = range;
    }

    public override NodeState Evaluate()
    {
        if (_target == null) return NodeState.Failure;
        float distance = Vector3.Distance(_self.position, _target.position);
        return distance <= _range ? NodeState.Success : NodeState.Failure;
    }
}

public class Leaf_Attack : BtNode
{
    private Transform _target;
    public Leaf_Attack(Transform target) { _target = target; }

    public override NodeState Evaluate()
    {
        Debug.Log("공격 중!"); 
        // 여기에 실제 공격 애니메이션/로직 추가
        return NodeState.Success; 
    }
}

public class Leaf_CheckDetectRange : BtNode
{
    private Transform _self;
    private Transform _target;
    private float _range;
    public Leaf_CheckDetectRange(Transform self, Transform target, float range)
    {
        _self = self; _target = target; _range = range;
    }
    public override NodeState Evaluate()
    {
        if (_target == null) return NodeState.Failure;
        float distance = Vector3.Distance(_self.position, _target.position);
        return distance <= _range ? NodeState.Success : NodeState.Failure;
    }
}

public class Leaf_Chase : BtNode
{
    private NavMeshAgent _agent;
    private Transform _target;
    public Leaf_Chase(NavMeshAgent agent, Transform target)
    {
        _agent = agent; _target = target;
    }
    public override NodeState Evaluate()
    {
        _agent.SetDestination(_target.position);
        Debug.Log("추적 중...");
        return NodeState.Running;
    }
}

public class Leaf_Patrol : BtNode
{
    private NavMeshAgent _agent;
    private List<Transform> _points;
    private int _index = 0;

    public Leaf_Patrol(NavMeshAgent agent, List<Transform> points)
    {
        _agent = agent; _points = points;
    }

    public override NodeState Evaluate()
    {
        if (_points.Count == 0) return NodeState.Failure;

        if (!_agent.pathPending && _agent.remainingDistance < 0.5f)
        {
            _index = (_index + 1) % _points.Count;
            _agent.SetDestination(_points[_index].position);
        }
        
        Debug.Log("순찰 중...");
        return NodeState.Running;
    }
}