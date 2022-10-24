using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ShieldPickable : MonoBehaviour,IPickable,IReset
{
    [SerializeField] float shield;
    HealthSystem healthSystem;
    public bool dontDestroy = false;
    public void Pick()
    {
        if(healthSystem == null) healthSystem = GameManager.GetGameManager().GetPlayer().GetComponent<HealthSystem>();

        if(healthSystem.CanShield())
        {
            healthSystem.Shield(shield);
            if(dontDestroy) gameObject.SetActive(false);
            else Destroy(this.gameObject);
        }
    }
    public void Reset()
    {
        gameObject.SetActive(true);
    }
}
