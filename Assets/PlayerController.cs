using System;
using System.Collections;
using System.Collections.Generic;
using StarterAssets;
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
    [Tooltip("Add offset to the position of the camera")]
    [SerializeField]
    private Transform targetPitch;
    
    [Tooltip("Add offset to the position of the camera")]
    [SerializeField]
    private Vector3 cameraOffset;
    
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

    private TypeOfView _typeOfView;

    //TODO: Salto más dínamico
    //TODO: Crouch
    //TODO: Modelo 3D con animaciones y sonidos
    //TODO: Movimiento más fluido, acelerar y decelerar

    
    [SerializeField] float m_YawRotationSpeed;
    [SerializeField] float m_PitchRotationSpeed;

    [SerializeField] float m_MinPitch;
    [SerializeField] float m_MaxPitch;

    [SerializeField] Transform m_PitchController;
    [SerializeField] bool m_UseYawInverted;
    [SerializeField] bool m_UsePitchInverted;

    [SerializeField] CharacterController m_CharacterController;
    [SerializeField] float m_DefaultSpeed;
    [SerializeField] float m_FastSpeedMultiplier = 3f;
    [SerializeField] private float m_LowSpeedMultiplier = .3f;
    [SerializeField] float m_CurrentSpeed;
    [SerializeField] KeyCode m_LeftKeyCode;
    [SerializeField] KeyCode m_RightKeyCode;
    [SerializeField] KeyCode m_UpKeyCode;
    [SerializeField] KeyCode m_DownKeyCode;
    [SerializeField] KeyCode m_JumpKeyCode;
    [SerializeField] KeyCode m_RunKeycode = KeyCode.LeftShift;
    [SerializeField] private KeyCode m_CrouchKeyCode = KeyCode.LeftControl;

    [SerializeField] Camera m_Camera;
    [SerializeField] float m_NormalMovementFOV;
    [SerializeField] float m_RunMovementFOV;
    [SerializeField] private float m_CrouchMovementFOV;

    float m_VerticalSpeed = 0.0f;
    [SerializeField] bool m_OnGround = true;

    [SerializeField] float m_JumpSpeed = 10.0f;

    private float timeOnAir;

    private void Start()
    {
        _hasAnimator = TryGetComponent(out _animator);
        _controller = GetComponent<CharacterController>();
        _input = GetComponent<PlayerInputs>();
        _playerInput = GetComponent<PlayerInput>();

        AssignAnimationIDs();
        
        yaw = transform.rotation.y;
        pitch = m_PitchController.localRotation.x;
        Cursor.visible = false;
        Cursor.lockState = CursorLockMode.Locked;
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
        JumpAndGravity();
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
		if (_input.look.sqrMagnitude >= Threshold)
		{
            pitch += _input.look.y * Time.deltaTime*m_PitchRotationSpeed;
			yaw += _input.look.x * Time.deltaTime*m_YawRotationSpeed;

			// clamp our pitch rotation
			pitch = ClampAngle(pitch, bottomClamp, topClamp);
            
            // Update Cinemachine camera target pitch
            m_PitchController.transform.localRotation = Quaternion.Euler(pitch, 0.0f, 0.0f);

            // rotate the player left and right
            // TODO: Rotate the body with IK animation
            transform.rotation = Quaternion.Euler(0.0f, yaw, 0.0f);
        }
    }

    private void OnAnimatorIK(int layerIndex)
    {
        // Neck X axis rotation
        // that means look up & down

        // Only rotate X axis if First Person
        // else also rotate Y axis
        Quaternion q = Quaternion.Euler(pitch,
            0.0f, 0.0f);
        _animator.SetBoneLocalRotation(HumanBodyBones.Head, q);
    }

    private void Move()
    {
        // set target speed based on move speed, sprint speed and if sprint is pressed
        float targetSpeed = _input.sprint ? sprintSpeed : moveSpeed;

        // a simplistic acceleration and deceleration designed to be easy to remove, replace, or iterate upon

        // note: Vector2's == operator uses approximation so is not floating point error prone, and is cheaper than magnitude
        // if there is no input, set the target speed to 0
        if (_input.move == Vector2.zero) targetSpeed = 0.0f;

        // a reference to the players current horizontal velocity
        float currentHorizontalSpeed = new Vector3(_controller.velocity.x, 0.0f, _controller.velocity.z).magnitude;

        float speedOffset = 0.1f;
        float inputMagnitude = _input.analogMovement ? _input.move.magnitude : 1f;

        // accelerate or decelerate to target speed
        // creates curved result rather than a linear one giving a more organic speed change
        // note T in Lerp is clamped, so we don't need to clamp our speed
        _speed = Mathf.Lerp(currentHorizontalSpeed, targetSpeed * inputMagnitude,
            Time.deltaTime * speedChangeRate);

        // round speed to 3 decimal places
        _speed = Mathf.Round(_speed * 1000f) / 1000f;

        // normalise input direction
        Vector3 inputDirection = new Vector3(_input.move.x, 0.0f, _input.move.y).normalized;

        // note: Vector2's != operator uses approximation so is not floating point error prone, and is cheaper than magnitude
        // if there is a move input rotate player when the player is moving
        if (_input.move != Vector2.zero)
        {
            // move
            inputDirection = transform.right * _input.move.x + transform.forward * _input.move.y;
        }

        // move the player
        collisionFlags =  m_CharacterController.Move(inputDirection.normalized * (_speed * Time.deltaTime) +
                                                            new Vector3(0.0f, _verticalVelocity, 0.0f) * Time.deltaTime);
        
        _animationBlend = Mathf.Lerp(_animationBlend, targetSpeed, Time.deltaTime * speedChangeRate);
        if (_animationBlend < 0.01f) _animationBlend = 0f;

        // update animator if using character
        if (_hasAnimator)
        {
            _animator.SetFloat(_animIDSpeed, _animationBlend);
            _animator.SetFloat(_animIDMotionSpeed, inputMagnitude);
        }
    }
    
    private void JumpAndGravity()
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

            // Jump
            if (_input.jump && _jumpTimeoutDelta <= 0.0f && timeOnAir < .1f)
            {
                // the square root of H * -2 * G = how much velocity needed to reach desired height
                _verticalVelocity = Mathf.Sqrt(jumpHeight * -2f * gravity);

                // update animator if using character
                if (_hasAnimator)
                {
                    _animator.SetBool(_animIDJump, true);
                }
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

            // if we are not grounded, do not jump
            _input.jump = false;
            
            if ((collisionFlags & CollisionFlags.Above) != 0)
            {
                _verticalVelocity = 0.0f;
            }
        }

        _verticalVelocity += gravity * Time.deltaTime;
        timeOnAir += grounded ? 0 : Time.deltaTime;
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