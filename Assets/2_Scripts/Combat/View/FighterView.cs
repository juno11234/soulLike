using System;
using UnityEngine;
using UnityEngine.UI;


public class FighterView : MonoBehaviour, IFighter
{
    [SerializeField] private FighterStats stats;
    [SerializeField] private Slider hpBar; // 예시: HP 바
    [SerializeField] private Slider staminaBar; // 예시: HP 바
    [SerializeField] private Collider modelCollider;
    public FighterViewModel ViewModel { get; private set; }

    public bool Invincible { get; set; }

    public event Action<int> OnTakeDamage;
    public event Action OnStaminaZero;
    public event Action OnDied;
    
    // IFighter 구현
    public Collider mainModelCollider => modelCollider;
    public GameObject GameObject => gameObject;

    private void Awake()
    {
        // 1. Model을 기반으로 ViewModel을 생성합니다.
        ViewModel = new FighterViewModel(stats);

        // 2. ViewModel의 데이터 변경을 View에 반영하도록 구독(Subscribe)합니다.
        ViewModel.CurrentHealth.Subscribe(UpdateHpBar);
        ViewModel.CurrentStamina.Subscribe(UpdateStaminaBar);
        ViewModel.IsDead.Subscribe(OnDeathStateChanged);

        ViewModel.OnTakeDamage+=OnTakeDamageInvoke;
        ViewModel.OnDied += OnDiedInvoke;
        ViewModel.OnStaminaZero += OnStaminaZeroInvoke;
    }

    private void UpdateHpBar(int newHealth)
    {
        hpBar.value = (float)newHealth / stats.MaxHealth;
    }

    private void UpdateStaminaBar(float newStamina)
    {
        staminaBar.value = newStamina / stats.MaxStamina;
    }

    private void OnDeathStateChanged(bool isDead)
    {
        
    }

    private void OnTakeDamageInvoke(int damage)
    {
        OnTakeDamage?.Invoke(damage);
    }

    private void OnStaminaZeroInvoke()
    {
        OnStaminaZero?.Invoke();
    }

    private void OnDiedInvoke()
    {
        OnDied?.Invoke();
    }
    // 기존 IFighter 인터페이스와의 호환성을 위한 메서드들
    // 이제 로직은 ViewModel에 위임합니다.
    public void TakeDamage(CombatEvent combatEvent)
    {
        if (Invincible) return;
        // 이벤트 시스템을 통해 공격받는 경우
        ViewModel.TakeDamage(combatEvent.Damage);
    }

    public void StaminaChange(float stamina)
    {
        ViewModel.StaminaChange(stamina);
    }

    public void TakeHeal(HealthEvent healEvent)
    {
        ViewModel.TakeHeal(healEvent.HealAmount);
    }
}