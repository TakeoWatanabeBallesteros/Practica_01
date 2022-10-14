using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ShootingRangeTarget : Hittable
{
    [SerializeField] public bool hit = false;
    [SerializeField] private Vector3 hitAngle;
    [SerializeField] private float rotateDuration = 1f;
    private Quaternion defaultAngle;

    private void Start()
    {
        defaultAngle = transform.localRotation;
    }

    // Update is called once per frame
    void Update()
    {
        if (!hit) return;
        StartCoroutine(Hitted());
        hit = false;
    }

    public override void OnHit()
    {
        hit = true;
    }
    
    private IEnumerator Hitted()
    {
        float elapsedTime = 0f;

        while (elapsedTime < rotateDuration)
        {
            elapsedTime += Time.deltaTime;
            transform.localRotation = Quaternion.Lerp(transform.localRotation, Quaternion.Euler(hitAngle), elapsedTime/rotateDuration);
            yield return null;
        }
    }
    
    public IEnumerator Reset()
    {
        float elapsedTime = 0f;

        while (elapsedTime < rotateDuration)
        {
            elapsedTime += Time.deltaTime;
            transform.localRotation = Quaternion.Lerp(transform.localRotation, defaultAngle, elapsedTime/rotateDuration);
            yield return null;
        }
    }
}
