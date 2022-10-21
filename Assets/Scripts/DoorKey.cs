using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DoorKey : Door
{
    [SerializeField] Keys key;
    public override void Open()
    {
        if(GameManager.GetGameManager().GetKey(key))base.Open();
        
    }
    public override void Close()
    {
        if(GameManager.GetGameManager().GetKey(key))base.Close();
    }
}
