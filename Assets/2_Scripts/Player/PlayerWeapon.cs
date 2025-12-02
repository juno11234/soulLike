using System.Collections.Generic;
using UnityEngine;

public class PlayerWeapon : MonoBehaviour
{
    private LayerMask _enemyLayer;
    private Collider _collider;
    private WeaponItem _stat;
    private HashSet<FighterView> _damaged = new HashSet<FighterView>();
    private int _damage;

    void Start()
    {
        _collider = GetComponentInChildren<Collider>();
        _collider.enabled = false;
        _enemyLayer = 1 << 8;
    }

    public void ActiveCollider(int strength, int dexterity)
    {
        _collider.enabled = true;
        int totalDamage = _stat.Damage + (strength * _stat.StrengthBonus() / 2) +
                          (dexterity * _stat.DexterityBonus() / 2);

        _damage = totalDamage;
    }

    public void DisableCollider()
    {
        _collider.enabled = false;
        _damaged.Clear();
    }

    void OnTriggerEnter(Collider other)
    {
        if ((_enemyLayer & (1 << other.gameObject.layer)) != 0)
        {
            var monster = CombatSystem.Instance.GetFighter(other);
            if (monster != null && _damaged.Contains(monster) == false)
            {
                CombatEvent e = new CombatEvent()
                {
                    Receiver = monster,
                    HitPosition = other.ClosestPoint(transform.position),
                    Collider = other,
                    Damage = _damage,
                };

                CombatSystem.Instance.AddInGameEvent(e);
                _damaged.Add(monster);
            }
        }
    }
}