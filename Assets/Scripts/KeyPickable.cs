using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class KeyPickable : MonoBehaviour,IPickable
{
    [SerializeField] Keys key;
    public void Pick()
    {
        GameManager.GetGameManager().SetKey(key);
        Destroy(this.gameObject);
    }
}
