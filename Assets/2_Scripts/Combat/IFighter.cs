using UnityEngine;

public interface IFighter 
{
  public Collider mainModelCollider { get; }
  public GameObject GameObject { get; }
  public void TakeDamage(CombatEvent combatEvent);
  public void TakeHeal(HealthEvent healEvent);
  
}
