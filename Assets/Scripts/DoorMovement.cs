using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DoorMovement : MonoBehaviour
{
    BoxCollider boxTop;
    BoxCollider boxBot;
    Transform doorTop;
    Transform doorBot;
    float speed;
    float initialPositionY;
    float finalTopPositionY;
    float finalBotPositionY;
    public void Start() {
        speed = 5;
        doorTop = transform.GetChild(1);
        doorBot = transform.GetChild(0);
        boxTop = doorTop.GetComponent<BoxCollider>();
        boxBot = doorBot.GetComponent<BoxCollider>();
        initialPositionY = boxBot.bounds.center.y + boxBot.bounds.extents.y;
        finalTopPositionY = boxTop.bounds.center.y + boxTop.bounds.extents.y;
        finalBotPositionY = boxBot.bounds.center.y - boxBot.bounds.extents.y;
    }
    public void Open()
    {
        StopAllCoroutines();
        StartCoroutine(Up(doorTop,boxTop,-boxTop.bounds.extents.y,finalTopPositionY));
        StartCoroutine(Down(doorBot,boxBot,boxBot.bounds.extents.y,finalBotPositionY));
    }
    public void Close()
    {
        StopAllCoroutines();
        StartCoroutine(Down(doorTop,boxTop,-boxTop.bounds.extents.y,initialPositionY));
        StartCoroutine(Up(doorBot,boxBot,boxBot.bounds.extents.y,initialPositionY));
    }

    IEnumerator Up(Transform door, BoxCollider box, float extent, float desiredPosition)
    {
        while (box.bounds.center.y + extent < desiredPosition)
        {
            door.position += new Vector3(0,speed * Time.deltaTime,0);
            yield return null;
        }
    }
    IEnumerator Down(Transform door, BoxCollider box, float extent, float desiredPosition)
    {
        while (box.bounds.center.y + extent > desiredPosition)
        {
            door.position -= new Vector3(0,speed * Time.deltaTime,0);
            yield return null;
        }
    }
}
