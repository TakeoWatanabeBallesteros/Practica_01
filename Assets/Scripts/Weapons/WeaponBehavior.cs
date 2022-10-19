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
    private float timeSinceLastShot;
    private bool aiming = false;

    private void Awake()
    {
        _controls = PlayerInputs.Controls;
    }

    private void OnEnable() {
        _controls.Player.Shoot.performed += Shoot;
        _controls.Player.Reload.performed += StartReload;
        _controls.Player.Aim.performed += Aim;
        _controls.Player.Aim.canceled += Aim;
    }

    private void OnDisable()
    {
        weaponData.reloading = false;
        _controls.Player.Shoot.performed -= Shoot;
        _controls.Player.Reload.performed -= StartReload;
        _controls.Player.Aim.performed -= Aim; 
        _controls.Player.Aim.canceled -= Aim;
    }

    private void Update() {
        timeSinceLastShot += Time.deltaTime;

        Debug.DrawRay(cam.position, cam.forward*1000);
        Aiming();
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

    private void OnGunShot() {  }

    private void Aim(InputAction.CallbackContext ctx) {aiming = !aiming; }

    private void Aiming()
    {
        /*if (!aiming)
        {
            weaponPosition.position = Vector3.Lerp(weaponPosition.position, weaponIdlePosition.position, .1f);
            gunCamera.fieldOfView = Mathf.Lerp(gunCamera.fieldOfView, 75, .1f);
            mainCamera.fieldOfView = Mathf.Lerp(mainCamera.fieldOfView, 60, .1f);
        }
        else
        {
            weaponPosition.position = Vector3.Lerp(weaponPosition.position, weaponAimPosition.position, .1f);
            gunCamera.fieldOfView = Mathf.Lerp(gunCamera.fieldOfView, 20, .1f);
            mainCamera.fieldOfView = Mathf.Lerp(mainCamera.fieldOfView, 20, .1f);
            _animator.SetFloat(_animIDMotionSpeed, 0);
        }*/
    }
}
