using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Gun : Item
{
    public LayerMask mask;
    public float damage = 10;
    public GameObject tip;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    //void Update()
    //{
    //    base.Update();
    //}

    public override void Use()
    {
        if (useTime <= 0)
        {
            if (uses > 0)
            {
                base.Use();
                useTime = useDelay;
                RaycastHit hit;
                Physics.Raycast(tip.transform.position, transform.right, out hit, 1000, mask, QueryTriggerInteraction.Ignore);
                Debug.DrawRay(tip.transform.position, transform.right * 1000, Color.red, 10);

                if (hit.collider != null)
                {
                    Debug.Log("pew" + hit.collider);

                    Hurtbox hurt = hit.collider.GetComponent<Hurtbox>();
                    if (hurt != null)
                    {
                        hurt.parent.health -= damage * hurt.multiplier;
                    }
                }
            }
        }
    }

}
