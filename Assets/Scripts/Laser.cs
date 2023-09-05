using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Laser : MonoBehaviour
{
    LineRenderer LR;
    public LayerMask mask;
    public Damagable connected;
    public float range = 100f;
    public bool playerControl = true;
    public bool triggered = false;


    // Start is called before the first frame update
    void Awake()
    {
        LR = GetComponent<LineRenderer>();
    }

    // Update is called once per frame
    void Update()
    {
        RaycastHit hit;
        Physics.Raycast(transform.position, transform.right, out hit, range, mask, QueryTriggerInteraction.Ignore);

        if (hit.collider != null)
        {
            LR.SetPosition(1, transform.InverseTransformPoint(hit.point));
            
            if(hit.collider.gameObject.layer != 0 && hit.collider.gameObject.layer != 8)
            {
                if(playerControl)
                {
                        triggered = true;
                        if (connected != null)
                        {
                            connected.Activate();
                            Destroy(gameObject);
                        }
                }
                else
                {
                    if (hit.collider.name != "AIInfo" && hit.transform.root.tag != "AI")
                    {
                            if (connected != null)
                            {
                                triggered = true;
                                connected.Activate();
                                Destroy(gameObject);
                            }
                    }
                }
            }
        }
        else
        {
            LR.SetPosition(1, -transform.forward * range);
        }
    }
}
