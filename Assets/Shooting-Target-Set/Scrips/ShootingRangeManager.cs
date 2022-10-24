using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ShootingRangeManager : MonoBehaviour
{
    [SerializeField] private List<ShootingRangeTarget> targets;
    [SerializeField] private bool reset;
    [SerializeField] private float roundMaxTime;
    private bool isRoundActive;
    public float roundTime { get; private set; }

    // Start is called before the first frame update
    void Start()
    {
        
    }

    private void Update()
    {
        if (isRoundActive) roundTime += Time.deltaTime;
        
        if(roundTime >= roundMaxTime) FinishRound();
        
        if(!reset) return;
        foreach (var x in targets)
        {
            x.Reset();
        }

        reset = false;
    }

    private void ResetTargets()
    {
        
    }

    private void StartRound()
    {
        
    }

    private void FinishRound()
    {
        roundTime = 0;
        isRoundActive = false;
        reset = true;
    }
}
