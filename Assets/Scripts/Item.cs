using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Item : MonoBehaviour
{
    public string ItemName;
    public string ItemDescription;
    public Utility.ItemType type;
    public int Value = 1;
    [Header("Use")]
    public int uses = 1;
    public int maxUses = 1;
    public int ExtraUses = 1;
    public float useDelay = 1;
    public float useTime = 0;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    public void Update()
    {
        useTime -= Time.deltaTime;
    }

    public virtual void Use()
    {
        uses--;
        useTime = useDelay;
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
