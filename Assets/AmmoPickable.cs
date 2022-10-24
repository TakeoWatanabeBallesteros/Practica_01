using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AmmoPickable : MonoBehaviour, IPickable
{
    [SerializeField] int ammo;
    public void Pick()
    {
        if(WeaponSwitching.currentWeapon.CanGetAmmo())
        {
            WeaponSwitching.currentWeapon.GetAmmo(ammo);
            Destroy(this.gameObject);
        }
    }
}
