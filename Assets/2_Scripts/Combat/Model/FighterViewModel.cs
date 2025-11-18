using System;

public class FighterViewModel
{
    private readonly FighterStats _stats;

    // View가 구독할 수 있는 데이터
    public readonly Observable<int> CurrentHealth;
    public readonly Observable<float> CurrentStamina;
    public readonly Observable<bool> IsDead;

    // View에게 특정 액션을 알리는 이벤트
    public event Action<int> OnTakeDamage;
    public event Action OnStaminaZero;
    public event Action OnDied;

    public FighterViewModel(FighterStats stats)
    {
        _stats = stats;
        CurrentHealth = new Observable<int>(_stats.MaxHealth);
        CurrentStamina = new Observable<float>(_stats.MaxStamina);
        IsDead = new Observable<bool>(false);
    }

    // View로부터 호출될 명령(메서드)
    public void TakeDamage(int damage)
    {
        if (IsDead.Value) return;

        CurrentHealth.Value -= damage;

        if (CurrentHealth.Value <= 0)
        {
            CurrentHealth.Value = 0;
            IsDead.Value = true;
            OnDied?.Invoke();
        }
        else
        {
            OnTakeDamage?.Invoke(damage);
        }
    }

    public void StaminaChange(float value)
    {
        if (IsDead.Value) return;
        CurrentStamina.Value += value;

        if (CurrentStamina.Value <= 0)
        {
            CurrentStamina.Value = 0;
            OnStaminaZero?.Invoke();
        }

        if (CurrentStamina.Value >= _stats.MaxStamina)
        {
            CurrentStamina.Value = _stats.MaxStamina;
        }
    }

    public void TakeHeal(int heal)
    {
        if (IsDead.Value) return;

        CurrentHealth.Value += heal;

        if (CurrentHealth.Value > _stats.MaxHealth)
        {
            CurrentHealth.Value = _stats.MaxHealth;
        }
    }
}