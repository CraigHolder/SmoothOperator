using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BaseManager : MonoBehaviour
{
    public static BaseManager instance;

    public float cash = 0;
    public float reputation = 0;

    private void Awake()
    {
        instance = this;
    }

    // Start is called before the first frame update
    void Start()
    {
        cash = PlayerPrefs.GetFloat("Cash", cash);
        reputation = PlayerPrefs.GetFloat("Rep");
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public bool BuyItem(float cost)
    {
        if(cash >= cost)
        {
            cash -= cost;
            return true;
        }

        return false;
    }
}
