using System;
using UnityEngine;

public class EnemyState : MonoBehaviour, IAttackAble
{
    [SerializeField] private bool canRespawn = true;
    public AttackData[] attackData;
    protected FighterView fighterView;
    protected EnemyAIBase enemyAI;

    private Animator _animator;
    private static readonly int Die = Animator.StringToHash("Die");
    private static readonly int Respawn1 = Animator.StringToHash("Blend Tree");
    private EnemyWeapon _weapon;
    private Vector3 _originTransform;
    private Quaternion _originRotation;
    private bool _isDead = false;

    private LockOnTarget _lockOnTarget;

    void Start()
    {
        fighterView = GetComponent<FighterView>();
        enemyAI = GetComponent<EnemyAIBase>();
        _animator = GetComponentInChildren<Animator>();
        _weapon = GetComponentInChildren<EnemyWeapon>();
        _lockOnTarget = GetComponentInChildren<LockOnTarget>();
        _originTransform = transform.position;
        _originRotation = transform.rotation;
        attackData = enemyAI.attackData;
        Initialized();

        fighterView.OnDied += Dead;
    }

    private void Update()
    {
        if (_isDead)
        {
            var stateInfo = _animator.GetCurrentAnimatorStateInfo(0);
            if (stateInfo.shortNameHash == Die && stateInfo.normalizedTime >= 0.9f)
            {
                gameObject.SetActive(false);
            }
        }
    }

    protected virtual void Dead()
    {
        enemyAI.Dead();
        _lockOnTarget.gameObject.SetActive(false);
        PlayTargetAniClip(Die, 0f);
        _isDead = true;
    }

    public void Respawn()
    {
        if (canRespawn == false || _isDead == false) return;
        gameObject.SetActive(true);
        transform.position = _originTransform;
        transform.rotation = _originRotation;
        PlayTargetAniClip(Respawn1, 0f);
        fighterView.Respawn();
        enemyAI.Respawn();
        _isDead = false;
        _lockOnTarget.gameObject.SetActive(true);
    }

    protected virtual void Initialized()
    {
    }

    private void PlayTargetAniClip(int hash, float transition)
    {
        _animator.CrossFade(hash, transition);
    }

    public void AttackForCollEnable()
    {
        _weapon.ActiveCollider(attackData[0].damage);
    }

    public void AttackForCollDisEnable()
    {
        _weapon.DisableCollider();
    }
}