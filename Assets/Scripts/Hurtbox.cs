using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Hurtbox : MonoBehaviour
{
    public Damagable parent;
    public float multiplier = 1;


    // Start is called before the first frame update
    void Awake()
    {
        parent = GetComponentInParent<Damagable>();
        //if(parent == null)
        //{
        //    parent = GetComponentInParent<SecurityCamera>();
        //    if (parent == null)
        //    {
        //        parent = GetComponentInParent<com>();
        //    }
        //}
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
