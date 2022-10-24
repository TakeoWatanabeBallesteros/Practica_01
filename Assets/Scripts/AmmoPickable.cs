using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AmmoPickable : MonoBehaviour, IPickable, IReset
{
    [SerializeField] int ammo;
    public bool dontDestroy = false;
    public void Pick()
    {
        if(WeaponSwitching.currentWeapon.CanGetAmmo())
        {
            WeaponSwitching.currentWeapon.GetAmmo(ammo);
            if(dontDestroy) gameObject.SetActive(false);
            else Destroy(this.gameObject);
        }
    }
    public void Reset()
    {
        gameObject.SetActive(true);
    }
}
