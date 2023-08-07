using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Laser : MonoBehaviour
{
    LineRenderer LR;
    public LayerMask mask;
    public bool security = false;


    // Start is called before the first frame update
    void Awake()
    {
        LR = GetComponent<LineRenderer>();
    }

    // Update is called once per frame
    void Update()
    {
        RaycastHit hit;
        Physics.Raycast(transform.position, transform.right, out hit, 100, mask, QueryTriggerInteraction.Ignore);

        if (hit.collider != null)
        {
            LR.SetPosition(1, transform.InverseTransformPoint( hit.point));
        }
    }
}
