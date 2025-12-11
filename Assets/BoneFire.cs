using System;
using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class BoneFire : MonoBehaviour
{
    [SerializeField] private GameObject fireEffect;
    [SerializeField] private Image fadeImage;
    private EnemyState[] _allEnemies;
    private PlayerStateMachine _player;
    private int _playerLayer;
    private bool _isPlayed;

    // Update is called once per frame
    private void Start()
    {
        _allEnemies = FindObjectsByType<EnemyState>
            (FindObjectsInactive.Include, FindObjectsSortMode.None);
        _player = FindAnyObjectByType<PlayerStateMachine>();
        fireEffect.SetActive(false);
        _playerLayer = 1 << 7;
    }

    private void BoneFireLit(bool isPressed)
    {
        if (_isPlayed) return;

        _isPlayed = true;
        fireEffect.SetActive(true);

        FadeManager.Instance.StartFade(Heal, fadeImage);

        foreach (var enemy in _allEnemies)
        {
            enemy.Respawn();
        }
    }

    private void Heal()
    {
        HealthEvent e = new HealthEvent()
        {
            HealAmount = 1000,
            Receiver = _player.FighterView
        };
        CombatSystem.Instance.AddInGameEvent(e);

        _isPlayed = false;
    }

    private void OnTriggerEnter(Collider other)
    {
        if ((_playerLayer & (1 << other.gameObject.layer)) != 0)
        {
            _player.OnRInput += BoneFireLit;
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if ((_playerLayer & (1 << other.gameObject.layer)) != 0)
        {
            _player.OnRInput -= BoneFireLit;
        }
    }
}