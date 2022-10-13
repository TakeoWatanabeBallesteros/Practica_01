using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlatformMover : MonoBehaviour
{
    [SerializeField] Transform pointA;
    [SerializeField] Transform pointB;
    [SerializeField] float speed;
    [SerializeField] float secondsStopped;
    Vector2 aPos;
    Vector2 bPos;
    Vector3 dirToA;
    Vector3 dirToB;
    [SerializeField] Transform player;
    float distanceAB;
    // Start is called before the first frame update
    void Start()
    {
        aPos = new Vector2(pointA.position.x,pointA.position.z);
        bPos = new Vector2(pointB.position.x,pointB.position.z);
        dirToA = pointA.position - pointB.position;
        dirToB = pointB.position - pointA.position;
        transform.position = pointA.position;
        distanceAB = Vector2.Distance(aPos,bPos);
        StartCoroutine(GoToB());
    }

    IEnumerator GoToB()
    {
        while(Vector2.Distance(new Vector2(transform.position.x,transform.position.z),aPos) < distanceAB)
        {
            transform.position += dirToB * speed * Time.deltaTime;
            if(player != null)
            {
                player.position += dirToB * speed * Time.deltaTime;
            }
            yield return null;
        }

        transform.position = pointB.position;
        yield return new WaitForSeconds(secondsStopped);
        StartCoroutine(GoToA());
    }

    IEnumerator GoToA()
    {
        while(Vector2.Distance(new Vector2(transform.position.x,transform.position.z),bPos) < distanceAB)
        {
            transform.position += dirToA * speed * Time.deltaTime;
            if(player != null)
            {
                player.position += dirToA * speed * Time.deltaTime;
            }
            yield return null;
        }

        transform.position = pointA.position;
        yield return new WaitForSeconds(secondsStopped);
        StartCoroutine(GoToB());
    }

    private void OnCollisionEnter(Collision other) {
        Debug.Log("collided");
        if(other.gameObject.layer == LayerMask.NameToLayer("Player"))
        {
            Debug.Log("PlayerDetected");
            player = other.transform;
        }
    }
    private void OnCollisionExit(Collision other) {
        if(other.gameObject.layer == LayerMask.NameToLayer("Player"))
        {
            player = null;
        }
    }
}
