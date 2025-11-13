using System;

public class FighterViewModel
{
    private readonly FighterStats _stats;
    // View가 구독할 수 있는 데이터
    public readonly Observable<int> CurrentHealth;
    public readonly Observable<bool> IsDead;

    // View에게 특정 액션을 알리는 이벤트
    public event Action<int> OnDamaged;
    public event Action OnDied;
    
    private int weaponDamage = 10;
    public FighterViewModel(FighterStats stats)
    {
        _stats = stats;
        CurrentHealth = new Observable<int>(_stats.MaxHealth);
        IsDead = new Observable<bool>(false);
    }

    // View로부터 호출될 명령(메서드)
    public void Attack(FighterViewModel target)
    {
        if (IsDead.Value || target.IsDead.Value) return;

        int damage = CombatCalculator.CalculateDamage(weaponDamage, target._stats);
        target.TakeDamage(damage);
    }

    public void TakeDamage(int damage)
    {
        if (IsDead.Value) return;

        CurrentHealth.Value -= damage;
        OnDamaged?.Invoke(damage);

        if (CurrentHealth.Value <= 0)
        {
            CurrentHealth.Value = 0;
            IsDead.Value = true;
            OnDied?.Invoke();
        }
    }
}