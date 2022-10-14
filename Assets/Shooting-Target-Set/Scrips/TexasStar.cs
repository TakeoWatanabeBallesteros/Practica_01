using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TexasStar : MonoBehaviour
{
    [SerializeField] private Transform rotationBase;
    [SerializeField] private float rotationSpeed = 1f;
    [SerializeField] public int hits;

    [SerializeField] private GameObject[] targets;
    [SerializeField] private Quaternion[] defaultAngles;

    private void Start()
    {
        for (int i = 0; i < 5; i++)
        {
            defaultAngles[i] = targets[i].transform.localRotation;
        }
    }

    // Update is called once per frame
    void Update()
    {
        rotationBase.Rotate(0,0 , rotationSpeed);
        if(hits < 5 ) return;
        Reset();
    }

    public void Reset()
    {
        for (int i = 0; i < 5; i++)
        {
            targets[i].transform.localRotation = defaultAngles[i];
            targets[i].GetComponent<TexasStarTarget>().hit = false;
        }

        hits = 0;
    }
}
