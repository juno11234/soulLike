using System;
using System.Collections;
using UnityEngine;

public class BossWall : MonoBehaviour
{
    private int _playerLayer;
    private PlayerStateMachine _player;
    [SerializeField] private Transform enterPos;
    [SerializeField] private Collider box;
    [SerializeField] private GameObject boss;

    void Start()
    {
        _player = FindAnyObjectByType<PlayerStateMachine>();
        _playerLayer = 1 << 7;
        boss.SetActive(false);
    }

    private void OnTriggerEnter(Collider other)
    {
        if ((_playerLayer & (1 << other.gameObject.layer)) != 0)
        {
            _player.OnRInput += EnterBossRoom;
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if ((_playerLayer & (1 << other.gameObject.layer)) != 0)
        {
            _player.OnRInput -= EnterBossRoom;
        }
    }

    private void EnterBossRoom(bool isPressed)
    {
        box.enabled = false;
        StartCoroutine(EnterBoss());
    }

    private IEnumerator EnterBoss()
    {
        float speed = 3f;

        // 문이나 벽 collider를 먼저 끈다
        box.enabled = false;
        Vector3 aPos = enterPos.position;
        Vector3 bPos = _player.transform.position;

        aPos.y = 0;
        while (true)
        {
            Vector3 dir = (aPos - bPos).normalized;
            dir.y = 0;
            // Move로 걸어가기
            _player.Controller.Move(dir * (speed * Time.deltaTime));

            // 목표에 도착하면
            if (Vector3.Distance(_player.transform.position, aPos) < 2f)
            {
                boss.SetActive(true);
                break;
            }

            yield return null; // 매 프레임 이동
        }

        box.enabled = true;
    }
}