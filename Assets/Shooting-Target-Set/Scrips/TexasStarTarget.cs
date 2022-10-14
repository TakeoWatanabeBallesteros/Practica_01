using System;
using System.Collections;
using System.Collections.Generic;
using Unity.Mathematics;
using UnityEngine;

public class TexasStarTarget : Hittable
{
    [SerializeField] public bool hit = false;
    [SerializeField] private Vector3 hitAngle;
    [SerializeField] private TexasStar manager;

    private void Start()
    {
        manager = GetComponentInParent<TexasStar>();
    }

    // Update is called once per frame
    void Update()
    {
        if (!hit) return;
        transform.localRotation = Quaternion.Lerp(transform.localRotation, Quaternion.Euler(hitAngle), .6f);
    }

    public override void OnHit()
    {
        hit = true;
        manager.hits++;
    }
}
