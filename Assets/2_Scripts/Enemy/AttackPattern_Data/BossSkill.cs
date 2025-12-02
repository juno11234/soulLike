using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BossSkill : MonoBehaviour
{
    [SerializeField] private AttackData attackData;
    [SerializeField] private float durationTime;
    private HashSet<FighterView> _damaged = new HashSet<FighterView>();
    private LayerMask _playerLayer;

    private void Start()
    {
        _playerLayer = 1 << 7;
        StartCoroutine(DestroyCoroutine());
    }

    private void OnTriggerEnter(Collider other)
    {
        if ((_playerLayer & (1 << other.gameObject.layer)) != 0)
        {
            var player = CombatSystem.Instance.GetFighter(other);
            if (player != null && _damaged.Contains(player) == false)
            {
                CombatEvent e = new CombatEvent()
                {
                    Receiver = player,
                    Collider = other,
                    Damage = attackData.damage,
                };

                CombatSystem.Instance.AddInGameEvent(e);
                _damaged.Add(player);
            }
        }
    }

    IEnumerator DestroyCoroutine()
    {
        yield return new WaitForSeconds(durationTime);
        Destroy(gameObject);
    }
}