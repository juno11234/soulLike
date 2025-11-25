using System;
using UnityEngine;

public enum WeaponRank
{
    S,
    A,
    B,
}

[CreateAssetMenu(menuName = "Item/Weapon")]
public class WeaponItem : Item
{
    public GameObject prefab;
    public bool isUnarmed;

    public int damage;
    public float staminaConsume;

    public WeaponRank strength;
    public WeaponRank dexterity;
}