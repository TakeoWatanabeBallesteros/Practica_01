using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PoperTarget : Hittable
{
    [SerializeField] private bool hit = false;
    [SerializeField] private Vector3 hitAngle;
    private Quaternion defaultAngles;

    private void Start()
    {
        defaultAngles = transform.localRotation;
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
    }

    public void Reset()
    {
        hit = false;
        transform.localRotation = defaultAngles;
    }
}
