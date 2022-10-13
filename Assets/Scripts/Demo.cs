using UnityEngine;

public class Demo : Hittable
{
    public override void OnHit()
    {
        Debug.Log("Done");
    }
}