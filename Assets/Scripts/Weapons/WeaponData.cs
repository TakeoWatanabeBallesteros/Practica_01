using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName="NewWeapon", menuName="Weapons/Gun")]
public class WeaponData : ScriptableObject {

    [Header("Info")]
    public new string name;
    public Sprite logo;

    public TypeOfWeapon type;

    [Header("Shooting")]
    public float damage;
    public float velocity;
    
    [Header("Reloading")]   
    public int maxAmmo;
    public int magSize;
    [Tooltip("In RPM")] public float fireRate;
    public float reloadTime;
    [HideInInspector] public bool reloading;

    [Header("Recoil")] 
    public float recoilX;
    public float recoilY;
    public float recoilZ;
    [Space]
    public float recoilAimX;
    public float recoilAimY;
    public float recoilAimZ;
    
}

public enum TypeOfWeapon
{
    Pistol,
    Smg,
    Rifle,
    Sniper
}