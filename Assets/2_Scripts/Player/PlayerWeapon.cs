using UnityEngine;

public class PlayerWeapon : MonoBehaviour
{
    [SerializeField] private LayerMask enemyLayer;
    private Collider _collider;

    void Start()
    {
        _collider = GetComponentInChildren<Collider>();
        _collider.enabled = false;
    }

    public void ActiveCollider()
    {
        _collider.enabled = true;
    }

    void OnTriggerEnter(Collider other)
    {
        if ((enemyLayer & (1 << other.gameObject.layer)) != 0)
        {
            CombatEvent e = new CombatEvent();
            e.Receiver = CombatSystem.Instance.GetFighter(other);
            e.Damage = 10;
            Debug.Log(other.gameObject.name);
            CombatSystem.Instance.AddInGameEvent(e);
        }
    }
}