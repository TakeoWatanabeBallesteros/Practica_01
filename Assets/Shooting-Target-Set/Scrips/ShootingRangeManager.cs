using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class ShootingRangeManager : MonoBehaviour
{
    [SerializeField] private List<ShootingRangeTarget> targets;
    [SerializeField] private bool reset;
    [SerializeField] private float roundMaxTime;
    [SerializeField] private int points;
    [SerializeField] private bool isRoundActive;
    [SerializeField] private float roundTime;
    [SerializeField] private TextMeshProUGUI timerUI;
    [SerializeField] private TextMeshProUGUI pointsUI;

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
        if (isRoundActive)
        {
            roundTime -= Time.deltaTime;
            timerUI.text = $"Time: {Math.Round(roundTime)}";
        }
        
        if(roundTime <= 0) FinishRound();
        
        if(!reset) return;
        ResetTargets();

        reset = false;
    }

    private void ResetTargets()
    {
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
        timerUI.gameObject.SetActive(true);
        pointsUI.gameObject.SetActive(true);
    }

    private void FinishRound()
    {
        timerUI.gameObject.SetActive(false);
        pointsUI.gameObject.SetActive(false);
        roundTime = roundMaxTime;
        isRoundActive = false;
        reset = true;
    }

    private void AddPoints(int points)
    {
        this.points += points;
        pointsUI.text = $"Points: {this.points}";
    }
}
