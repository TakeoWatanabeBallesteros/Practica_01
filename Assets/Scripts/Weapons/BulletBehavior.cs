using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BulletBehavior : MonoBehaviour
{
    public float velocity;
    public float damage;
    public PoolableObject decal;
    public Vector3 destination;
    [SerializeField] private LayerMask layerMask;

    private float distance;
    // Start is called before the first frame update
    void Start()
    {
        Destroy(gameObject, 2f);
        transform.forward = (destination-transform.position).normalized;
    }

    // Update is called once per frame
    void Update()
    {
        distance += velocity * Time.deltaTime;
        if (Physics.Raycast(transform.position, transform.forward, out RaycastHit hitInfo, distance, layerMask)){
            hitInfo.transform.GetComponent<Hittable>()?.OnHit();
            hitInfo.transform.GetComponent<IDamageable>()?.TakeDamage(damage);
            //var d = Instantiate(decal, hitInfo.point + (hitInfo.normal * 0.01f), Quaternion.FromToRotation(Vector3.up, hitInfo.normal));
            if(decal != null)
            {
                ObjectPool objectPool = ObjectPool.CreateInstance(decal, 30);
                DecalBehaviour poolableObject = (DecalBehaviour)objectPool.GetObject();

                poolableObject.transform.position = hitInfo.point - (hitInfo.normal * 0.001f);
                poolableObject.transform.rotation = Quaternion.FromToRotation(Vector3.forward, hitInfo.normal);
                
                poolableObject.SetFakeParent(hitInfo.transform);
            }
            Destroy(gameObject);
        }

        transform.position += distance * transform.forward;
    }
}
