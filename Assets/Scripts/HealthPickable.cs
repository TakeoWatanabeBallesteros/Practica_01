using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HealthPickable : MonoBehaviour,IPickable,IReset
{
    [SerializeField] float health;
    HealthSystem healthSystem;
    public bool dontDestroy = false;
    public void Pick()
    {
        if(healthSystem == null) healthSystem = GameManager.GetGameManager().GetPlayer().GetComponent<HealthSystem>();

        if(healthSystem.CanHeal())
        {
            healthSystem.Heal(health);
            if(dontDestroy) gameObject.SetActive(false);
            else Destroy(this.gameObject);
        }
    }
    public void Reset()
    {
        gameObject.SetActive(true);
    }
}
