using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DeadZone : MonoBehaviour
{
    public void Die(HealthSystem healthSystem)
    {
        healthSystem.TakeDamage(1000);
    }
}
