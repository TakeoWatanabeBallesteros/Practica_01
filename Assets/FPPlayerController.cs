using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FPPlayerController : MonoBehaviour
{
    float m_Yaw;
    float m_Pitch;
    [SerializeField] float m_YawRotationSpeed;
    [SerializeField] float m_PitchRotationSpeed;

    [SerializeField] float m_MinPitch;
    [SerializeField] float m_MaxPitch;

    [SerializeField] Transform m_PitchController;
    [SerializeField] bool m_UseYawInverted;
    [SerializeField] bool m_UsePitchInverted;

    [SerializeField] CharacterController m_CharacterController;
    [SerializeField] float m_Speed;
    [SerializeField] float m_FastSpeedMultiplier = 3f;
    [SerializeField] KeyCode m_LeftKeyCode;
    [SerializeField] KeyCode m_RightKeyCode;
    [SerializeField] KeyCode m_UpKeyCode;
    [SerializeField] KeyCode m_DownKeyCode;
    [SerializeField] KeyCode m_JumpKeyCode;
    [SerializeField] KeyCode m_RunKeycode = KeyCode.LeftShift;

    [SerializeField] Camera m_Camera;
    [SerializeField] float m_NormalMovementFOV;
    [SerializeField] float m_RunMovementFOV;

    float m_VerticalSpeed = 0.0f;
    [SerializeField] bool m_OnGround = true;

    [SerializeField] float m_JumpSpeed = 10.0f;

    private float lastVelocityY;

    void Start()
    {
        m_Yaw = transform.rotation.y;
        m_Pitch = m_PitchController.localRotation.x;
        Cursor.visible = false;
        Cursor.lockState = CursorLockMode.Locked;
    }

    // Update is called once per frame
    void Update()
    {
        Vector3 l_RightDirection = transform.right;
        Vector3 l_ForwardDirection = transform.forward;
        Vector3 l_Direction = Vector3.zero;
        float l_Speed = m_Speed;

        if(Input.GetKey(m_UpKeyCode))
            l_Direction = l_ForwardDirection;
        if (Input.GetKey(m_DownKeyCode))
            l_Direction = -l_ForwardDirection;
        if (Input.GetKey(m_RightKeyCode))
            l_Direction += l_RightDirection;
        if (Input.GetKey(m_LeftKeyCode))
            l_Direction -= l_RightDirection;
        if (Input.GetKeyDown(m_JumpKeyCode) && m_OnGround)
            m_VerticalSpeed = m_JumpSpeed;
        float l_FOV = m_NormalMovementFOV;
        if (Input.GetKey(m_RunKeycode))
        {
            l_Speed = m_Speed * m_FastSpeedMultiplier;
            l_FOV = m_RunMovementFOV;
        }
        m_Camera.fieldOfView = Mathf.Lerp(m_Camera.fieldOfView, l_FOV, .05f);
        

        l_Direction.Normalize();
        Vector3 l_Movement = l_Direction * l_Speed * Time.deltaTime;

        //Rotation
        float l_MouseX = Input.GetAxis("Mouse X");
        float l_MouseY = Input.GetAxis("Mouse Y");

        m_Yaw = m_Yaw + l_MouseX * m_YawRotationSpeed *Time.deltaTime * (m_UseYawInverted ? -1.0f : 1.0f);
        m_Pitch = m_Pitch + l_MouseY * m_PitchRotationSpeed *Time.deltaTime * (m_UsePitchInverted ? -1.0f : 1.0f);
        m_Pitch = Mathf.Clamp(m_Pitch, m_MinPitch, m_MaxPitch);

        transform.rotation = Quaternion.Euler(0.0f, m_Yaw, 0.0f);
        m_PitchController.localRotation = Quaternion.Euler(m_Pitch, 0.0f, 0.0f);

        m_VerticalSpeed = m_VerticalSpeed + Physics.gravity.y * Time.deltaTime;
        l_Movement.y = m_VerticalSpeed * Time.deltaTime;

        var lastPositionY = transform.position.y;

        CollisionFlags l_CollisionFlags =  m_CharacterController.Move(l_Movement);

        if ((l_CollisionFlags & CollisionFlags.Above) != 0 && m_VerticalSpeed > 0.0f)
        {
            m_VerticalSpeed = 0.0f;
        }
        //TODO Preguntar al profe si aplicar siempre gravedad es una solucion correcta
        if ((l_CollisionFlags & CollisionFlags.Below)!=0)
        {
            //m_VerticalSpeed = 0.0f;
            m_OnGround = true;
        }
        else
        {
            m_OnGround = false;
        }

        lastVelocityY = m_CharacterController.velocity.y;
    }

    bool OnZenit()
    {
        return lastVelocityY >= 0 && m_CharacterController.velocity.y < 0;
    }
}
