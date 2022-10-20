using System;
using Unity.Mathematics;
using UnityEngine;

public class WeaponSway : MonoBehaviour {

    [Header("Sway Settings")]
    [SerializeField] private float smooth;
    [SerializeField] private float multiplier;
    
    private PlayerControls _controls;
    private Vector2 lookVector;

    private void Awake()
    {
        _controls = PlayerInputs.Controls;
    }

    private void OnEnable()
    {
        
        _controls.Player.Look.performed += ctx => lookVector = ctx.ReadValue<Vector2>();
        _controls.Player.Look.canceled += ctx => lookVector = Vector2.zero;
    }

    private void OnDisable()
    {
        _controls.Player.Look.performed -= ctx => lookVector = ctx.ReadValue<Vector2>();
        _controls.Player.Look.canceled -= ctx => lookVector = Vector2.zero;
    }

    private void Update()
    {
        // get mouse input
        float mouseX = lookVector.x * multiplier;
        float mouseY = lookVector.y * multiplier;

        // calculate target rotation
        Quaternion rotationX = Quaternion.AngleAxis(-mouseY, Vector3.right);
        Quaternion rotationY = Quaternion.AngleAxis(mouseX, Vector3.up);

        Quaternion targetRotation = rotationX * rotationY;

        // rotate 
        transform.localRotation = Quaternion.Slerp(transform.localRotation, targetRotation, smooth * Time.deltaTime);
    }
}
