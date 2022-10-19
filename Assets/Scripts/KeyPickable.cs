using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class KeyPickable : MonoBehaviour,IPickable
{
    [SerializeField] int keyNumber;
    public void Pick()
    {
        GameManager.GetGameManager().SetKey(keyNumber);
        Destroy(this.gameObject);
    }
}
