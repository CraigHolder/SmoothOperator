using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SecurityCamera : Damagable
{
    public GameObject head;

    public float maxRotation = 50;
    public float camSpeed = 10;
    float angle;
    bool right = true;

    public float visionRange = 10;
    public float visionAngle = 10;

    public Transform lens;
    public bool watchingPlayer;

    public LayerMask playerMask;
    public LayerMask concernMask;
    public LayerMask obstructionMask;

    public List<Computer> onlinePCs = new List<Computer>();
    public static List<SecurityCamera> allCams;

    void Awake()
    {
        if (allCams == null)
        {
            allCams = new List<SecurityCamera>();
        }
        allCams.Add(this);
    }
    // Start is called before the first frame update
    void Start()
    {
        for (int i = 0; i < Computer.allPCs.Count; i++)
        {
            if(Computer.allPCs[i].online)
            {
                onlinePCs.Add(Computer.allPCs[i]);
            }
        }
        //onlinePCs
        
    }

    // Update is called once per frame
    void LateUpdate()
    {
       if(health <= 0)
        {
            this.enabled = false;
            head.GetComponent<Rigidbody>().isKinematic = false;
            head.layer = 0;
            InfoDescription = "Destroyed";
        }
       else
        {
            FieldOfViewCheck();

            if(right)
            {
                head.transform.Rotate(0, camSpeed * Time.deltaTime,0);

                angle = head.transform.localRotation.eulerAngles.y;
                angle = (angle > 180) ? angle - 360 : angle;

                if (angle > maxRotation)
                {
                    right = false;
                    //head.transform.localRotation = Quaternion.Euler(0, maxRotation, 0);
                }
            }
            else
            {
                head.transform.Rotate(0, -camSpeed * Time.deltaTime, 0);

                angle = head.transform.localRotation.eulerAngles.y;
                angle = (angle > 180) ? angle - 360 : angle;

                if (angle < -maxRotation)
                {
                    right = true;
                    //head.transform.localRotation = Quaternion.Euler(0, -maxRotation, 0);
                }
            }
        }

       if(watchingPlayer && (Player.instance.armed || Player.instance.masked))
        {

        }
    }

    private void FieldOfViewCheck()
    {
        float visibleSus = 0;


        Collider[] rangeChecks = Physics.OverlapSphere(transform.position, visionRange, playerMask);
        Debug.DrawRay(lens.position, (Quaternion.AngleAxis(-(visionAngle / 2), Vector3.up) * lens.transform.forward) * visionRange, Color.blue);
        Debug.DrawRay(lens.position, (Quaternion.AngleAxis((visionAngle / 2), Vector3.up) * lens.transform.forward) * visionRange, Color.blue);
        if (rangeChecks.Length != 0)
        {
            Transform target = rangeChecks[0].transform;
            Vector3 directionToTarget = (target.position - lens.transform.position).normalized;

            if (Vector3.Angle(lens.transform.forward, directionToTarget) < visionAngle / 2)
            {
                float distanceToTarget = Vector3.Distance(lens.position, new Vector3(target.position.x, target.position.y + Random.Range(2, 6), target.position.z));
                Debug.DrawRay(lens.position, directionToTarget * (distanceToTarget), Color.green, 1f);
                if (!Physics.Raycast(lens.position, directionToTarget, distanceToTarget, obstructionMask))
                {
                    watchingPlayer = true;

                    if(Player.instance.armed)
                    {
                        visibleSus += 40 * Time.deltaTime;
                    }
                    if (Player.instance.masked)
                    {
                        visibleSus += 30 * Time.deltaTime;
                    }
                    if (Player.instance.tresspassing)
                    {
                        visibleSus += 30 * Time.deltaTime;
                    }
                    //lastSeenPos = Player.instance.transform.position;
                    //TO DO: if someone is watching a screen linked to the cameras all security gain the players lastSeenPos
                }
                else
                {
                    watchingPlayer = false;
                }

            }
            else
            {
                watchingPlayer = false;
            }

        }
        else if (watchingPlayer)
        {
            watchingPlayer = false;
        }

        //suspisous objects
        Collider[] rangeChecks2 = Physics.OverlapSphere(transform.position, visionRange, concernMask);
        for (int i = 0; i < rangeChecks2.Length; i++)
        {
            Transform target = rangeChecks2[i].transform;
            Vector3 directionToTarget = (target.position - lens.transform.position).normalized;

            if (Vector3.Angle(transform.forward, directionToTarget) < visionAngle / 2)
            {
                float distanceToTarget = Vector3.Distance(lens.position, new Vector3(target.position.x, target.position.y + Random.Range(2, 6), target.position.z));
                Debug.DrawRay(lens.position, directionToTarget * (distanceToTarget), Color.green, 0.1f);
                if (!Physics.Raycast(lens.position, directionToTarget, distanceToTarget, obstructionMask))
                {
                    visibleSus += 40 * Time.deltaTime;
                    //set computer screens linked to the camera to convern layer
                }

            }
        }

        for (int i = 0; i < onlinePCs.Count; i++)
        {
            onlinePCs[i].concern += visibleSus;
        }
    }
}
