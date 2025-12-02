using System;
using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class BoneFire : MonoBehaviour
{
    [SerializeField] private GameObject fireEffect;
    [SerializeField] private Image fadeImage;
    [SerializeField] private float fadeDuration = 1.5f;
    [SerializeField] private float waitTime = 0.5f;
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

    public void BoneFireLit(bool isPressed)
    {
        if (_isPlayed) return;

        _isPlayed = true;
        fireEffect.SetActive(true);
        StartCoroutine(FadeSequence());

        HealthEvent e = new HealthEvent()
        {
            HealAmount = 1000,
            Receiver = _player.FighterView
        };
        CombatSystem.Instance.AddInGameEvent(e);

        foreach (var enemy in _allEnemies)
        {
            enemy.Respawn();
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if ((_playerLayer & (1 << other.gameObject.layer)) != 0)
        {
            Debug.Log(other.gameObject.name);
            _player.OnRInput += BoneFireLit;
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if ((_playerLayer & (1 << other.gameObject.layer)) != 0)
        {
            Debug.Log("out");
            _player.OnRInput -= BoneFireLit;
        }
    }

    private IEnumerator FadeSequence()
    {
        yield return new WaitForSeconds(waitTime);
        yield return Fade(true);
        yield return new WaitForSeconds(waitTime);
        yield return Fade(false);
        yield return new WaitForSeconds(fadeDuration);
        _isPlayed = false;
    }

    private IEnumerator Fade(bool isout)
    {
        float timer = 0f;

        Color color = fadeImage.color;

        while (timer < fadeDuration)
        {
            timer += Time.deltaTime;
            float t = timer / fadeDuration;

            if (isout)
            {
                color.a = Mathf.Lerp(0f, 1f, t); // 점점 어둡게
            }
            else
            {
                color.a = Mathf.Lerp(1f, 0f, t);
            }

            fadeImage.color = color;

            yield return null;
        }

        if (isout)
        {
            color.a = 1f;
        }
        else
        {
            color.a = 0f;
        }

        fadeImage.color = color;
    }
}