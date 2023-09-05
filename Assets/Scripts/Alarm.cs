using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Alarm : Damagable
{
    public bool noTrigger = true;
    public bool silentAlarm = false;
    float time = 0f;
    bool triggered = false;
    // Start is called before the first frame update
    void Start()
    {
        if(noTrigger)
        {
            LevelManager.instance.ActiveAlarms.Add(this);
        }
    }

    // Update is called once per frame
    void Update()
    {
        if(health <= 0)
        {
            if(!noTrigger)
            {
                if(!triggered)
                {
                    Activate();
                }
                else if(health > -20)
                {
                    time -= Time.deltaTime;
                    if (time <= 0)
                    {
                        time = 0.5f;
                        PlayAudio(10, Utility.audioType.Useing);
                    }
                }
                else
                {
                    LevelManager.instance.ActiveAlarms.Remove(this);
                    this.enabled = false;
                    GetComponent<Rigidbody>().isKinematic = false;
                    GetComponent<MeshCollider>().enabled = true;
                }
            }
            else
            {
                LevelManager.instance.ActiveAlarms.Remove(this);
                this.enabled = false;
                GetComponent<Rigidbody>().isKinematic = false;
                GetComponent<MeshCollider>().enabled = true;
            }
        }
        else
        {
            if(triggered)
            {
                time -= Time.deltaTime;
                if(time <= 0)
                {
                    time = 0.5f;
                    PlayAudio(10,Utility.audioType.Useing);
                }
            }
        }
    }
    public override void Activate()
    {
        if(triggered)
        {
            return;
        }
        for (int i = 0; i < LevelManager.instance.ActiveSecurity.Count; i++)
        {

            LevelManager.instance.ActiveSecurity[i].Alert();

        }
        MakeSound(100);
        triggered = true;
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
}
