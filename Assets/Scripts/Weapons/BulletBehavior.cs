using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BulletBehavior : MonoBehaviour
{
    public float velocity;
    public int damage;
    public GameObject decal;

    private float distance;
    // Start is called before the first frame update
    void Start()
    {
        Destroy(gameObject, 2f);
    }

    // Update is called once per frame
    void Update()
    {
        distance += velocity * Time.deltaTime;
        if (Physics.Raycast(transform.position, transform.forward, out RaycastHit hitInfo, distance)){
            hitInfo.transform.GetComponent<Hittable>()?.OnHit();
            // hitInfo.transform.GetComponent<IDamageable>()?.TakeDamage(gunData.damage);
            //var d = Instantiate(decal, hitInfo.point + (hitInfo.normal * 0.01f), Quaternion.FromToRotation(Vector3.up, hitInfo.normal));
            var d = Instantiate(decal, hitInfo.point - (hitInfo.normal * 0.001f), Quaternion.FromToRotation(Vector3.forward, hitInfo.normal));
            d.transform.parent = hitInfo.transform;
            Destroy(d, 5f);
            Destroy(gameObject);
        }

        transform.position += distance * transform.forward;
    }
}
