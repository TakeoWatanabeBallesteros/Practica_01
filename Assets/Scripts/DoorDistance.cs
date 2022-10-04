using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DoorDistance : DoorMovement
{
    bool playerInside;
    [SerializeField] float range;
    [SerializeField] LayerMask whatIsPlayer;
    [SerializeField] BoxCollider col;
    Collider playerCol;
    new void Start()
    {
        base.Start();
        col = GetComponent<BoxCollider>();
        playerCol = FindObjectOfType<PlayerController>().GetComponent<Collider>();
    }
    private void OnDrawGizmos() {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(col.bounds.center,range);
    }

    void Update()
    {
        CheckIfPlayerInRange();
    }
    void CheckIfPlayerInRange()
    {
        if(Vector2.Distance(new Vector2(col.bounds.center.x,col.bounds.center.z), new Vector2(playerCol.bounds.center.x,playerCol.bounds.center.z)) > range)
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
