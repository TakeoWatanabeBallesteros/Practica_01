using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.Serialization;
using Random = UnityEngine.Random;

[RequireComponent(typeof(CharacterController))]
//[RequireComponent(typeof(PlayerInput))]
//TODO: Move audio to another script
public class PlayerController : MonoBehaviour
{
    [Header("Player")]
    [Tooltip("The model of the player")]
    [SerializeField]
    private GameObject model;
    
    [Tooltip("Move speed of the character in m/s")]
    [SerializeField]
    private float moveSpeed = 2.0f;

    [Tooltip("Sprint speed of the character in m/s")]
    [SerializeField]
    private float sprintSpeed = 5.335f;

    [Tooltip("How fast the character turns to face movement direction")]
    [SerializeField]
    [Range(0.0f, 0.3f)]
    private float rotationSmoothTime = 0.12f;

    [Tooltip("Acceleration and deceleration")]
    [SerializeField]
    private float speedChangeRate = 10.0f;

    [SerializeField]
    private AudioClip landingAudioClip;
    [SerializeField]
    private AudioClip[] footstepAudioClips;
    [SerializeField]
    [Range(0, 1)] private float footstepAudioVolume = 0.5f;

    [Space(10)]
    [Tooltip("The height the player can jump")]
    [SerializeField]
    private float jumpHeight = 1.2f;

    [Tooltip("The character uses its own gravity value. The engine default is -9.81f")]
    [SerializeField]
    private float gravity = -15.0f;

    [Space(10)]
    [Tooltip("Time required to pass before being able to jump again. Set to 0f to instantly jump again")]
    [SerializeField]
    private float jumpTimeout = 0.50f;

    [Tooltip("Time required to pass before entering the fall state. Useful for walking down stairs")]
    [SerializeField]
    private float fallTimeout = 0.15f;

    [Header("Player Grounded")]
    [Tooltip("If the character is grounded or not. Not part of the CharacterController built in grounded check")]
    [SerializeField]
    private bool grounded = true;

    private CollisionFlags collisionFlags;

    [Header("Camera")]
    [Tooltip("The position of the main camera, normally the head")]
    [SerializeField]
    private Transform targetPitch;

    [Tooltip("The camera that shows the gun and player")]
    [SerializeField] 
    private Transform gunCamera;
    
    [Tooltip("Add offset to the position of the camera")]
    [SerializeField]
    private Vector3 cameraOffset;
    
    [Tooltip("The position of the gun camera")]
    [SerializeField]
    private Transform targetGunCam;
    
    [Tooltip("How far in degrees can you move the camera up")]
    [SerializeField]
    private float topClamp = 70.0f;

    [Tooltip("How far in degrees can you move the camera down")]
    [SerializeField]
    private float bottomClamp = -30.0f;

    [Tooltip("Additional degrees to override the camera. Useful for fine tuning camera position when locked")]
    [SerializeField]
    private float cameraAngleOverride = 0.0f;

    [Tooltip("For locking the camera position on all axis")]
    [SerializeField]
    private bool lockCameraPosition = false;

    [Tooltip("Fix or not the neck")]
    [SerializeField]
    private bool rotateHead = true;
    
    [Header("Weapon")]
    [Tooltip("The transform of the weapon, to edit the pitch")]
    [SerializeField]
    private Transform weaponPosition;
    [Tooltip("The default position of the weapon")]
    [SerializeField]
    private Transform weaponIdlePosition;
    [Tooltip("The position of the weapon when aiming")]
    [SerializeField]
    private Transform weaponAimPosition;
    
    // camera
    float yaw;
    float pitch;
    
    // player
    private float _speed;
    private float _animationBlend;
    private float _targetRotation = 0.0f;
    private float _fpsRotationVelocity;
    private float _rotationVelocity;
    private float _verticalVelocity;
    private float _terminalVelocity = 53.0f;

    // timeout deltatime
    private float _jumpTimeoutDelta;
    private float _fallTimeoutDelta;

    // animation IDs
    private int _animIDSpeed;
    private int _animIDGrounded;
    private int _animIDJump;
    private int _animIDFreeFall;
    private int _animIDMotionSpeed;

    private PlayerInput _playerInput;
    private Animator _animator;
    private CharacterController _controller;
    //Change class name and action names
    private PlayerInputs _input;
    private GameObject _mainCamera;

    private const float Threshold = 0.01f;

    private bool _hasAnimator;

    private bool IsCurrentDeviceMouse => _playerInput.currentControlScheme == "KeyboardMouse";

    //TODO: Salto más dínamico
    //TODO: Crouch
    //TODO: Modelo 3D con animaciones y sonidos
    //TODO: Movimiento más fluido, acelerar y decelerar

    
    [SerializeField] float m_YawRotationSpeed;
    [SerializeField] float m_PitchRotationSpeed;

    [SerializeField] Transform m_PitchController;

    [SerializeField] CharacterController m_CharacterController;

    [SerializeField] Camera m_Camera;
    [SerializeField] float m_NormalMovementFOV;
    [SerializeField] float m_RunMovementFOV;
    [SerializeField] private float m_CrouchMovementFOV;

    float m_VerticalSpeed = 0.0f;

    private float timeOnAir;
    private float targetSpeed;
    private Vector2 moveVector;
    private Vector2 lookVector;

    private bool aiming = false;

    private PlayerControls _controls;
    
    bool cameraLocked = false;

    private void Awake()
    {
        _controls = PlayerInputs.Controls;
    }

    void OnEnable() {
#if UNITY_EDITOR
        _controls.Debug.LockCamera.performed += _ =>
        {
            cameraLocked = !cameraLocked;
            if (cameraLocked)
            {
                _controls.Player.Look.Disable();
                Cursor.lockState = CursorLockMode.None;
            }
            else
            {
                _controls.Player.Look.Enable();
                Cursor.lockState = CursorLockMode.Locked;
            }
            Cursor.visible = cameraLocked;
        };
#endif
        _controls.Player.Look.performed += ctx => lookVector = ctx.ReadValue<Vector2>();
        _controls.Player.Look.canceled += ctx => lookVector = Vector2.zero;
        
        _controls.Player.Move.performed += ctx => moveVector = ctx.ReadValue<Vector2>();
        _controls.Player.Move.canceled += ctx => moveVector = Vector2.zero;
        
        // set target speed based on move speed, sprint speed and if sprint is pressed
        _controls.Player.Sprint.performed += ctx => targetSpeed = sprintSpeed;
        _controls.Player.Sprint.canceled += ctx => targetSpeed = moveSpeed;
        
        _controls.Player.Jump.performed += Jump;
        //Controls.Player.Jump.canceled += ctx => OnJumpFinished();
        
        _controls.Player.Aim.performed += Aim;
        _controls.Player.Aim.canceled += Aim;
        
    }

    void OnDisable() {
        _controls.Player.Look.performed -= ctx => lookVector = ctx.ReadValue<Vector2>();
        _controls.Player.Look.canceled -= ctx => lookVector = Vector2.zero;
        
        _controls.Player.Move.performed -= ctx => moveVector = ctx.ReadValue<Vector2>();
        _controls.Player.Move.canceled -= ctx => moveVector = Vector2.zero;
        
        // set target speed based on move speed, sprint speed and if sprint is pressed
        _controls.Player.Sprint.performed -= ctx => targetSpeed = sprintSpeed;
        _controls.Player.Sprint.canceled -= ctx => targetSpeed = moveSpeed;

        _controls.Player.Jump.performed -= Jump;
        //Controls.Player.Jump.canceled -= ctx => OnJumpFinished();
        
        _controls.Player.Aim.performed -= Aim;
        _controls.Player.Aim.canceled -= Aim;
    }

    private void Start()
    {
        _hasAnimator = TryGetComponent(out _animator);
        _controller = GetComponent<CharacterController>();
        _input = GetComponent<PlayerInputs>();
        _playerInput = GetComponent<PlayerInput>();

        AssignAnimationIDs();
        
        yaw = transform.rotation.y;
        pitch = m_PitchController.localRotation.x;

        targetSpeed = moveSpeed;
        
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = cameraLocked;
    }
    
    private void AssignAnimationIDs()
    {
        _animIDSpeed = Animator.StringToHash("Speed");
        _animIDGrounded = Animator.StringToHash("Grounded");
        _animIDJump = Animator.StringToHash("Jump");
        _animIDFreeFall = Animator.StringToHash("FreeFall");
        _animIDMotionSpeed = Animator.StringToHash("MotionSpeed");
    }

    // Update is called once per frame
    private void Update()
    {
        Move();
        GroundAndGravity();
        Aiming();
    }

    private void LateUpdate()
    {
        CameraRotation();
    }

    // TODO: Should have a sensitivity mouse variable
    private void CameraRotation()
    {
        // Set Camera Root to head position
        m_PitchController.position = targetPitch.position;
        
        // if there is an input
		if (lookVector.sqrMagnitude >= Threshold)
		{
            pitch += lookVector.y * Time.deltaTime*m_PitchRotationSpeed;
			yaw += lookVector.x * Time.deltaTime*m_YawRotationSpeed;

			// clamp our pitch rotation
			pitch = ClampAngle(pitch, bottomClamp, topClamp);
            
            // Update camera target pitch
            m_PitchController.transform.localRotation = Quaternion.Euler(pitch, 0.0f, 0.0f);
            //gun.localRotation = Quaternion.Euler(-pitch, -180f, 0.0f);

            // rotate the player left and right
            // TODO: Rotate the body with IK animation
            transform.rotation = Quaternion.Euler(0.0f, yaw, 0.0f);
        }
        
        // Set Camera Root to head position
        gunCamera.position = targetPitch.position+targetPitch.forward*.05f;
        gunCamera.rotation = targetGunCam.rotation;
    }

    private void OnAnimatorIK(int layerIndex)
    {
        // Neck X axis rotation
        // that means look up & down

        // Only rotate X axis if First Person
        // else also rotate Y axis
        Quaternion q = Quaternion.Euler(pitch,
            0.0f, 0.0f);
        _animator.SetBoneLocalRotation(HumanBodyBones.Chest, q);
    }

    private void Move()
    { 
        // a simplistic acceleration and deceleration designed to be easy to remove, replace, or iterate upon

        // note: Vector2's == operator uses approximation so is not floating point error prone, and is cheaper than magnitude
        // if there is input, and we check if we were pressing sprint (because the sprint event set the targetSpeed before this)
        // else set the target speed to 0
        if (moveVector != Vector2.zero && targetSpeed <= moveSpeed) targetSpeed = moveSpeed;
        else if(moveVector == Vector2.zero && targetSpeed < sprintSpeed) targetSpeed = 0.0f;

        // a reference to the players current horizontal velocity
        float currentHorizontalSpeed = new Vector3(_controller.velocity.x, 0.0f, _controller.velocity.z).magnitude;

        float speedOffset = 0.1f;
        float inputMagnitude = moveVector.magnitude < 1 && moveVector.magnitude != 0 ? moveVector.magnitude : 1f;

        // accelerate or decelerate to target speed
        // creates curved result rather than a linear one giving a more organic speed change
        // note T in Lerp is clamped, so we don't need to clamp our speed
        _speed = Mathf.Lerp(currentHorizontalSpeed, targetSpeed * inputMagnitude,
            Time.deltaTime * speedChangeRate);

        // round speed to 3 decimal places
        _speed = Mathf.Round(_speed * 1000f) / 1000f;

        // normalise input direction
        Vector3 inputDirection = new Vector3(moveVector.x, 0.0f, moveVector.y).normalized;

        // note: Vector2's != operator uses approximation so is not floating point error prone, and is cheaper than magnitude
        // if there is a move input rotate player when the player is moving
        if (moveVector != Vector2.zero)
        {
            // move
            inputDirection = transform.right * moveVector.x + transform.forward * moveVector.y;
        }
        // move the player
        collisionFlags =  m_CharacterController.Move(inputDirection.normalized * (_speed * Time.deltaTime) +
                                                            new Vector3(0.0f, _verticalVelocity, 0.0f) * Time.deltaTime);
        //transform.position = Vector3.zero;
        _animationBlend = Mathf.Lerp(_animationBlend, _speed, Time.deltaTime * speedChangeRate);
        if (_animationBlend < 0.01f) _animationBlend = 0f;

        // update animator if using character
        if (_hasAnimator)
        {
            _animator.SetFloat(_animIDSpeed, _animationBlend);
            _animator.SetFloat(_animIDMotionSpeed, inputMagnitude);
        }
    }
    
    private void GroundAndGravity()
    {
        if ((collisionFlags & CollisionFlags.Below)!=0)
        {
            m_VerticalSpeed = 0.0f;
            timeOnAir = 0;
            grounded = true;
            // update animator if using character
            if (_hasAnimator)
            {
                _animator.SetBool(_animIDGrounded, grounded);
            }
        }
        else grounded = false;
        
        if (grounded)
        {
            // reset the fall timeout timer
            _fallTimeoutDelta = fallTimeout;

            // update animator if using character
            if (_hasAnimator)
            {
                _animator.SetBool(_animIDJump, false);
                _animator.SetBool(_animIDFreeFall, false);
            }

            // stop our velocity dropping infinitely when grounded
            if (_verticalVelocity < 0.0f)
            {
                _verticalVelocity = 0f;
            }

            // jump timeout
            if (_jumpTimeoutDelta >= 0.0f)
            {
                _jumpTimeoutDelta -= Time.deltaTime;
            }
        }
        else
        {
            // reset the jump timeout timer
            _jumpTimeoutDelta = jumpTimeout;

            // fall timeout
            if (_fallTimeoutDelta >= 0.0f)
            {
                _fallTimeoutDelta -= Time.deltaTime;
            }
            else
            {
                // update animator if using character
                if (_hasAnimator)
                {
                    _animator.SetBool(_animIDFreeFall, true);
                }
            }

            if ((collisionFlags & CollisionFlags.Above) != 0)
            {
                _verticalVelocity = 0.0f;
            }
        }

        _verticalVelocity += gravity * Time.deltaTime;
        timeOnAir += grounded ? 0 : Time.deltaTime;
    }

    private void Jump(InputAction.CallbackContext ctx)
    {
        if (!(_jumpTimeoutDelta <= 0.0f) || !(timeOnAir < .1f)) return;
        // the square root of H * -2 * G = how much velocity needed to reach desired height
        _verticalVelocity = Mathf.Sqrt(jumpHeight * -2f * gravity);

        // update animator if using character
        if (_hasAnimator)
        {
            _animator.SetBool(_animIDJump, true);
        }
    }

    private void Aim(InputAction.CallbackContext ctx) {aiming = !aiming; }

    private void Aiming()
    {
        if (!aiming)
        {
            weaponPosition.position = Vector3.Lerp(weaponPosition.position, weaponIdlePosition.position, .1f);
        }
        else
        {
            weaponPosition.position = Vector3.Lerp(weaponPosition.position, weaponAimPosition.position, .1f);
        }
    }
    
    private static float ClampAngle(float lfAngle, float lfMin, float lfMax)
    {
        if (lfAngle < -360f) lfAngle += 360f;
        if (lfAngle > 360f) lfAngle -= 360f;
        return Mathf.Clamp(lfAngle, lfMin, lfMax);
    }
    
    private void OnFootstep(AnimationEvent animationEvent)
    {
        if (animationEvent.animatorClipInfo.weight > 0.5f)
        {
            if (footstepAudioClips.Length > 0)
            {
                var index = Random.Range(0, footstepAudioClips.Length);
                AudioSource.PlayClipAtPoint(footstepAudioClips[index], transform.TransformPoint(_controller.center), footstepAudioVolume);
            }
        }
    }

    private void OnLand(AnimationEvent animationEvent)
    {
        if (animationEvent.animatorClipInfo.weight > 0.5f)
        {
            AudioSource.PlayClipAtPoint(landingAudioClip, transform.TransformPoint(_controller.center), footstepAudioVolume);
        }
    }
}