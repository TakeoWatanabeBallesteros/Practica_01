using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DroppedKey : DroppedBase
{
    [SerializeField] int keyNumber;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    protected override void WhenPicked()
    {
        GameManager.GetGameManager().SetKey(keyNumber);
    }
}
