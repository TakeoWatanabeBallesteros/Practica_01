using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ShootingRangeTarget : Hittable
{
    [SerializeField] private Animator _animator;

    private void Start()
    {
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public override void OnHit()
    {
    }
    
    private void Hitted()
    {
    }
    
    public void Reset()
    {
    }
}
