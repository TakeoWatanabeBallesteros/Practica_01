using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ShootingRangeManager : MonoBehaviour
{
    [SerializeField] private List<ShootingRangeTarget> targets;
    [SerializeField] private bool reset;
    [SerializeField] private float roundMaxTime;
    [SerializeField] private int points;
    [SerializeField] private bool isRoundActive;
    [SerializeField] private float roundTime;

    private void OnEnable()
    {
        StartButton.OnStartRound += StartRound;
        ShootingRangeTarget.OnHitPoint += AddPoints;
    }

    private void OnDisable()
    {
        StartButton.OnStartRound -= StartRound;
        ShootingRangeTarget.OnHitPoint -= AddPoints;
    }

    // Start is called before the first frame update
    void Start()
    {
        
    }

    private void Update()
    {
        if (isRoundActive) roundTime += Time.deltaTime;
        
        if(roundTime >= roundMaxTime) FinishRound();
        
        if(!reset) return;
        ResetTargets();

        reset = false;
    }

    private void ResetTargets()
    {
        Debug.Log("Ap");
        foreach (var x in targets)
        {
            x.Reset();
        }
    }

    private void StartRound()
    {
        if(isRoundActive) return;
        reset = true;
        points = 0;
        isRoundActive = true;
    }

    private void FinishRound()
    {
        roundTime = 0;
        isRoundActive = false;
        reset = true;
    }

    private void AddPoints(int points)
    {
        this.points += points;
    }
}
