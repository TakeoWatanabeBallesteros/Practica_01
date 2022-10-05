using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;
using TMPro;

public class DoorKey : DoorMovement
{
    [SerializeField] int keyNumber;
    [SerializeField] Collider lockCollider;
    Collider playerCol;
    [SerializeField] float range;
    [SerializeField] Image background;
    [SerializeField] float alphaSpeed;
    bool fadeIn;
    bool fadeOut;
    bool interacting;
    bool activated;
    
    new void Start()
    {
        base.Start();
        playerCol = FindObjectOfType<PlayerController>().GetComponent<Collider>();
        CheckKey();
    }
    private void OnDrawGizmos() {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(lockCollider.bounds.center,range);
    }

    
    void Update()
    {
        if(!activated)
        {
            Check();
        }
    }
    void Check()
    {
        if(InRange() && LookingAtLock())
        {
            if(background.color.a < 1 && !fadeIn)
            {
                StopAllCoroutines();
                fadeOut = false;
                StartCoroutine(FadeIn());
            }

            if(!interacting)
            {
                //suscribirse al metodo
                interacting = true;
            }
        }
        else
        {
            if(background.color.a > 0 && !fadeOut)
            {
                StopAllCoroutines();
                fadeIn = false;
                StartCoroutine(FadeOut());
            }

            if(interacting)
            {
                //desuscribirse del metodo
                interacting = false;
            }
        }
    }

    bool InRange()
    {
        return Vector2.Distance(new Vector2(lockCollider.bounds.center.x,lockCollider.bounds.center.z), new Vector2(playerCol.bounds.center.x,playerCol.bounds.center.z)) < range;
    }
    bool LookingAtLock()
    {
        RaycastHit hit;
        if(Physics.Raycast(Camera.main.transform.position, Camera.main.transform.forward, out hit))
        {
            return hit.collider == lockCollider;
        }
        else
        {
            return false;
        }
        
    }

    IEnumerator FadeIn()
    {
        fadeIn = true;
        while(background.color.a < 1)
        {
            background.color += new Color(0,0,0,alphaSpeed * Time.deltaTime);
            yield return null;
        }
        fadeIn = false;
        background.color = new Color(background.color.r,background.color.g,background.color.b,1);
        
    }
    IEnumerator FadeOut()
    {
        fadeOut = true;
        while(background.color.a > 0)
        {
            background.color -= new Color(0,0,0,alphaSpeed * Time.deltaTime);
            yield return null;
        }
        fadeOut = false;
        background.color = new Color(background.color.r,background.color.g,background.color.b,0);
    }

    void CheckKey()
    {
        int[] keys = GameManager.instance.ReadKeys();
        if(keys[keyNumber] == 1)
        {
            base.Open();
            background.color = new Color(background.color.r,background.color.g,background.color.b,0);
            activated = true;
            //desuscribirse del metodo
        }
    }
}
