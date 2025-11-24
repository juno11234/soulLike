using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

// 만들어야할 노드
//1. 피격/그로기 확인,
//2. 페이즈 확인,
//3. 공격 가능 여부,
//4. 공격 각도 확인,
//5. 배회,
//6. 공격 전조
//7. 확률 선택지

public class Leaf_CheckAttackRange : BtNode
{
    private Transform _self;
    private Transform _target;
    private Animator _animator;
    private float _range;
    private bool _isStarted;

    public Leaf_CheckAttackRange(Transform self, Transform target, float range, Animator animator)
    {
        _self = self;
        _target = target;
        _range = range;
        _animator = animator;
    }

    public override NodeState Evaluate()
    {
        if (_isStarted == false)
        {
            _isStarted = true;
            _animator.SetFloat("Horizontal", 0);
            _animator.SetFloat("Vertical", 0);
        }

        float distance = Vector3.Distance(_self.position, _target.position);
        return distance <= _range ? NodeState.Success : NodeState.Failure;
    }
}

public class Leaf_Strafe : BtNode
{
    private static readonly int Horizontal = Animator.StringToHash("Horizontal");
    private Transform _self;
    private Transform _target;
    private NavMeshAgent _agent;
    private float _strafeDuration;
    private Animator _animator;

    private float _timer;
    private int _strafeDirection;
    private bool _isStarted;

    public Leaf_Strafe(Transform self, Transform target, NavMeshAgent agent, Animator animator, float strafeDuration)
    {
        _self = self;
        _target = target;
        _agent = agent;
        _animator = animator;
        _strafeDuration = strafeDuration;
        _timer = 0;
        _isStarted = false;
    }

    public override NodeState Evaluate()
    {
        if (_isStarted == false)
        {
            _isStarted = true;
            _strafeDirection = Random.Range(0, 2) == 0 ? -1 : 1;
            //_animator.SetFloat(Horizontal, _strafeDirection);
            _agent.updateRotation = false;
        }

        _timer += Time.deltaTime;
        // 바라보기
        Vector3 lookPos = _target.position;
        lookPos.y = _self.position.y;
        _self.LookAt(lookPos);

        // 이동 방향 계산
        Vector3 targetDir = (_target.position - _self.position).normalized;
        targetDir.y = 0;
        Vector3 strafeDir = Vector3.Cross(targetDir, Vector3.up) * _strafeDirection;
        // 이동 실행
        _agent.Move(strafeDir * (_agent.speed * Time.deltaTime));
        if (_timer >= _strafeDuration)
        {
            _timer = 0;
            _agent.updateRotation = true;
            return NodeState.Success;
        }

        return NodeState.Running;
    }
}

public class Leaf_Chase : BtNode
{
    private static readonly int Vertical = Animator.StringToHash("Vertical");
    private Transform _self;
    private Transform _target;
    private Animator _animator;
    private NavMeshAgent _agent;
    private float _range;

    private bool _isStarted;

    public Leaf_Chase(Transform self, Transform target, Animator animator, NavMeshAgent agent, float range)
    {
        _self = self;
        _target = target;
        _animator = animator;
        _agent = agent;
        _range = range;
    }

    public override NodeState Evaluate()
    {
        if (_isStarted == false)
        {
            _isStarted = true;
            _animator.SetFloat(Vertical, 1f);
        }

        Vector3 lookPos = _target.position;
        lookPos.y = _self.position.y;
        _self.LookAt(lookPos);
        float distance = Vector3.Distance(_self.position, _target.position);
        if (distance > _range)
        {
            Vector3 targetDir = (_target.position - _self.position).normalized;
            targetDir.y = 0;
            _agent.Move(targetDir * (_agent.speed * 1.5f * Time.deltaTime));
            return NodeState.Running;
        }
        else if (distance <= _range)
        {
            return NodeState.Success;
        }

        return NodeState.Failure;
    }
}

public class Leaf_PerformAttack : BtNode
{
    private Transform _self;
    private Transform _target;
    private MonsterAI _monster;
    private Animator _animator;
    private AttackData _attackData;

    private float _timer;
    private bool _isAttacking;
    private bool _isDamaged;

    public Leaf_PerformAttack(Transform self, Transform target, MonsterAI monster, Animator animator, AttackData data)
    {
        _self = self;
        _target = target;
        _monster = monster;
        _animator = animator;
        _attackData = data;
    }

    public override NodeState Evaluate()
    {
        // 1. 공격 시작 (진입)
        if (_isAttacking == false)
        {
            _isAttacking = true;
            _isDamaged = false;
            _timer = 0f;
            // 애니메이션 실행 및 전조 효과(빨간색 등)를 넣을 수 있음
        }

        _timer += Time.deltaTime;

        // 2. 선딜레이 구간 (Wind-up)
        if (_timer < _attackData.windupTime)
        {
            // 이 구간에서는 플레이어를 천천히 바라보게 할지, 멈출지 결정할 수 있음.
            // 소울류는 보통 공격 직전까지는 방향을 살짝 보정해줌.
            RotateTowardsTarget();
            return NodeState.Running;
        }

        // 3. 타격 구간 (Impact)
        if (_timer < _attackData.windupTime + _attackData.activeTime)
        {
            if (_isDamaged == false)
            {
                _animator.CrossFade(_attackData.animTrigger, 0f);
                _isDamaged = true; // 데미지는 한 번만
            }

            return NodeState.Running;
        }

        // 4. 후딜레이 구간 (Recovery)
        if (_timer < _attackData.windupTime + _attackData.activeTime + _attackData.recoveryTime)
        {
            // 이 구간에서는 몬스터가 움직이지 못함 (플레이어의 공격 기회)
            return NodeState.Running;
        }

        // 5. 종료 (모든 시간이 지남)
        _isAttacking = false; // 상태 초기화
        return NodeState.Success; // 공격 행동 완료!
    }

    private void RotateTowardsTarget()
    {
        // 공격 준비 중에는 천천히 회전하여 조준 보정
        Vector3 direction = (_target.position - _self.position).normalized;
        direction.y = 0;
        Quaternion lookRot = Quaternion.LookRotation(direction);
        _self.rotation = Quaternion.Slerp(_self.rotation, lookRot, Time.deltaTime * 5f);
    }
}

public class Sequence_Combo : BtNode
{
    private readonly Transform _self;
    private readonly Transform _target;
    private readonly float _range;
    
    private int _currentIndex = 0;

    public Sequence_Combo(List<BtNode> children, Transform self, Transform target, float range) : base(children)
    {
        _self = self;
        _target = target;
        _range = range;
    }

    public override NodeState Evaluate()
    {
        // 이 노드가 새로 시작될 때만 거리 체크를 수행
        if (State != NodeState.Running)
        {
            float distance = Vector3.Distance(_self.position, _target.position);
            if (distance > _range)
            {
                // 거리가 멀면 콤보를 시작조차 하지 않고 실패 반환
                return NodeState.Failure;
            }
            // 거리가 가깝다면 콤보 인덱스 초기화 후 시작
            _currentIndex = 0;
        }
        
        // 보호 코드: 자식이 없으면 실패
        if (Children.Count == 0) return NodeState.Failure;

        // 현재 진행 중인 공격(자식 노드) 가져오기
        BtNode currentAttack = Children[_currentIndex];
        NodeState result = currentAttack.Evaluate();
        switch (result) //자식의 공격 노드 성공 여부
        {
            case NodeState.Running:
                // 현재 공격이 진행 중이면 상태 유지
                State = NodeState.Running;
                return State;

            case NodeState.Failure:
                // 공격 중 하나라도 실패(캔슬 등)하면 콤보 전체 중단
                _currentIndex = 0;
                State = NodeState.Failure;
                return State;

            case NodeState.Success:
                // 현재 공격이 성공적으로 끝남 (Recovery까지 완료)
                if (_currentIndex >= Children.Count - 1)
                {
                    // 마지막 콤보였으면 전체 성공
                    _currentIndex = 0;
                    State = NodeState.Success;
                    return State;
                }

                // 다음 공격으로 진행
                _currentIndex++;
                State = NodeState.Running; // 트리 자체는 아직 '실행 중'으로 유지
                return State;
        }

        return NodeState.Failure; // 기본적으로 실패를 반환하는 것이 더 안전합니다.
    }
}