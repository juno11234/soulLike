using System;
using UnityEngine;

public class PlayerInventory : MonoBehaviour
{
    private WeaponSlotManager _weaponSlotManager;
    public WeaponItem rightWeapon;
    public WeaponItem leftWeapon;

    private void Awake()
    {
        _weaponSlotManager = GetComponent<WeaponSlotManager>();
    }

    private void Start()
    {
        _weaponSlotManager.LoadWeaponOnSlot(rightWeapon, false);
        _weaponSlotManager.LoadWeaponOnSlot(leftWeapon, true);
    }
}