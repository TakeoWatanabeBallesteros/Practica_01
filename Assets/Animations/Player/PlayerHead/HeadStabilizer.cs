using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HeadStabilizer : MonoBehaviour
{
    [SerializeField] private Transform camStady;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        //transform.LookAt(FocusTarget());
    }

    private Vector3 FocusTarget()
    {
        Vector3 pos = new Vector3(transform.position.x, transform.position.y + camStady.localPosition.y,
            transform.position.z);
        return pos + camStady.forward * 15.0f;
    }
}
