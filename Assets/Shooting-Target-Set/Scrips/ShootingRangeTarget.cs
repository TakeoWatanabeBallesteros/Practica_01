using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ShootingRangeTarget : Hittable
{
    [SerializeField] private Animator animator;
    private int _hitAnim;
    private int _resetAnim;

    private void Start()
    {
        _hitAnim = Animator.StringToHash("Hit");
        _resetAnim = Animator.StringToHash("Reset");
    }

    public override void OnHit()
    {
        animator.SetTrigger(_hitAnim);
    }

    public void Reset()
    {
        animator.SetTrigger(_resetAnim);
    }
}
