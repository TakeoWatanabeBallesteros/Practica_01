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
    
    void WeaponChanged(int currentMagAmmo, int maxMagAmmo, int currentAmmo, string name, Sprite logo)
    {
        _currentMagAmmo = currentMagAmmo;
        _currentAmmo = currentAmmo;
        ammoDisplayText.text = _currentMagAmmo + " / " + _currentAmmo;
        nameDisplay.text = name;
        logoDisplay.sprite = logo;
        ammoDisplayImage.fillAmount = (float)currentMagAmmo/maxMagAmmo;
    }

    void WeaponShoot(int currentMagAmmo, int maxMagAmmo)
    {
        _currentMagAmmo = currentMagAmmo;
        ammoDisplayText.text = _currentMagAmmo + " / " + _currentAmmo;
        ammoDisplayImage.fillAmount = (float)currentMagAmmo/maxMagAmmo;
    }
    
    void WeaponReload(int currentMagAmmo, int currentAmmo)
    {
        _currentMagAmmo = currentMagAmmo;
        _currentAmmo = currentAmmo;
        ammoDisplayText.text = _currentMagAmmo + " / " + _currentAmmo;
        ammoDisplayImage.fillAmount = 1f;
    }
}
