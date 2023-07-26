using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Hurtbox : MonoBehaviour
{
    public StateMachine parent;
    public float multiplier = 1;


    // Start is called before the first frame update
    void Awake()
    {
        parent = GetComponentInParent<StateMachine>();
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
