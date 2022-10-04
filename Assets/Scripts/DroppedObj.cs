using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(SphereCollider))]
public class DroppedObj : MonoBehaviour
{
    void Awake()
    {
        GetComponent<SphereCollider>().isTrigger = true;
    }
    private void OnTriggerEnter(Collider other) {
        if(other.gameObject.layer == LayerMask.NameToLayer("Player"))
        {
            WhenPicked();
            //particulas
            Destroy(gameObject);
        }
    }
    protected virtual void WhenPicked() {
        //things to do when the item gets picked(heal, more ammo, upgrade, etc)
    }
}
