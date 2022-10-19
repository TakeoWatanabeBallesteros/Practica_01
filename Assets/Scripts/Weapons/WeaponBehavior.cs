using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

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
        _controls.Player.Shoot.performed += Shoot;
        _controls.Player.Reload.performed += StartReload;
    }

    private void OnDisable()
    {
        weaponData.reloading = false;
        _controls.Player.Shoot.performed -= Shoot;
        _controls.Player.Reload.performed -= StartReload;
    }

    public void StartReload(InputAction.CallbackContext ctx) {
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

    //Check which type if gun
    private void Shoot(InputAction.CallbackContext ctx) {
        if (weaponData.currentAmmo > 0) {
            if (CanShoot()) {
                //Instantiate bullet

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
