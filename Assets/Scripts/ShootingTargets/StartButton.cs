using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class StartButton : MonoBehaviour
{
    private Outline outline;
    [SerializeField] private Transform player;
    [SerializeField] private GameObject UI;
    
    private PlayerControls _controls;
    
    public delegate void StartRound();

    public static event StartRound OnStartRound;
    private bool _enabled = true;

    // Start is called before the first frame update
    void Start()
    {
        _controls = PlayerInputs.Controls;
        outline = GetComponent<Outline>();
    }

    // Update is called once per frame
    void Update()
    {
        if ((Vector3.Distance(player.position, transform.position) > 1.1f ||
             !(Vector3.Dot(player.forward.normalized, (transform.position - player.position).normalized) > .995f)) && _enabled)
        {
            _controls.Player.Interact.performed -= _StartRound;
            outline.enabled = false;
            UI.SetActive(false);
            _enabled = false;
        }
        else if(!(Vector3.Distance(player.position, transform.position) > 1.1f ||
                 !(Vector3.Dot(player.forward.normalized, (transform.position - player.position).normalized) > .995f)) && !_enabled)
        {
            _controls.Player.Interact.performed += _StartRound;
            UI.SetActive(true);
            _enabled = true;
#if UNITY_EDITOR
#else
            if (!outline.enabled) outline.enabled = true;
#endif
        }

    }

    void _StartRound(InputAction.CallbackContext ctx)
    {
        OnStartRound?.Invoke();
    }
}
