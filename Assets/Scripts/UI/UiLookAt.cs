using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UiLookAt : MonoBehaviour
{
    private void LateUpdate()
    {
        var direction = transform.position - Camera.main.transform.position;
        var lookRotation = Quaternion.LookRotation(direction);
        transform.rotation = lookRotation;
    }
}
