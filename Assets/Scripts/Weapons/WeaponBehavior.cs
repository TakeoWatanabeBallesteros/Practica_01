using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using UnityEngine;
using UnityEngine.InputSystem;
using Debug = UnityEngine.Debug;

public class WeaponBehavior : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private WeaponData weaponData;
    [SerializeField] private Camera cam;
    [SerializeField] private Transform muzzle;
    
    private PlayerControls _controls;
    private float timeSinceLastShot;
    private bool aiming = false;
    private bool shooting = false;

    private void Awake()
    {
        _controls = PlayerInputs.Controls;
    }

    private void OnEnable() {
        _controls.Player.Shoot.performed += Shoot;
        _controls.Player.Shoot.canceled += StopShooting;
        _controls.Player.Reload.performed += StartReload;
        _controls.Player.Aim.performed += Aim;
        _controls.Player.Aim.canceled += Aim;
    }

    private void OnDisable()
    {
        weaponData.reloading = false;
        _controls.Player.Shoot.performed -= Shoot;
        _controls.Player.Shoot.canceled -= StopShooting;
        _controls.Player.Reload.performed -= StartReload;
        _controls.Player.Aim.performed -= Aim; 
        _controls.Player.Aim.canceled -= Aim;
    }

    private void Update() {
        timeSinceLastShot += Time.deltaTime;
        Debug.DrawRay(muzzle.transform.position, transform.forward*1000);
        Debug.DrawRay(cam.transform.position, cam.transform.forward*1000);
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
                switch (weaponData.type)
                {
                    case TypeOfWeapon.Rifle:
                        break;
                    case TypeOfWeapon.Smg:
                        break;
                    case TypeOfWeapon.Pistol:
                        if(shooting) return;
                        break;
                    case TypeOfWeapon.Sniper:
                        if(shooting) return;
                        break;
                }
                //Instantiate bullet
                GameObject bullet = new GameObject("Bullet");
                BulletBehavior b = bullet.AddComponent<BulletBehavior>();
                b.damage = weaponData.damage;
                b.velocity = weaponData.velocity;
                Instantiate(bullet, cam.transform.position, cam.transform.rotation);
                weaponData.currentAmmo--;
                timeSinceLastShot = 0;
                shooting = true;
                OnGunShot();
            }
        }
    }

    private void StopShooting(InputAction.CallbackContext ctx)
    {
        shooting = false;
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
