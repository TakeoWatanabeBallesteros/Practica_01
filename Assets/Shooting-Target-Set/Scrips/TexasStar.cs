using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TexasStar : MonoBehaviour
{
    [SerializeField] private Transform rotationBase;
    [SerializeField] private float rotationSpeed = 1f;

    // Update is called once per frame
    void Update()
    {
        rotationBase.Rotate(0, 0, rotationSpeed);
    }

}
