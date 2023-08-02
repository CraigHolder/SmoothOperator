using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Door : Damagable
{
    Rigidbody RB;

    // Start is called before the first frame update
    void Start()
    {
        RB = GetComponent<Rigidbody>();
    }
    public override void Activate()
    {
        base.Activate();
        RB.isKinematic = !RB.isKinematic;
        
    }
    // Update is called once per frame
    void Update()
    {
        if (RB.isKinematic)
        {
            if((int)transform.rotation.eulerAngles.y != 0)
            {
                if (transform.rotation.eulerAngles.y < 180)
                {
                    transform.Rotate(0, -200 * Time.deltaTime, 0);
                }
                else if (transform.rotation.eulerAngles.y > 180)
                {
                    transform.Rotate(0, 200 * Time.deltaTime, 0);
                }
            }
            
            
            //transform.Rotate();
        }







        //if(Vector3.Distance(Player.instance.transform.position, transform.position) < 5)
        //{
        //    RaycastHit hit;
        //    Physics.Raycast(Camera.main.transform.position, Camera.main.transform.forward, out hit, 5, mask, QueryTriggerInteraction.Ignore);

        //    if (hit.collider == collider)
        //    {
        //        if (Input.GetKeyDown(KeyCode.F))
        //        {
        //            grabbed = true;
        //        }
        //        Debug.Log(hit.collider + "DOOR");
        //    }

        //    if (Input.GetKeyUp(KeyCode.F))
        //    {
        //        grabbed = false;
        //    }

        //    if (grabbed)
        //    {
        //        Player.instance.usingDoor = true;
        //        if(Input.GetKey(KeyCode.W))
        //        {
        //            if(rot > -90)
        //            {
        //                transform.Rotate(0, -2 * Time.deltaTime, 0);
        //                rot -= 2 * Time.deltaTime;
        //            }
        //        }
        //        if (Input.GetKey(KeyCode.S))
        //        {
        //            if (rot < 90)
        //            {
        //                transform.Rotate(0, 2 * Time.deltaTime, 0);
        //                rot += 2 * Time.deltaTime;
        //            }
        //        }
        //    }
        //    else
        //    {
        //        Player.instance.usingDoor = false;
        //    }
        //}

    }
}
