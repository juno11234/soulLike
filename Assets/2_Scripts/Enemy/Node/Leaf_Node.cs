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
    private readonly Transform _self;
    private readonly Transform _target;
    private readonly float _range;

    public Leaf_CheckAttackRange(Transform self, Transform target, float range)
    {
        _self = self;
        _target = target;
        _range = range;
    }

    public override NodeState Evaluate()
    {
        float distance = Vector3.Distance(_self.position, _target.position);
        if (distance <= _range)
        {
            // [책임 추가] 범위 안에 들어왔으므로, 모든 이동을 확실하게 멈춘다.
            // 혹시 모를 이전 경로를 완전히 초기화
            return NodeState.Success;
        }
        else
        {
            // [책임 추가] 범위 밖이므로, 다른 노드가 AI를 움직일 수 있도록 허용한다.
            return NodeState.Failure;
        }
    }
}

public class Leaf_Cleaner : BtNode
{
    private readonly Animator _animator;
    private readonly NavMeshAgent _agent;

    public Leaf_Cleaner(Animator animator, NavMeshAgent agent)
    {
        _animator = animator;
        _agent = agent;
    }

    public override NodeState Evaluate()
    {
        _animator.SetFloat("Vertical", 0f);
        _animator.SetFloat("Horizontal", 0f);
        _agent.isStopped = true;
        _agent.ResetPath();

        return NodeState.Success;
    }
}

public class Leaf_CheckSkillAble : BtNode
{
    private BossMonsterAI _boss;

    public Leaf_CheckSkillAble(BossMonsterAI boss)
    {
        _boss = boss;
    }

    public override NodeState Evaluate()
    {
        if (_boss.secondPhase && _boss.skillCool == false)
        {
            _boss.skillCool = true;
            return NodeState.Success;
        }
        else
        {
            return NodeState.Failure;
        }
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
            _agent.updateRotation = false;
        }

        _animator.SetFloat(Horizontal, _strafeDirection, 0.12f, Time.deltaTime);
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
            _isStarted = false;
            _animator.SetFloat(Horizontal, 0);
            _agent.updateRotation = true;
            return NodeState.Success;
        }

        return NodeState.Running;
    }
}

public class Leaf_Chase : BtNode
{
    private static readonly int Vertical = Animator.StringToHash("Vertical");
    private BossMonsterAI _boss;
    private Transform _self;
    private Transform _target;
    private Animator _animator;
    private NavMeshAgent _agent;


    private bool _isStarted;
    private float _timer;

    public Leaf_Chase(BossMonsterAI boss, Transform self, Transform target, Animator animator, NavMeshAgent agent)
    {
        _boss = boss;
        _self = self;
        _target = target;
        _animator = animator;
        _agent = agent;
        _timer = 0;
    }

    public override NodeState Evaluate()
    {
        if (_isStarted == false)
        {
            _isStarted = true;
            _agent.isStopped = false; // 에이전트 이동 시작
        }

        _timer += Time.deltaTime;
        _animator.SetFloat(Vertical, 1f, 0.12f, Time.deltaTime);

        // 플레이어를 계속 바라보게 함
        Vector3 lookPos = _target.position;
        lookPos.y = _self.position.y;
        //_self.LookAt(lookPos);

        if (_timer >= 3f)
        {
            _agent.isStopped = true;
            _agent.ResetPath();
            _isStarted = false; // 다음 추적을 위해 상태 초기화
            _animator.SetFloat(Vertical, 0f);
            _timer = 0f;
            return NodeState.Success;
        }

        _agent.SetDestination(_target.position);
        return NodeState.Running;
    }
}

public class Leaf_Wait : BtNode
{
    private Animator _animator;
    private NavMeshAgent _agent;
    private bool _isStarted;
    private float _timer;

    public Leaf_Wait(Animator animator, NavMeshAgent agent)
    {
        _animator = animator;
        _agent = agent;
    }

    public override NodeState Evaluate()
    {
        if (_isStarted == false)
        {
            _agent.ResetPath();

            _isStarted = true;
        }

        _timer += Time.deltaTime;

        if (_timer >= 1f)
        {
            _timer = 0;
            return NodeState.Success;
        }

        return NodeState.Running;
    }
}

public class Leaf_BackMove : BtNode
{
    private static readonly int Vertical = Animator.StringToHash("Vertical");
    private Transform _self;
    private Transform _target;
    private Animator _animator;
    private NavMeshAgent _agent;

    private float _timer;
    private bool _isStarted;

    public Leaf_BackMove(Animator animator, NavMeshAgent agent, Transform self, Transform target)
    {
        _animator = animator;
        _agent = agent;
        _self = self;
        _target = target;
    }

    public override NodeState Evaluate()
    {
        if (_isStarted == false)
        {
            _isStarted = true;
            _agent.updateRotation = false;
        }

        _timer += Time.deltaTime;
        _animator.SetFloat(Vertical, -1f, 0.12f, Time.deltaTime);

        Vector3 lookPos = _target.position;
        lookPos.y = _self.position.y;
        _self.LookAt(lookPos);

        // 이동 방향 계산
        Vector3 targetDir = (_target.position - _self.position).normalized;
        targetDir.y = 0;
        // 이동 실행
        _agent.Move(-targetDir * (_agent.speed * Time.deltaTime));

        if (_timer >= 2f)
        {
            _animator.SetFloat(Vertical, 0f);
            _isStarted = false;
            _agent.updateRotation = true;
            _timer = 0f;
            return NodeState.Success;
        }

        return NodeState.Running;
    }
}

public class Leaf_PerformAttack : BtNode
{
    private readonly Transform _self;
    private readonly Transform _target;
    private readonly BossMonsterAI _bossMonster;
    private readonly Animator _animator;
    private readonly AttackData _attackData;
    private RootMotionHandler _rootHandler;

    private bool _useRoot;
    private float _timer;
    private bool _isAttacking;
    private bool _isDamaged;

    public Leaf_PerformAttack
    (Transform self, Transform target, BossMonsterAI bossMonster, Animator animator, AttackData data, bool useRoot,
        RootMotionHandler rootHandler)
    {
        _self = self;
        _target = target;
        _bossMonster = bossMonster;
        _animator = animator;
        _attackData = data;
        _useRoot = useRoot;
        _rootHandler = rootHandler;
    }

    public override NodeState Evaluate()
    {
        // 1. 공격 시작 (진입)
        if (_isAttacking == false)
        {
            _isAttacking = true;
            _isDamaged = false;
            _timer = 0f;
        }

        if (_useRoot)
        {
            _animator.applyRootMotion = true;
        }

        _timer += Time.deltaTime;

        // 2. 선딜레이 구간 (Wind-up)
        if (_timer < _attackData.windupTime)
        {
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

            if (_useRoot)
            {
                Vector3 pos = _self.position + _rootHandler.DeltaPos;

                pos.y = 0f;
                _self.position = pos;
                _self.rotation *= _rootHandler.DeltaRot;
            }
            else
            {
                // 루트 모션을 사용하지 않는 공격일 때만 수동으로 전진
                float s = Mathf.Lerp(3f, 0, Time.deltaTime * 3f);
                Vector3 moveVec = _self.forward * (s * Time.deltaTime);
                _self.position += moveVec;
            }

            return NodeState.Running;
        }


        // 4. 후딜레이 구간 (Recovery)
        if (_timer < _attackData.windupTime + _attackData.activeTime + _attackData.recoveryTime)
        {
            return NodeState.Running;
        }

        // 5. 종료 (모든 시간이 지남)
        _isAttacking = false; // 상태 초기화
        if (_useRoot)
        {
            _animator.applyRootMotion = false;
        }
        // 루트 모션을 사용했었다면, 상태를 반드시 원래대로 되돌려서 다음 행동(추적 등)이 정상 작동하게 함

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