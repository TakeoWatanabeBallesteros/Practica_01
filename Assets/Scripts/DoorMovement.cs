using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(BoxCollider))]
public class DoorMovement : MonoBehaviour
{
    
    float halfHeight;
    BoxCollider box;
    float speed;
    float finalHeight;
    float initialHeight;
    public void Start() {
        speed = 5;
        box = GetComponent<BoxCollider>();
        halfHeight = box.bounds.extents.y;
        finalHeight = box.bounds.center.y + halfHeight;
        initialHeight = box.bounds.center.y - halfHeight;
    }
    public void Open()
    {
        StopAllCoroutines();
        StartCoroutine(Opening());
    }
    public void Close()
    {
        StopAllCoroutines();
        StartCoroutine(Closing());
    }

    IEnumerator Opening()
    {
        while (box.bounds.center.y - halfHeight < finalHeight)
        {
            transform.position += new Vector3(0,speed * Time.deltaTime,0);
            yield return null;
        }
    }
    IEnumerator Closing()
    {
        while (box.bounds.center.y - halfHeight > initialHeight)
        {
            transform.position -= new Vector3(0,speed * Time.deltaTime,0);
            yield return null;
        }
    }
}
