using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Door : MonoBehaviour
{
    Animator anim;
    private void Start() {
        anim = GetComponent<Animator>();
    }

    public virtual void Open()
    {
        anim.SetBool("character_nearby",true);
    }
    public virtual void Close()
    {
        anim.SetBool("character_nearby",false);
    }
    public void Reset()
    {
        anim.SetBool("character_nearby",false);
        anim.Play("Close");
    }
}
