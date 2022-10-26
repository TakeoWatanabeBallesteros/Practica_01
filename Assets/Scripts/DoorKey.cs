using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class DoorKey : Door
{
    [SerializeField] private Outline outline;
    [SerializeField] private Transform player;
    [SerializeField] private GameObject UI;
    [SerializeField] Keys key;
    [SerializeField] private Material m1, m2, m3;
    [SerializeField] private MeshRenderer panel;
    
    
    
    private PlayerControls _controls;
    private bool open = false;
    private bool onSee = true;
    
    void Start()
    {
        _controls = PlayerInputs.Controls;
        panel.material = m1;
    }

    private void Update()
    {
        if ((Vector3.Distance(player.position, transform.position) > 1.1f ||
             !(Vector3.Dot(player.forward.normalized, (transform.position - player.position).normalized) > .98f))&& onSee)
        {
            _controls.Player.Interact.performed -= Interact;
            outline.enabled = false;
            UI.SetActive(false);
            onSee = false;
        }
        else if (!(Vector3.Distance(player.position, transform.position) > 1.1f ||
                   !(Vector3.Dot(player.forward.normalized, (transform.position - player.position).normalized) > .98f))&& !onSee)
        {
            _controls.Player.Interact.performed += Interact;
            UI.SetActive(true);
            onSee = true;
#if UNITY_EDITOR
#else
            outline.enabled = true;
#endif
        }
    }

    private void Interact(InputAction.CallbackContext ctx)
    {
        if (open) 
            Close();
        else
            Open();
    }

    public override void Open()
    {
        if(GameManager.GetGameManager().GetKey(key))
        {
            
            if(panel.material.color == Color.black) StartCoroutine(ShowColor(m3));
            base.Open();
            open = true;
            return;
        }

        if(panel.material.color == Color.black) StartCoroutine(ShowColor(m2));

    }
    public override void Close()
    {
        if(GameManager.GetGameManager().GetKey(key))
        {
            if(panel.material.color == Color.black) StartCoroutine(ShowColor(m3));
            base.Close();
            open = false;
        }
    }

    private IEnumerator ShowColor(Material m)
    {
        panel.material = m;
        yield return new WaitForSeconds(3.0f);
        panel.material = m1;
    }
}
