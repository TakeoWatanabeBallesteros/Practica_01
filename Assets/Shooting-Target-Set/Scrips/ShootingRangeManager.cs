using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ShootingRangeManager : MonoBehaviour
{
    [SerializeField] private List<ShootingRangeTarget> targets;
    [SerializeField] private bool reset;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    private void Update()
    {
        if(!reset) return;
        foreach (var x in targets)
        {
            x.Reset();
        }

        reset = false;
    }

    private void Reset()
    {
        
    }
}
