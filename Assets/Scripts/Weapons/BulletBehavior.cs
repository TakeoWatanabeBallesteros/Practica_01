using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BulletBehavior : MonoBehaviour
{
    public float velocity;
    public int damage;

    private float distance;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        distance += velocity * Time.deltaTime;
        if (Physics.Raycast(transform.position, transform.forward, out RaycastHit hitInfo, distance)){
            hitInfo.transform.GetComponent<Hittable>()?.OnHit();
            // hitInfo.transform.GetComponent<IDamageable>()?.TakeDamage(gunData.damage);
            Destroy(gameObject);
        }

        transform.position += distance * transform.forward;
    }
}
