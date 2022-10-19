using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WeaponBehavior : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private WeaponData weaponData;
    [SerializeField] private Transform cam;
    
    private PlayerControls _controls;
    float timeSinceLastShot;

    private void Awake()
    {
        _controls = PlayerInputs.Controls;
    }

    private void OnEnable() {
        //_controls.shootInput += Shoot;
        //_controls.reloadInput += StartReload;
    }

    private void OnDisable()
    {
        weaponData.reloading = false;
    }

    public void StartReload() {
        if (!weaponData.reloading && this.gameObject.activeSelf)
            StartCoroutine(Reload());
    }

    private IEnumerator Reload() {
        weaponData.reloading = true;

        yield return new WaitForSeconds(weaponData.reloadTime);

        weaponData.currentAmmo = weaponData.magSize;

        weaponData.reloading = false;
    }

    private bool CanShoot() => !weaponData.reloading && timeSinceLastShot > 1f / (weaponData.fireRate / 60f);

    private void Shoot() {
        if (weaponData.currentAmmo > 0) {
            if (CanShoot()) {
                

                weaponData.currentAmmo--;
                timeSinceLastShot = 0;
                OnGunShot();
            }
        }
    }

    private void Update() {
        timeSinceLastShot += Time.deltaTime;

        Debug.DrawRay(cam.position, cam.forward);
    }

    private void OnGunShot() {  }
}
