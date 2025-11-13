using UnityEngine;
using UnityEngine.UI;

public class FighterView : MonoBehaviour, IFighter
{
    [Header("Model")]
    [SerializeField] private FighterStats stats;

    [Header("View Components")]
    [SerializeField] private Animator animator;
    [SerializeField] private Slider hpBar; // 예시: HP 바
    
    public FighterViewModel ViewModel { get; private set; }

    // IFighter 구현
    public Collider MainCollider => GetComponent<Collider>();
    public GameObject GameObject => gameObject;


    private void Awake()
    {
        // 1. Model을 기반으로 ViewModel을 생성합니다.
        ViewModel = new FighterViewModel(stats);

        // 2. ViewModel의 데이터 변경을 View에 반영하도록 구독(Subscribe)합니다.
        ViewModel.CurrentHealth.Subscribe(UpdateHpBar);
        ViewModel.IsDead.Subscribe(OnDeathStateChanged);

        // 3. ViewModel의 이벤트를 구독하여 애니메이션, 이펙트 등을 처리합니다.
        ViewModel.OnDamaged += (damage) => animator.SetTrigger("Hit");
        ViewModel.OnDied += () => animator.SetTrigger("Die");
    }

    private void UpdateHpBar(int newHealth)
    {
        if (hpBar != null)
        {
            hpBar.value = (float)newHealth / stats.MaxHealth;
        }
        Debug.Log($"{gameObject.name}의 체력 변경: {newHealth}");
    }

    private void OnDeathStateChanged(bool isDead)
    {
        if (isDead)
        {
            Debug.Log($"{gameObject.name}가 사망했습니다.");
            // TODO: 추가적인 사망 처리 (예: 충돌 비활성화, AI 중지 등)
        }
    }
    
    // 기존 IFighter 인터페이스와의 호환성을 위한 메서드들
    // 이제 로직은 ViewModel에 위임합니다.
    public void TakeDamage(CombatEvent combatEvent)
    {
        // 이벤트 시스템을 통해 공격받는 경우
        var attackerView = combatEvent.Sender as FighterView;
        if (attackerView != null)
        {
            attackerView.ViewModel.Attack(this.ViewModel);
        }
    }

    public void TakeHeal(CombatEvent combatEvent)
    {
        // TODO: 힐 로직을 ViewModel에 구현해야 합니다.
    }
}
