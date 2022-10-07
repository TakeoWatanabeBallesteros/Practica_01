using System;
using UnityEngine;
#if ENABLE_INPUT_SYSTEM
using UnityEngine.InputSystem;
#endif

public class PlayerInputs : MonoBehaviour
{
	static PlayerControls controls;
    public static PlayerControls Controls{
        get{
            if(controls != null) {return controls;}
            return controls = new PlayerControls();
        }
    }

    private void Awake()
    {
        PlayerInputs.Enable();
    }

    public static void Enable()
    {
        Controls.Enable();
    }
    
    public static void Disable()
    {
        Controls.Disable();
    }
}