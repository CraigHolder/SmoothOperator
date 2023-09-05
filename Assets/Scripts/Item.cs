using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Item : Info
{
    public Utility.ItemType type;
    public int Value = 1;
    public GameObject rHold;
    public GameObject lHold;
    public bool shop = false;
    public Vector3 holdPos = Vector3.zero;
    public Vector3 aimPos = Vector3.zero;

    [Header("Use")]
    public int uses = 1;
    public int maxUses = 1;
    public int ExtraUses = 1;
    public float useDelay = 1;
    public float useTime = 0;

    [Header("Sound")]
    public float useVolume = 1;
    public int useClip = -1;
    public int reloadClip = 7;
    public int emptyClip = 0;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    public void Update()
    {
        useTime -= Time.deltaTime;
    }

    public virtual bool Use()
    {
        uses--;
        useTime = useDelay;
        return true;
    }
    public virtual void AltUse()
    {
        
    }
    public virtual void Reload()
    {
        useTime = useDelay * 3;
        int ammoNeeded = uses - maxUses;
        ExtraUses += ammoNeeded;
        if(ExtraUses < 0)
        {
            uses = maxUses + ExtraUses;
            ExtraUses = 0;
        }
        else
        {
            uses = maxUses;
        }
    }
}
