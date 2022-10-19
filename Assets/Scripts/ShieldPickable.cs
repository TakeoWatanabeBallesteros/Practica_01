using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ShieldPickable : MonoBehaviour,IPickable
{
    [SerializeField] float shield;
    HealthSystem healthSystem;
    public void Pick()
    {
        if(healthSystem == null) healthSystem = GameManager.GetGameManager().GetPlayer().GetComponent<HealthSystem>();

        if(healthSystem.CanShield())
        {
            healthSystem.Shield(shield);
            Destroy(this.gameObject);
        }
    }
}
