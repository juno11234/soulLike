using System;
using UnityEngine;

public class Test : MonoBehaviour
{
    private Collider _collider;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
    }

    // Update is called once per frame
    void Update()
    {
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            var e = new CombatEvent();
            e.Receiver = other.gameObject.GetComponent<FighterView>();
            e.Collider = other;
            e.Damage = 20;

            CombatSystem.Instance.AddInGameEvent(e);
        }
    }
}