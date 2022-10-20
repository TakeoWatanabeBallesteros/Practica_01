using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using Unity.Mathematics;
using UnityEngine;
using UnityEngine.InputSystem;
using Debug = UnityEngine.Debug;

public class WeaponBehavior : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private WeaponData weaponData;

    [SerializeField] private RecoilBehavior recoilBehavior;
    [SerializeField] private Camera cam;
    [SerializeField] private Transform muzzle;
    [SerializeField] private GameObject decal;
    [SerializeField] private Animator animator;
    [SerializeField] private float idleFov;
    [SerializeField] private float aimFov;
    [SerializeField] private float smooth;
    [SerializeField] private int currentAmmo;
    [SerializeField] private int currentMagAmmo;

    private PlayerControls _controls;
    private float timeSinceLastShot;
    private bool aiming = false;
    private bool shooting = false;
    private int AimingID;
    private int ShootingID;

    private void Awake()
    {
        _controls = PlayerInputs.Controls;
        AimingID = Animator.StringToHash("Aim");
        ShootingID = Animator.StringToHash("Shooting");
        currentAmmo = weaponData.maxAmmo;
        currentMagAmmo = weaponData.magSize;
    }

    private void OnEnable() {
        _controls.Player.Shoot.performed += StartShooting;
        _controls.Player.Shoot.canceled += StopShooting;
        _controls.Player.Reload.performed += StartReload;
        _controls.Player.Aim.performed += Aim;
        _controls.Player.Aim.canceled += Aim;
    }

    private void OnDisable()
    {
        weaponData.reloading = false;
        _controls.Player.Shoot.performed -= StartShooting;
        _controls.Player.Shoot.canceled -= StopShooting;
        _controls.Player.Reload.performed -= StartReload;
        _controls.Player.Aim.performed -= Aim; 
        _controls.Player.Aim.canceled -= Aim;
    }

    private void Update() {
        if(shooting) Shoot();
        timeSinceLastShot += Time.deltaTime;
        Debug.DrawRay(muzzle.transform.position, transform.forward*1000);
        Debug.DrawRay(cam.transform.position, cam.transform.forward*1000);
        Aiming();
    }

    public void StartReload(InputAction.CallbackContext ctx) {
        if (!weaponData.reloading && this.gameObject.activeSelf && currentAmmo > 0)
            StartCoroutine(Reload());
    }

    private IEnumerator Reload() {
        weaponData.reloading = true;

        yield return new WaitForSeconds(weaponData.reloadTime);

        currentMagAmmo = currentAmmo < weaponData.magSize ? currentAmmo : weaponData.magSize;
        currentAmmo -= weaponData.magSize;
        math.clamp(currentAmmo, 0, weaponData.maxAmmo);
        
        weaponData.reloading = false;
    }

    private bool CanShoot() => !weaponData.reloading && timeSinceLastShot > 1f / (weaponData.fireRate / 60f) && currentMagAmmo > 0;

    //Check which type if gun
    private void Shoot() {
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
                    //if(shooting) return;
                    break;
            }
            //Instantiate bullet
            GameObject bullet = new GameObject("Bullet");
            BulletBehavior b = bullet.AddComponent<BulletBehavior>();
            b.damage = weaponData.damage;
            b.velocity = weaponData.velocity;
            b.decal = decal;
            Instantiate(bullet, cam.transform.position, cam.transform.rotation);
            currentMagAmmo--;
            timeSinceLastShot = 0;
            OnGunShot();
        }
        else if (!weaponData.reloading && this.gameObject.activeSelf && currentAmmo > 0 && currentMagAmmo == 0)
        {
            StartCoroutine(Reload());
            shooting = false;
            animator.SetBool(ShootingID, shooting);
        }
        
    }

    private void StartShooting(InputAction.CallbackContext ctx)
    {
        shooting = true;
        animator.SetBool(ShootingID, shooting);
    }

    private void StopShooting(InputAction.CallbackContext ctx)
    {
        shooting = false;
        animator.SetBool(ShootingID, shooting);
    }

    private void OnGunShot()
    {
        if(!shooting)recoilBehavior.RecoilFire(weaponData.recoilX, weaponData.recoilY, weaponData.recoilZ);
        else recoilBehavior.RecoilFire(weaponData.recoilAimX, weaponData.recoilAimY, weaponData.recoilAimZ);
    }

    private void Aim(InputAction.CallbackContext ctx)
    {
        aiming = !aiming;
        animator.SetBool(AimingID, aiming);
    }

    private void Aiming()
    {
        if (weaponData.type == TypeOfWeapon.Sniper)return;
        if (!aiming)
        {
            cam.fieldOfView = Mathf.Lerp(cam.fieldOfView, idleFov, smooth);
        }
        else
        {
            cam.fieldOfView = Mathf.Lerp(cam.fieldOfView, aimFov, smooth);
        }
    }
}
