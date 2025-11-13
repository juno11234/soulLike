using UnityEngine;

public interface IFighter 
{
  public Collider MainCollider { get; }
  public GameObject GameObject { get; }
  public void TakeDamage(CombatEvent combatEvent);
  public void TakeHeal(CombatEvent combatEvent);
}
