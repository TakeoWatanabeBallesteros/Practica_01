using System;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class WeaponUIBehavior : MonoBehaviour
{
    [SerializeField] TextMeshProUGUI ammoDisplayText;
    [SerializeField] Image ammoDisplayImage;
    [SerializeField] Image logoDisplay;
    [SerializeField] TextMeshProUGUI nameDisplay;

    private int _currentMagAmmo;

    private int _currentAmmo;
    private void OnEnable()
    {
        WeaponSwitching.OnWeaponSwitch += WeaponChanged;
        WeaponBehavior.OnWeaponShoot += WeaponShoot;
        WeaponBehavior.OnWeaponReload += WeaponReload;
    }
    
    void WeaponChanged(int currentMagAmmo, int currentAmmo, WeaponData data)
    {
        _currentMagAmmo = currentMagAmmo;
        _currentAmmo = currentAmmo;
        ammoDisplayText.text = _currentMagAmmo + " / " + _currentAmmo;
        nameDisplay.text = data.name;
        logoDisplay.sprite = data.logo;
        ammoDisplayImage.fillAmount = (float)currentMagAmmo/data.magSize;
    }

    void WeaponShoot(int currentMagAmmo, WeaponData data)
    {
        _currentMagAmmo = currentMagAmmo;
        ammoDisplayText.text = _currentMagAmmo + " / " + _currentAmmo;
        ammoDisplayImage.fillAmount = (float)currentMagAmmo/data.magSize;
    }
    
    void WeaponReload(int currentMagAmmo, int currentAmmo, WeaponData data)
    {
        _currentMagAmmo = currentMagAmmo;
        _currentAmmo = currentAmmo;
        ammoDisplayText.text = _currentMagAmmo + " / " + _currentAmmo;
        ammoDisplayImage.fillAmount = (float)currentMagAmmo/data.magSize;
    }
}
