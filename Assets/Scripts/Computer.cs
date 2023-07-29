using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Computer : Damagable
{
    public static List<Computer> allPCs;
    public bool online = true;
    public float concern = 0;
    // Start is called before the first frame update
    void Awake()
    {
        if(allPCs == null)
        {
            allPCs = new List<Computer>();
        }
        allPCs.Add(this);
    }

    // Update is called once per frame
    void Update()
    {
        if(health <= 0)
        {
            this.enabled = false;
            //head.GetComponent<Rigidbody>().isKinematic = false;
            gameObject.layer = 0;
            InfoDescription = "Destroyed";
            online = false;
            for (int i = 0; i < SecurityCamera.allCams.Count; i++)
            {
                SecurityCamera.allCams[i].onlinePCs.Remove(this);
            }

        }

        concern = 0;
    }
}
