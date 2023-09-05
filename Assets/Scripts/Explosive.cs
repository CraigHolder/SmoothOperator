using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Explosive : Damagable
{
    public bool armed = false;
    public Transform explodePoint;
    public float damage = 50f;
    public float radius = 1f;
    public float noise = 500f;
    public float angle = 360f;
    public LayerMask obstructionMask;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        Debug.DrawRay(explodePoint.position, (Quaternion.AngleAxis(-(angle / 2), Vector3.up) * explodePoint.forward) * radius, Color.blue);
        Debug.DrawRay(explodePoint.position, (Quaternion.AngleAxis((angle / 2), Vector3.up) * explodePoint.forward) * radius, Color.blue);

        Debug.DrawRay(explodePoint.position, (Quaternion.AngleAxis(-(angle / 2), Vector3.right) * explodePoint.forward) * radius, Color.blue);
        Debug.DrawRay(explodePoint.position, (Quaternion.AngleAxis((angle / 2), Vector3.right) * explodePoint.forward) * radius, Color.blue);
        if (health <= 0)
        {
            if(armed)
            {
                Activate();
            }
            else
            {
                Destroy(gameObject);
            }
        }
    }


    public void PlayAudio(int index, Utility.audioType type)
    {
        AudioSource temp = Instantiate(Resources.Load<GameObject>("AudioNode"), transform.position, transform.rotation).GetComponent<AudioSource>();
        temp.transform.parent = AudioUtility.instance.transform;
        switch (type)
        {
            case Utility.audioType.Talking:
                temp.clip = AudioUtility.talkSounds[index];
                temp.Play();

                break;
            case Utility.audioType.Walking:
                temp.clip = AudioUtility.walkSounds[index];
                temp.Play();

                break;
            case Utility.audioType.Useing:
                temp.clip = AudioUtility.useSounds[index];
                temp.Play();

                break;
            default:
                break;
        }

        //Audio.Play();

    }



    public override void Activate()
    {
        if(armed)
        {
            MakeSound(noise);
            PlayAudio(1, Utility.audioType.Useing);

            Collider[] rangeChecks2 = Physics.OverlapSphere(transform.position, radius);
            for (int i = 0; i < rangeChecks2.Length; i++)
            {
                Transform target = rangeChecks2[i].transform;
                Vector3 directionToTarget = (target.position - transform.position).normalized;
                if (Vector3.Angle(transform.forward, directionToTarget) < angle / 2)
                {
                    float distanceToTarget = Vector3.Distance(explodePoint.position, new Vector3(target.position.x, target.position.y + Random.Range(2, 6), target.position.z));
                    Debug.DrawRay(explodePoint.position, directionToTarget * (distanceToTarget), Color.cyan, 0.1f);
                    if (!Physics.Raycast(explodePoint.position, directionToTarget, distanceToTarget, obstructionMask))
                    {
                        Hurtbox hurt = target.GetComponent<Hurtbox>();
                        if (hurt != null)
                        {
                            hurt.parent.health -= damage;
                        }
                        //visibleSus += 40 * Time.deltaTime;
                        //set computer screens linked to the camera to convern layer
                    }
                }
            }
            Destroy(gameObject);
        }
    }

    public override void DeActivate()
    {
        armed = false;
    }

    public void MakeSound(float volume)
    {
        Collider[] rangeChecks = Physics.OverlapSphere(transform.position, volume);

        for (int i = 0; i < rangeChecks.Length; i++)
        {
            if (rangeChecks[i].tag == "AI")
            {
                StateMachine ai = rangeChecks[i].GetComponent<StateMachine>();
                ai.Hear(transform.position, volume);
            }
        }
    }
}
