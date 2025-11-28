using System.Collections.Generic;
using UnityEngine;

public class EnemyWeapon : MonoBehaviour
{
    private LayerMask _playerLayer;
    private Collider _collider;
    private HashSet<FighterView> _damaged = new HashSet<FighterView>();

    private int _damage;
    private void Start()
    {
        _collider = GetComponent<Collider>();
        _collider.enabled = false;
        _playerLayer = 1<<7;
    }

    public void ActiveCollider(int damage)
    {
        _collider.enabled = true;
        
        _damage=damage;
    }

    public void DisableCollider()
    {
        _collider.enabled = false;
        _damaged.Clear();
    }

    void OnTriggerEnter(Collider other)
    {
        if ((_playerLayer & (1 << other.gameObject.layer)) != 0)
        {
            var player = CombatSystem.Instance.GetFighter(other);
            if (player != null && _damaged.Contains(player) == false)
            {
                CombatEvent e = new CombatEvent()
                {
                    Receiver = player,
                    HitPosition = other.ClosestPoint(transform.position),
                    Collider = other,
                    Damage = _damage,
                };

                CombatSystem.Instance.AddInGameEvent(e);
                _damaged.Add(player);
            }
        }
    }
}