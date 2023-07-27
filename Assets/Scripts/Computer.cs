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
        concern = 0;
    }
}
