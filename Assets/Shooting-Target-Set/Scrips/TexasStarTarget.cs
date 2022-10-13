using System.Collections;
using System.Collections.Generic;
using Unity.Mathematics;
using UnityEngine;

public class TexasStarTarget : Hittable
{
    [SerializeField] private bool hit = false;
    [SerializeField] private bool reset;
    [SerializeField] private Vector3 hitAngle;
    private Quaternion defaultAngle;
    
    // Start is called before the first frame update
    void Start()
    {
       defaultAngle = transform.localRotation;
    }

    // Update is called once per frame
    void Update()
    {
        if (hit)
        {
            transform.localRotation = Quaternion.Euler(hitAngle+defaultAngle.eulerAngles);
            hit = false;
        }
        if (!reset) return;
        transform.localRotation = defaultAngle;
        reset = false;
    }

    public override void OnHit()
    {
        hit = true;
    }
}
