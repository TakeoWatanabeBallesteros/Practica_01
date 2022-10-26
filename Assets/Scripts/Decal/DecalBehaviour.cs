using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DecalBehaviour : PoolableObject
{
    public Transform FakeParent;//Remember to assign the parent transform 
    private Vector3 pos, fw, up;
    
    private void OnEnable()
    {
        StartCoroutine(_OnEnable());
    }
    
    public override void OnDisable()
    {
        Parent.ReturnObjectToPool(this);
        base.OnDisable();
    }

    private IEnumerator _OnEnable()
    {
        yield return new WaitForSeconds(4.0f);
        gameObject.SetActive(false);
    }
    
    private void Update()
    {
        var newpos = FakeParent.transform.TransformPoint(pos);
        var newfw = FakeParent.transform.TransformDirection(fw);
        var newup = FakeParent.transform.TransformDirection(up);
        var newrot = Quaternion.LookRotation(newfw, newup);
        transform.position = newpos;
        transform.rotation = newrot;
    }
 
    public void SetFakeParent(Transform Parent)
    {
        FakeParent = Parent;
        //Offset vector
        pos = Parent.transform.InverseTransformPoint(transform.position);
        fw = Parent.transform.InverseTransformDirection(transform.forward);
        up = Parent.transform.InverseTransformDirection(transform.up);
    }
}
