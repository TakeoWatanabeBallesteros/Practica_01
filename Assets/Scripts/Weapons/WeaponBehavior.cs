using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using Unity.Mathematics;
using UnityEngine;
using UnityEngine.InputSystem;
using Debug = UnityEngine.Debug;

public class WeaponBehavior : MonoBehaviour, IReset
{
    [Header("References")] 
    [SerializeField] private BulletBehavior bullet;
    [SerializeField] private WeaponData weaponData;
    [SerializeField] private RecoilBehavior recoilBehavior;
    [SerializeField] private Camera cam;
    [SerializeField] private Transform muzzle;
    [SerializeField] private PoolableObject decal;
    [SerializeField] private Animator animator;
    [SerializeField] private float idleFov;
    [SerializeField] private float aimFov;
    [SerializeField] private float smooth;
    [SerializeField] private AudioClip shootAudio;
    [SerializeField] public int currentAmmo { get; private set; } 
    public int currentMagAmmo { get; private set; }
    [SerializeField] private LayerMask _layerMask;
    [SerializeField] private TrailRenderer trail;
    [SerializeField] private ParticleSystem ImpactParticleSystem;
    [SerializeField] private ParticleSystem muzzleFlash;


    private PlayerControls _controls;
    private float timeSinceLastShot;
    private bool aiming = false;
    private bool shooting = false;
    private int AimingID;
    private int ShootingID;
    private int onlyOneShoot;

    public delegate void WeaponShoot(int currentMagAmmo, WeaponData dataWeapon);
    public delegate void WeaponReload(int currentMagAmmo, int currentAmmo, WeaponData dataWeapon);
    
    public static event WeaponShoot OnWeaponShoot;
    public static event WeaponReload OnWeaponReload;

    private void Awake()
    {
        _controls = PlayerInputs.Controls;
        AimingID = Animator.StringToHash("Aim");
        ShootingID = Animator.StringToHash("Shooting");
        currentAmmo = weaponData.maxAmmo;
        currentMagAmmo = weaponData.magSize;
    }

    private void OnEnable()
    {
        timeSinceLastShot = 100;
        _controls.Player.Shoot.performed += StartShooting;
        _controls.Player.Shoot.canceled += StopShooting;
        _controls.Player.Reload.performed += StartReload;
        _controls.Player.Aim.performed += Aim;
        _controls.Player.Aim.canceled += StopAim;
    }

    private void OnDisable()
    {
        weaponData.reloading = false;
        _controls.Player.Shoot.performed -= StartShooting;
        _controls.Player.Shoot.canceled -= StopShooting;
        _controls.Player.Reload.performed -= StartReload;
        _controls.Player.Aim.performed -= Aim; 
        _controls.Player.Aim.canceled -= StopAim;
    }

    private void Update() {
        if(shooting) Shoot();
        timeSinceLastShot += Time.deltaTime;
        Aiming();
    }

    public void StartReload(InputAction.CallbackContext ctx) {
        if (!weaponData.reloading && this.gameObject.activeSelf && currentAmmo > 0)
            StartCoroutine(Reload());
    }

    private IEnumerator Reload()
    {
        weaponData.reloading = true;

        yield return new WaitForSeconds(weaponData.reloadTime);

        int bulletsToReload = Mathf.Clamp(weaponData.magSize - currentMagAmmo,0,currentAmmo);
        currentAmmo -= bulletsToReload;
        currentMagAmmo += bulletsToReload;

        OnWeaponReload?.Invoke(currentMagAmmo, currentAmmo, weaponData);
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
                    if(onlyOneShoot == 1) return;
                    break;
                case TypeOfWeapon.Sniper:
                    if(onlyOneShoot == 1) return;
                    break;
            }
            //Instantiate bullet
            RaycastHit hit;
            Vector3 destination =
                Physics.Raycast(cam.transform.position, cam.transform.forward, out hit, float.MaxValue, _layerMask)
                    ? hit.point
                    : cam.transform.forward * 1000;
            var b = Instantiate(bullet, muzzle.position, Quaternion.identity);
            b.damage = weaponData.damage;
            b.velocity = weaponData.velocity;
            b.decal = decal;
            b.destination = destination;
            b.layerMask = _layerMask;
            TrailRenderer _trail = Instantiate(trail, muzzle.position, Quaternion.identity);
            StartCoroutine(SpawnTrail(_trail, hit));
            currentMagAmmo--;
            OnWeaponShoot?.Invoke(currentMagAmmo,weaponData);
            muzzleFlash.Play();
            timeSinceLastShot = 0;
            onlyOneShoot++;
            OnGunShot();
        }
        else if (!weaponData.reloading && this.gameObject.activeSelf && currentAmmo > 0 && currentMagAmmo == 0)
        {
            StartCoroutine(Reload());
            shooting = false;
            animator.SetBool(ShootingID, shooting);
        }
        if(weaponData.type == TypeOfWeapon.Sniper || weaponData.type == TypeOfWeapon.Pistol)
        {
            aiming = false;
            animator.SetBool(AimingID, aiming);
        }
    }

    private void StartShooting(InputAction.CallbackContext ctx)
    {
        shooting = CanShoot();
        animator.SetBool(ShootingID, shooting);
    }

    private void StopShooting(InputAction.CallbackContext ctx)
    {
        shooting = false;
        animator.SetBool(ShootingID, shooting);
        onlyOneShoot = 0;
    }

    private void OnGunShot()
    {
        ShootSound();
        if(!aiming)recoilBehavior.RecoilFire(weaponData.recoilX, weaponData.recoilY, weaponData.recoilZ);
        else recoilBehavior.RecoilFire(weaponData.recoilAimX, weaponData.recoilAimY, weaponData.recoilAimZ);
    }

    private void Aim(InputAction.CallbackContext ctx)
    {
        aiming = weaponData.type != TypeOfWeapon.Pistol && (weaponData.type != TypeOfWeapon.Sniper || CanShoot());
        animator.SetBool(AimingID, aiming);
    }
    private void StopAim(InputAction.CallbackContext ctx)
    {
        aiming = false;
        animator.SetBool(AimingID, aiming);
    }

    private void Aiming()
    {
        if (weaponData.type == TypeOfWeapon.Sniper)return;
        if (!aiming)
        {
            cam.fieldOfView = Mathf.Lerp(cam.fieldOfView, idleFov, smooth*Time.deltaTime);
        }
        else
        {
            cam.fieldOfView = Mathf.Lerp(cam.fieldOfView, aimFov, smooth*Time.deltaTime);
        }
    }
    public WeaponData GetData()
    {
        return weaponData;
    }
    public bool CanGetAmmo()
    {
        return currentAmmo < weaponData.maxAmmo;
    }
    public void GetAmmo(int ammoAmount)
    {
        currentAmmo = Mathf.Clamp(currentAmmo + ammoAmount,0,weaponData.maxAmmo);
        OnWeaponReload?.Invoke(currentMagAmmo, currentAmmo, weaponData);
    }

    public void ShootSound()
    {
        AudioSource.PlayClipAtPoint(shootAudio, muzzle.position);
    }
    public void Reset()
    {
        currentAmmo = weaponData.maxAmmo;
        currentMagAmmo = weaponData.magSize;
        OnWeaponReload?.Invoke(currentMagAmmo, currentAmmo, weaponData);
    }
    
    private IEnumerator SpawnTrail(TrailRenderer Trail, RaycastHit HitPoint)
    {
        Vector3 point;
        point = HitPoint.point;
        if (HitPoint.point == Vector3.zero)
        {
            point = cam.transform.forward*1000;
            Destroy(Trail.gameObject, 3.0f);
        }
        
        Vector3 startPosition = Trail.transform.position;
        float distance = Vector3.Distance(Trail.transform.position, point);
        float remainingDistance = distance;

        while (remainingDistance > 0)
        {
            if (Trail == null) yield break;
            Trail.transform.position = Vector3.Lerp(startPosition, point, 1 - (remainingDistance / distance));

            remainingDistance -= 100 * Time.deltaTime;

            yield return null;
        }
        Trail.transform.position = point;

        var i = Instantiate(ImpactParticleSystem, HitPoint.point, Quaternion.LookRotation(HitPoint.normal));
        i.transform.parent = HitPoint.transform;

        Destroy(Trail.gameObject, Trail.time);
    }
}
