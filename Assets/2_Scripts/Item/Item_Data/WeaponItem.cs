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
    public GameObject Prefab;

    public int Damage;
    public float StaminaConsume;

    public WeaponRank Strength;
    public WeaponRank Dexterity;

    private void OnValidate()
    {
        Type = ItemType.Weapon;
        MaxAmount = 1;
    }

    public int StrengthBonus()
    {
        switch (Strength)
        {
            case WeaponRank.S:
                return 3;
            case WeaponRank.A:
                return 2;
            case WeaponRank.B:
                return 1;
            default:
                return 0;
        }
    }

    public int DexterityBonus()
    {
        switch (Dexterity)
        {
            case WeaponRank.S:
                return 3;
            case WeaponRank.A:
                return 2;
            case WeaponRank.B:
                return 1;
            default:
                return 0;
        }
    }
}