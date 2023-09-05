using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class Player : StateMachine
{
    public static Player instance;

    float jumpRemaining;
    float jumpAmount;
    public bool Climable = false;

    int money = 0;
    public bool tresspassing = false;
    public bool armed = false;
    public bool masked = false;

    CharacterController CC;

    bool spaceHeld = false;


    [Header("Inventory")]
    public Item held;
    public int selectedHotbar;
    public int hotBarSize = 5;
    public List<Item> hotBar;
    public LayerMask mask;
    public TMPro.TMP_Text infoText;
    public List<GameObject> hotbarOBJ;

    private void Awake()
    {
        instance = this;
    }

    // Start is called before the first frame update
    void Start()
    {
        animatorIK = animator.GetComponent<IKController>();
        hurtboxAnimatorIK = hurtboxAnimator.GetComponent<IKController>();
        animatorIK.ikActive = true;
        hurtboxAnimatorIK.ikActive = true;
        CC = GetComponent<CharacterController>();

        Audio = GetComponent<AudioSource>();

        for (int i = 0; i < hotBarSize; i++)
        {
            hotBar.Add(null);
        }
    }

    // Update is called once per frame
    void LateUpdate()
    {
        if(health <= 0)
        {
            Ragdoll();
            DropItem();
            Time.timeScale = 0.1f;
            Time.fixedDeltaTime = 0.02F * Time.timeScale;
            if (Input.GetKeyDown(KeyCode.E))
            {
                Time.timeScale = 1;
                Time.fixedDeltaTime = 0.02F;
                SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
            }
            return;
        }
        else if(health < 50)
        {
            healthLevel = Utility.healthLevel.Wounded;
        }

        Move();
        Inventory();

    }

    void Update()
    {

    }

    public void Inventory()
    {
        RaycastHit hit;
        Physics.Raycast(Camera.main.transform.position, Camera.main.transform.forward, out hit, 5,mask, QueryTriggerInteraction.Ignore);
        
        if(hit.collider != null)
        {
            //Debug.Log(hit.collider);
            Info info = hit.collider.GetComponentInParent<Info>();

            if (info != null)
            {
                infoText.text = info.InfoName + "\n" + info.InfoDescription;

                Item item = hit.collider.GetComponent<Item>();

                if (item != null)
                {
                    if (Input.GetKeyDown(KeyCode.E))
                    {
                        if(item.shop)
                        {
                            if (BaseManager.instance.BuyItem(item.Value))
                            {
                                DropItem();
                                hotbarOBJ[selectedHotbar].transform.GetChild(1).GetComponent<TMPro.TMP_Text>().text = item.InfoName;
                                item = Instantiate(item.gameObject, item.transform.position, item.transform.rotation).GetComponent<Item>();
                                switch (item.type)
                                {
                                    case Utility.ItemType.Equipment:
                                        hotBar[selectedHotbar] = item;
                                        item.transform.parent = transform;
                                        item.GetComponent<Rigidbody>().isKinematic = true;

                                        if (item.rHold != null)
                                        {
                                            animatorIK.rightHandObj = item.rHold.transform;
                                        }
                                        if (item.lHold != null)
                                        {
                                            animatorIK.leftHandObj = item.lHold.transform;
                                        }
                                        break;
                                    case Utility.ItemType.Gun:
                                        hotBar[selectedHotbar] = item;
                                        item.transform.parent = Camera.main.transform;
                                        item.GetComponent<Rigidbody>().isKinematic = true;
                                        item.transform.localPosition = new Vector3(0, -0.2f, 0.5f);
                                        item.transform.localRotation = Quaternion.Euler(0, -90, 7.4f);

                                        if (item.rHold != null)
                                        {
                                            animatorIK.rightHandObj = item.rHold.transform;
                                        }
                                        if (item.lHold != null)
                                        {
                                            animatorIK.leftHandObj = item.lHold.transform;
                                        }
                                        break;
                                    case Utility.ItemType.Melee:
                                        hotBar[selectedHotbar] = item;
                                        item.transform.parent = transform;
                                        item.GetComponent<Rigidbody>().isKinematic = true;

                                        if (item.rHold != null)
                                        {
                                            animatorIK.rightHandObj = item.rHold.transform;
                                        }
                                        if (item.lHold != null)
                                        {
                                            animatorIK.leftHandObj = item.lHold.transform;
                                        }

                                        break;
                                    case Utility.ItemType.Value:
                                        money += item.Value;
                                        Destroy(item.gameObject);

                                        break;
                                    case Utility.ItemType.Environment:
                                        item.Use();

                                        break;
                                }
                            }
                            else
                            {
                                PlayAudio(0,Utility.audioType.Useing);
                            }
                        }
                        else
                        {
                            DropItem();

                            hotbarOBJ[selectedHotbar].transform.GetChild(1).GetComponent<TMPro.TMP_Text>().text = item.InfoName;

                            switch (item.type)
                            {
                                case Utility.ItemType.Equipment:
                                    hotBar[selectedHotbar] = item;
                                    item.transform.parent = transform;
                                    item.GetComponent<Rigidbody>().isKinematic = true;

                                    if (item.rHold != null)
                                    {
                                        animatorIK.rightHandObj = item.rHold.transform;
                                    }
                                    if (item.lHold != null)
                                    {
                                        animatorIK.leftHandObj = item.lHold.transform;
                                    }
                                    break;
                                case Utility.ItemType.Gun:
                                    hotBar[selectedHotbar] = item;
                                    item.transform.parent = Camera.main.transform;
                                    item.GetComponent<Rigidbody>().isKinematic = true;
                                    item.transform.localPosition = new Vector3(0, -0.2f, 0.5f);
                                    item.transform.localRotation = Quaternion.Euler(0, -90, 0f);

                                    if (item.rHold != null)
                                    {
                                        animatorIK.rightHandObj = item.rHold.transform;
                                    }
                                    if (item.lHold != null)
                                    {
                                        animatorIK.leftHandObj = item.lHold.transform;
                                    }
                                    break;
                                case Utility.ItemType.Melee:
                                    hotBar[selectedHotbar] = item;
                                    item.transform.parent = transform;
                                    item.GetComponent<Rigidbody>().isKinematic = true;

                                    if (item.rHold != null)
                                    {
                                        animatorIK.rightHandObj = item.rHold.transform;
                                    }
                                    if (item.lHold != null)
                                    {
                                        animatorIK.leftHandObj = item.lHold.transform;
                                    }

                                    break;
                                case Utility.ItemType.Value:
                                    money += item.Value;
                                    Destroy(item.gameObject);

                                    break;
                                case Utility.ItemType.Environment:
                                    item.Use();

                                    break;
                            }
                        }
                    }
                }
                else
                {
                    Damagable damagable = hit.collider.GetComponent<Damagable>();
                    if(damagable != null)
                    {
                        if (Input.GetKeyDown(KeyCode.E))
                        {
                            damagable.Activate();
                        }
                    }

                }
            }
            else
            {
                infoText.text = "";
            }
        }
        else
        {
            infoText.text = "";
        }
        
        for (int i = 49; i < 58; i++)
        {
            if (Input.GetKey((KeyCode)i))
            {
                if(i - 49 < hotBar.Count)
                {
                    if(hotBar[selectedHotbar] != null)
                    {
                        hotBar[selectedHotbar].gameObject.SetActive(false);
                        animatorIK.leftHandObj = null;
                        animatorIK.rightHandObj = null;
                    }

                    selectedHotbar = i - 49;

                    for (int j = 0; j < hotbarOBJ.Count; j++)
                    {
                        hotbarOBJ[j].transform.GetChild(0).gameObject.SetActive(false);
                        
                    }
                    hotbarOBJ[selectedHotbar].transform.GetChild(0).gameObject.SetActive(true);
                    if (hotBar[selectedHotbar] != null)
                    {
                        hotBar[selectedHotbar].gameObject.SetActive(true);
                        if (hotBar[selectedHotbar].rHold != null)
                        {
                            animatorIK.rightHandObj = hotBar[selectedHotbar].rHold.transform;
                        }
                        if (hotBar[selectedHotbar].lHold != null)
                        {
                            animatorIK.leftHandObj = hotBar[selectedHotbar].lHold.transform;
                        }
                    }
                }
            }
        }

        if (hotBar[selectedHotbar] != null)
        {
           if( hotBar[selectedHotbar].type == Utility.ItemType.Gun || hotBar[selectedHotbar].type == Utility.ItemType.Melee)
            {
                armed = true;
            }
           else
            {
                armed = false;
            }
        }
        else
        {
            armed = false;
        }

        if (Input.GetMouseButton(0))
        {
            if(hotBar[selectedHotbar] != null)
            {
                if (hotBar[selectedHotbar].Use())
                {
                    if(hotBar[selectedHotbar].useClip != -1)
                    {
                        PlayAudio(hotBar[selectedHotbar].useClip, Utility.audioType.Useing);
                        MakeSound(hotBar[selectedHotbar].useVolume);
                    }

                } else if(hotBar[selectedHotbar].uses == 0)
                {
                    if (hotBar[selectedHotbar].emptyClip != -1 && Input.GetMouseButtonDown(0))
                    {
                        PlayAudio(hotBar[selectedHotbar].emptyClip, Utility.audioType.Useing);

                    }
                }
                

            }
        }
        if (Input.GetMouseButton(1))
        {
            if (hotBar[selectedHotbar] != null)
            {
                hotBar[selectedHotbar].transform.localPosition = hotBar[selectedHotbar].aimPos;//new Vector3(0, 0f, 0.5f);
            }
        }
        else
        {
            if (hotBar[selectedHotbar] != null)
            {
                hotBar[selectedHotbar].transform.localPosition = hotBar[selectedHotbar].holdPos;//new Vector3(0, 0.2f, 0.5f);
            }
        }

        if (Input.GetKeyDown(KeyCode.R))
        {
            if (hotBar[selectedHotbar].useTime <= 0 && hotBar[selectedHotbar] != null)
            {
                if(hotBar[selectedHotbar].type == Utility.ItemType.Gun)
                {
                    hotBar[selectedHotbar].Reload();
                    if (hotBar[selectedHotbar].reloadClip != -1)
                    {
                        PlayAudio(hotBar[selectedHotbar].reloadClip, Utility.audioType.Useing);

                    }
                }
            }
        }

        if(Input.GetKeyDown(KeyCode.Q))
        {
            DropItem();
        }
    }

    public override void DropItem()
    {
        if (hotBar[selectedHotbar] != null)
        {
            hotbarOBJ[selectedHotbar].transform.GetChild(1).GetComponent<TMPro.TMP_Text>().text = "Empty";
            hotBar[selectedHotbar].transform.parent = null;
            hotBar[selectedHotbar].transform.position += Vector3.up;
            hotBar[selectedHotbar].GetComponent<Rigidbody>().isKinematic = false;
            hotBar[selectedHotbar] = null;

            animatorIK.leftHandObj = null;
            animatorIK.rightHandObj = null;
        }
    }

    private void Move()
    {
        Vector3 movement = Vector3.zero;

        if (Input.GetKey(KeyCode.W))
        {
            movement += transform.forward;
            
        }
        if (Input.GetKey(KeyCode.S))
        {
            movement -= transform.forward;
        }
        if (Input.GetKey(KeyCode.A))
        {
            movement -= transform.right;
        }
        if (Input.GetKey(KeyCode.D))
        {
            movement += transform.right;
        }
        //0.08
        if (Input.GetKey(KeyCode.G))
        {
            Time.timeScale = 0;
        }

        if (Input.GetKey(KeyCode.LeftControl))
        {
            animator.SetBool("Crouch", true);
            hurtboxAnimator.SetBool("Crouch", true);
            Camera.main.transform.localPosition = new Vector3(0, 0.07f, 0.1f);
            CC.height = 0.8f;
            CC.radius = 0.3f;
            models.transform.localPosition = new Vector3(0, -0.482f, 0);
            models.transform.localScale = new Vector3(1, 0.75f, 1);

        }

        //Physics.Raycast(transform.position + (Vector3.up * 0.65f), Vector3.up, 1);

        else if(!Physics.Raycast(transform.position, Vector3.up, 2.2f, obstructionMask))
        {
            animator.SetBool("Crouch", false);
            hurtboxAnimator.SetBool("Crouch", false);
            Camera.main.transform.localPosition = new Vector3(0, 0.65f, 0f);
            CC.height = 1.75f;
            CC.radius = 0.3f;
            models.transform.localPosition = new Vector3(0, -1.06f, 0);
            models.transform.localScale = new Vector3(1, 1, 1);
        }

        if (movement.magnitude != 0 && CC.isGrounded)
        {
            moveAudioDelay -= Time.deltaTime * ((Ath + 2) / 2f);
            if(moveAudioDelay <= 0)
            {
                moveAudioDelay = 1.5f;//0.5f;
                PlayAudio(Random.Range(0, AudioUtility.walkSounds.Count), Utility.audioType.Walking);
                animator.SetFloat("Walk", 1);
                hurtboxAnimator.SetFloat("Walk", 1);
            }
                
            
        }
        else
        {

            animator.SetFloat("Walk", 0);
            hurtboxAnimator.SetFloat("Walk", 0);
        }

        //Base speed
        movement = movement.normalized;
        movement *= ((Ath + 2) / 2f);


        //Sprint
        if (Input.GetKey(KeyCode.LeftShift))
        {
            movement *= 2f;
        }
        float maxJump = 11.5f + Ath;
        //Jump
        if (Input.GetKeyDown(KeyCode.Space) && jumpRemaining <= 0)
        {
            jumpRemaining = 11.5f;
            jumpAmount = 11.5f;

            spaceHeld = true;
        }
        else
        {
            if (CC.isGrounded)
            {
                jumpRemaining = 0;
                jumpAmount = 0;
            }

            if (spaceHeld && (jumpAmount < maxJump || Climable))
            {
                jumpAmount += (Ath * Time.deltaTime) * 4;
                jumpRemaining += (Ath * Time.deltaTime) * 4;
            }

            if(spaceHeld && Climable)
            {
                jumpRemaining = 11.5f + (Ath /2);
            }
        }

        if(Input.GetKeyUp(KeyCode.Space))
        {
            spaceHeld = false;
        }
        
        

        //gravity
        movement += Vector3.down * 9.8f;
        if (jumpRemaining > 0)
        {
            movement += jumpRemaining * Vector3.up;
            jumpRemaining -= 9.8f * Time.deltaTime;
        }

        if(healthLevel == Utility.healthLevel.Wounded)
        {
            movement /= 2;
        }
        //final Move
        CC.Move(movement * Time.deltaTime);

        
    }

    private void OnControllerColliderHit(ControllerColliderHit hit)
    {
        if(health > 0)
        {
            Rigidbody otherRigidbody = hit.collider.attachedRigidbody;
            if (otherRigidbody != null && !otherRigidbody.isKinematic)
            {
                otherRigidbody.velocity += CC.velocity;
            }
        }
    }
    public override void Ragdoll()
    {
        animator.transform.parent = null;
        animator.enabled = false;
        animatorIK.ikActive = false;
        animator.GetComponentInChildren<SkinnedMeshRenderer>().updateWhenOffscreen = true;

        List<Transform> ragdoll = new List<Transform>();
        List<Transform> hurtbox = new List<Transform>();
        foreach (Transform g in animator.GetComponentsInChildren<Transform>())
        {
            if (g.name != "Jaw")
            {
                ragdoll.Add(g);
            }
        }
        foreach (Transform g in hurtboxAnimator.GetComponentsInChildren<Transform>())
        {
            hurtbox.Add(g);
        }
        for (int i = 0; i < ragdoll.Count; i++)
        {
            hurtbox[i].position = ragdoll[i].position;
            hurtbox[i].rotation = ragdoll[i].rotation;
        }

        //AIInfo.transform.position = ragdoll[3].position;
        //AIInfo.transform.rotation = ragdoll[3].rotation;
    }
}
