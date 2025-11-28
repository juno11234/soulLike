using UnityEngine;

public class EnemyState : MonoBehaviour, IAttackAble
{
    private FighterView _fighterView;
    private Animator _animator;
    private static readonly int Die = Animator.StringToHash("Die");
    private EnemyAIBase _enemyAI;
    private EnemyWeapon _weapon;
    public AttackData[] attackData;
    private LockOnTarget _lockOnTarget;

    void Start()
    {
        _fighterView = GetComponent<FighterView>();
        _enemyAI = GetComponent<EnemyAIBase>();
        _animator = GetComponentInChildren<Animator>();
        _weapon = GetComponentInChildren<EnemyWeapon>();
        _lockOnTarget = GetComponentInChildren<LockOnTarget>();

        _fighterView.OnDied += Dead;
    }

    protected virtual void Dead()
    {
        _enemyAI.Dead();
        _lockOnTarget.gameObject.SetActive(false); 
        PlayTargetAniClip(Die, 0f);
    }

    private void PlayTargetAniClip(int hash, float transition)
    {
        _animator.CrossFade(hash, transition);
    }

    public void AttackForCollEnable()
    {
        _weapon.ActiveCollider(10);
    }

    public void AttackForCollDisEnable()
    {
        _weapon.DisableCollider();
    }
}