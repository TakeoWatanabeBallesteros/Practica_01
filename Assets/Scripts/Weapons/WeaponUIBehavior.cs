using System;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class WeaponUIBehavior : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI amgAmmAndMag;

    private int _currentMagAmmo;

    private int _currentAmmo;
    private void OnEnable()
    {
        WeaponSwitching.OnWeaponSwitch += WeaponChanged;
        WeaponBehavior.OnWeaponShoot += WeaponShoot;
        WeaponBehavior.OnWeaponReload += WeaponReload;
    }

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
    
    void WeaponChanged(int currentMagAmmo, int currentAmmo)
    {
        _currentMagAmmo = currentMagAmmo;
        _currentAmmo = currentAmmo;
        amgAmmAndMag.text = _currentMagAmmo + " / " + _currentAmmo;
    }

    void WeaponShoot(int currentMagAmmo)
    {
        _currentMagAmmo = currentMagAmmo;
        amgAmmAndMag.text = _currentMagAmmo + " / " + _currentAmmo;
    }
    
    void WeaponReload(int currentMagAmmo, int currentAmmo)
    {
        _currentMagAmmo = currentMagAmmo;
        _currentAmmo = currentAmmo;
        amgAmmAndMag.text = _currentMagAmmo + " / " + _currentAmmo;
    }
}
