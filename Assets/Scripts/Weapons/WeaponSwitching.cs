using System;
using System.Collections;
using System.Collections.Generic;
using System.Security.Claims;
using Unity.Mathematics;
using UnityEngine;
using UnityEngine.InputSystem;

public class WeaponSwitching : MonoBehaviour
{
    [Header("References")] 
    [SerializeField] private Transform[] weapons;
    private int[] yourWeapons; //sames index as weapons, but 0 = dont have and 1 = you have it

    [Header("Settings")] 
    [SerializeField] private float switchTime;

    private PlayerControls _controls;
    [SerializeField]private int selectedWeaponIndex;
    private float timeSinceLastSwitch;

    private void Awake() => _controls = PlayerInputs.Controls;

    private void OnEnable()
    {
        _controls.Player.SwitchWeapons.performed += Select;
        _controls.Player.ScrollWeapons.performed += SelectScroll;
    }

    private void OnDisable()
    {
        _controls.Player.SwitchWeapons.performed -= Select;
        _controls.Player.ScrollWeapons.performed -= SelectScroll;
    }

    // Start is called before the first frame update
    void Start()
    {
        SetWeapons();
    }

    // Update is called once per frame
    void Update()
    {
        timeSinceLastSwitch += Time.deltaTime;
    }

    private void SetWeapons()
    {
        weapons = new Transform[transform.childCount];
        yourWeapons = new int[transform.childCount];

        for (int i = 0; i < transform.childCount; i++)
        {
            weapons[i] = transform.GetChild(i);
            yourWeapons[i] = 1;
        }
        yourWeapons[0] = 1;

        selectedWeaponIndex = 0;
    }

    private void Select(InputAction.CallbackContext ctx)
    {
        int index = (int)ctx.ReadValue<float>()-1;
        if (yourWeapons[index] == 1 && timeSinceLastSwitch > switchTime)
        {
            weapons[selectedWeaponIndex].gameObject.SetActive(false);
            weapons[index].gameObject.SetActive(true);
            selectedWeaponIndex = index;

            timeSinceLastSwitch = 0;
        }
    }
    
    private void SelectScroll(InputAction.CallbackContext ctx)
    {
        int index = ctx.ReadValue<float>() > 0 ? selectedWeaponIndex+1 : selectedWeaponIndex-1;
        index = math.clamp(index, 0, weapons.Length-1);
        if (index != selectedWeaponIndex && yourWeapons[index] == 1 && timeSinceLastSwitch > switchTime)
        {
            weapons[selectedWeaponIndex].gameObject.SetActive(false);
            weapons[index].gameObject.SetActive(true);
            selectedWeaponIndex = index;

            timeSinceLastSwitch = 0;
        }
    }
}
