using System;
using System.Collections.Generic;
using UnityEngine;

public class CombatSystem : MonoBehaviour
{
    private const int Max_Event_Count = 10;

    public class Callback
    {
        public Action<CombatEvent> OnCombatEvent;
    }

    public static CombatSystem Instance;

    private Dictionary<Collider, IFighter> monsterDict = new Dictionary<Collider, IFighter>();
    private Queue<InGameEvent> eventQueue = new Queue<InGameEvent>();
    public readonly Callback Events = new Callback();

    private void Awake()
    {
        Instance = this;
    }

    private void Update()
    {
        int processCount = 0;

        while (eventQueue.Count > 0 && processCount < Max_Event_Count)
        {
            var inGameEvent = eventQueue.Dequeue();
            switch (inGameEvent.Type)
            {
                case InGameEvent.EventType.Combat:
                    var combatEvent = inGameEvent as CombatEvent;
                    inGameEvent.Receiver.TakeDamage(combatEvent);
                    break;
                case InGameEvent.EventType.Heal:
                    var healEvent = inGameEvent as HealthEvent;
                    inGameEvent.Receiver.TakeHeal(healEvent);
                    break;
            }

            processCount++;
        }
    }

    public void RegisterMonster(IFighter monster)
    {
        if (monsterDict.TryAdd(monster.mainModelCollider, monster) == false)
        {
            Debug.Log("몬스터가 이미존재 덮어씀");
        }
    }

    public IFighter GetMonster(Collider coll)
    {
        return monsterDict[coll];
    }

    public void RemoveMonster(Collider coll)
    {
        monsterDict.Remove(coll);
    }

    public void AddInGameEvent(InGameEvent e)
    {
        eventQueue.Enqueue(e);
    }
}