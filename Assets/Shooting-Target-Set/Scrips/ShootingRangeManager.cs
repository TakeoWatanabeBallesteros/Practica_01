using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ShootingRangeManager : MonoBehaviour
{
    private ShootingRangeTarget[] targets;
    [SerializeField] private bool reset;
    // Start is called before the first frame update
    void Start()
    {
        targets = FindObjectsOfType<ShootingRangeTarget>();
    }

    private void Update()
    {
        if(!reset) return;
        Reset();
        reset = false;
    }

    private void Reset()
    {
        foreach (var x in targets)
        {
            StartCoroutine(x.Reset());
        }
    }
}
