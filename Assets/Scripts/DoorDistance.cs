using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DoorDistance : DoorMovement
{
    bool playerInside;
    [SerializeField] float range;
    [SerializeField] LayerMask whatIsPlayer;
    BoxCollider col;
    Collider playerCollider;
    Vector3 centerP;
    
    new void Start()
    {
        base.Start();
        playerInside = false;
        playerCollider = FindObjectOfType<PlayerController>().GetComponent<Collider>();
        col = GetComponent<BoxCollider>();
        centerP = new Vector3(col.bounds.center.x,col.bounds.center.y,col.bounds.center.z);
    }
    private void OnDrawGizmos() {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(centerP,range);
    }

    void Update()
    {
        CheckIfPlayerInRange();
    }
    void CheckIfPlayerInRange()
    {
        if(Vector3.Distance(centerP, playerCollider.bounds.center) > range)
        {
            if(playerInside)
            {
                playerInside = !playerInside;
                base.Close();
            }
        }
        else
        {
            if(!playerInside)
            {
                playerInside = !playerInside;
                base.Open();
            }
        }
    }
}
