using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HealthPickable : MonoBehaviour,IPickable
{
    [SerializeField] float health;
    HealthSystem healthSystem;
    public void Pick()
    {
        if(healthSystem == null) healthSystem = GameManager.GetGameManager().GetPlayer().GetComponent<HealthSystem>();

        if(healthSystem.CanHeal())
        {
            healthSystem.Heal(health);
            Destroy(this.gameObject);
        }
    }
}
