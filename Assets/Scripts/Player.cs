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

    private void Awake()
    {
        instance = this;
    }

    // Start is called before the first frame update
    void Start()
    {
        CC = GetComponent<CharacterController>();
        for (int i = 0; i < hotBarSize; i++)
        {
            hotBar.Add(null);
        }
    }

    // Update is called once per frame
    void Update()
    {
        if(Input.GetKey(KeyCode.LeftAlt))
        {
            return;
        }
        Move();
        Inventory();

        if(health <= 0)
        {
            SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
        }
        if(health < 50)
        {
            healthLevel = Utility.healthLevel.Wounded;
        }

    }

    public void Inventory()
    {
        RaycastHit hit;
        Physics.Raycast(Camera.main.transform.position, Camera.main.transform.forward, out hit, 10,mask, QueryTriggerInteraction.Ignore);
        
        if(hit.collider != null)
        {
            Item item = hit.collider.GetComponent<Item>();
            if(item != null)
            {
                infoText.text = item.name + "\n" + item.ItemDescription;

                if(Input.GetKey(KeyCode.E))
                {
                    switch(item.type)
                    {
                        case Utility.ItemType.Equipment:
                            hotBar[selectedHotbar] = item;
                            item.transform.parent = transform;
                            item.GetComponent<Rigidbody>().isKinematic = true;

                            break;
                        case Utility.ItemType.Gun:
                            hotBar[selectedHotbar] = item;
                            item.transform.parent = Camera.main.transform;
                            item.GetComponent<Rigidbody>().isKinematic = true;
                            item.transform.localPosition = new Vector3(0,-0.2f,0.5f);
                            item.transform.localRotation = Quaternion.Euler(0,-90,7.4f);
                            break;
                        case Utility.ItemType.Melee:
                            hotBar[selectedHotbar] = item;
                            item.transform.parent = transform;
                            item.GetComponent<Rigidbody>().isKinematic = true;

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
                    selectedHotbar = i - 49;
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

        if (Input.GetMouseButton(0))
        {
            if(hotBar[selectedHotbar] != null)
            {
                
                    hotBar[selectedHotbar].Use();
            }
        }
        if (Input.GetMouseButton(1))
        {
            if (hotBar[selectedHotbar] != null)
            {
                hotBar[selectedHotbar].transform.localPosition = new Vector3(0, 0f, 0.5f);
            }
        }
        else
        {
            if (hotBar[selectedHotbar] != null)
            {
                hotBar[selectedHotbar].transform.localPosition = new Vector3(0, -0.2f, 0.5f);
            }
        }

        if (Input.GetKey(KeyCode.R))
        {
            if (hotBar[selectedHotbar] != null)
            {
                if(hotBar[selectedHotbar].type == Utility.ItemType.Gun)
                {
                    hotBar[selectedHotbar].Reload();
                }
            }
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
            CC.height = 2;
            CC.radius = 0.5f;
            models.transform.localPosition = new Vector3(0, -1.06f, 0);
            models.transform.localScale = new Vector3(1, 1, 1);
        }

        if (movement.magnitude != 0)
        {
            animator.SetFloat("Walk", 1);
            hurtboxAnimator.SetFloat("Walk", 1);
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
}