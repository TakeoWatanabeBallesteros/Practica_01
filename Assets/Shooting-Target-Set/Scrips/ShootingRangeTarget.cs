using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ShootingRangeTarget : Hittable
{
    [SerializeField] private Animator animator;
    private int _hitAnim;
    private int _resetAnim;
    private bool hited;
    
    public delegate void HitPoint(int points);

    public static event HitPoint OnHitPoint;

    private void Start()
    {
        _hitAnim = Animator.StringToHash("Hit");
        _resetAnim = Animator.StringToHash("Reset");
    }

    public override void OnHit()
    {
        animator.SetTrigger(_hitAnim);
        if(!hited)OnHitPoint?.Invoke(1);
        hited = true;
    }

    public void Reset()
    {
        animator.SetTrigger(_resetAnim);
        hited = false;
    }
}
