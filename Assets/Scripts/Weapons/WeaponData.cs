using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName="NewWeapon", menuName="Weapons/Gun")]
public class WeaponData : ScriptableObject {

    [Header("Info")]
    public new string name;

    public TypeOfWeapon type;

    [Header("Shooting")]
    public int damage;
    public float velocity;
    
    [Header("Reloading")]   
    public int maxAmmo;
    public int magSize;
    [Tooltip("In RPM")] public float fireRate;
    public float reloadTime;
    [HideInInspector] public bool reloading;
    
}

public enum TypeOfWeapon
{
    Pistol,
    Smg,
    Rifle,
    Sniper
}