using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class StateMachine : Damagable
{
    public Utility.Factions faction;
    public Utility.Attitude attitude;
    public Utility.healthLevel healthLevel;
    public Utility.Suspicion suspicion;
    public List<Utility.StatusEffects> statusEffects;
    public List<float> statusEffectLength;
    public bool male = true;

    public float sus = 0;
    public float speed = 0;

    public Animator animator;
    public Animator hurtboxAnimator;
    public IKController animatorIK;
    public IKController hurtboxAnimatorIK;
    public GameObject models;

    [Header("Audio")]
    protected AudioSource Audio;
    [SerializeField]
    AudioSource walkAudio;
    int lastClip = -1;
    Utility.audioType lastAduioType = Utility.audioType.Useing;
    protected float moveAudioDelay = 0.5f;

    [Header("Skills")]
    public int CQC = 1;
    public int Lurk = 1;
    public int Acc = 1;
    public int Ath = 5;
    public int SC = 1;
    public int Hack = 1;

    [Header("AI")]
    public bool watchingPlayer = false;
    NavMeshAgent nav;
    public Vector3 goalPos;
    public List<GameObject> pathPoints = new List<GameObject>();
    int currentPathPoint = 0;
    float pathPause = 0;
    float pathTurnTime = 0f;
    float pathTurnAmount = 0f;
    bool pathTurnRight = false;
    public Transform eyes;
    public float visionRange = 10;
    public float visionAngle = 10;
    public LayerMask playerMask;
    public LayerMask concernMask;
    public LayerMask obstructionMask;
    Vector3 lastSeenPos;
    public GameObject AIInfo;
    float reactionTime = 1;
    public bool wanders = false;

    bool talking = false;
    public float talkDuration = 0;
    public GameObject talkPartner;


    [Header("AIInventory")]
    public Item AIHeld;
    public List<Item> AIItems;


    // Start is called before the first frame update
    void Start()
    {
        animatorIK = animator.GetComponent<IKController>();
        hurtboxAnimatorIK = hurtboxAnimator.GetComponent<IKController>();

        nav = GetComponent<NavMeshAgent>();
        nav.avoidancePriority = 50;
        //nav.avoidancePriority = LevelManager.currAvoidVal;
        //LevelManager.currAvoidVal++;

        AIInfo.transform.parent = LevelManager.instance.AIInfos.transform;
        AIInfo.GetComponent<Info>().InfoName = InfoName;
        AIInfo.GetComponent<Info>().InfoDescription = InfoDescription;

        Audio = GetComponent<AudioSource>();

        if(pathPoints.Count <= 0)
        {
            switch (faction)
            {
                case Utility.Factions.Civilian:
                    LevelManager.instance.ActiveCivilians.Add(this);

                    int pointAmount = Random.Range(2, 10);
                    for (int i = 0; i < pointAmount; i++)
                    {
                        pathPoints.Add(LevelManager.instance.pathPoints[Random.Range(0, LevelManager.instance.pathPoints.Count)]);
                    }


                    break;
                case Utility.Factions.Security:
                    LevelManager.instance.ActiveSecurity.Add(this);

                    int pointAmount2 = Random.Range(2, 10);
                    for (int i = 0; i < pointAmount2; i++)
                    {
                        pathPoints.Add(LevelManager.instance.patrolPoints[Random.Range(0, LevelManager.instance.patrolPoints.Count)]);
                    }
                    break;
                case Utility.Factions.Criminal:
                    LevelManager.instance.ActiveCriminals.Add(this);
                    break;
                case Utility.Factions.Operator:
                    LevelManager.instance.ActiveOperators.Add(this);
                    break;
            }
        }
        

        if(AIHeld != null)
        {
            animatorIK.ikActive = true;
            hurtboxAnimatorIK.ikActive = true;
            if (AIHeld.rHold != null)
            {
                animatorIK.rightHandObj = AIHeld.rHold.transform;
            }
            if (AIHeld.lHold != null)
            {
                animatorIK.leftHandObj = AIHeld.lHold.transform;
            }
        }
    }

    // Update is called once per frame
    void LateUpdate()
    {
        speed = ((Ath + 2f) / 2f);

        moveAudioDelay -= Time.deltaTime * speed;
        if(moveAudioDelay <= 0)
        {
            moveAudioDelay = 1.5f;//0.5f;
            PlayAudio(Random.Range(0, AudioUtility.walkSounds.Count), Utility.audioType.Walking);
        }

        if (healthLevel == Utility.healthLevel.Dead || !gameObject.activeSelf)
        {
            Ragdoll();
            return;
        }

        if(health < 50)
        {
            healthLevel = Utility.healthLevel.Wounded;
            attitude = Utility.Attitude.Afraid;
            speed /= 2;
            AIInfo.GetComponent<Info>().InfoDescription = "Wounded";

            if (health < 10)
            {
                healthLevel = Utility.healthLevel.Unconsious;
                AIInfo.GetComponent<Info>().InfoDescription = "Unconsious";
                Ragdoll();
                DropItem();
                Path(transform.position);

                switch (faction)
                {
                    case Utility.Factions.Civilian:

                            LevelManager.instance.UnconsiousCivilians.Add(this);
                            LevelManager.instance.ActiveCivilians.Remove(this);

                        break;
                    case Utility.Factions.Security:
                        
                            LevelManager.instance.UnconsiousSecurity.Add(this);
                            LevelManager.instance.ActiveSecurity.Remove(this);

                        break;
                    case Utility.Factions.Criminal:
                            LevelManager.instance.UnconsiousCriminals.Add(this);
                            LevelManager.instance.ActiveCriminals.Remove(this);

                        break;
                    case Utility.Factions.Operator:
                            LevelManager.instance.UnconsiousOperators.Add(this);
                            LevelManager.instance.ActiveOperators.Remove(this);

                        break;
                }
                if (health < 0)
                {
                    healthLevel = Utility.healthLevel.Dead;
                    AIInfo.GetComponent<Info>().InfoDescription = "Dead";
                    Path(transform.position);

                    switch (faction)
                    {
                        case Utility.Factions.Civilian:

                            if(LevelManager.instance.UnconsiousCivilians.Contains(this))
                            {
                                LevelManager.instance.UnconsiousCivilians.Remove(this);
                            }
                            else if(LevelManager.instance.ActiveCivilians.Contains(this))
                            {
                                LevelManager.instance.ActiveCivilians.Remove(this);
                            }
                            
                            LevelManager.instance.DeadCivilians.Add(this);
                            break;
                        case Utility.Factions.Security:
                            if (LevelManager.instance.UnconsiousSecurity.Contains(this))
                            {
                                LevelManager.instance.UnconsiousSecurity.Remove(this);
                            }
                            else if (LevelManager.instance.ActiveSecurity.Contains(this))
                            {
                                LevelManager.instance.ActiveSecurity.Remove(this);
                            }

                            LevelManager.instance.DeadSecurity.Add(this);
                            break;
                        case Utility.Factions.Criminal:
                            if (LevelManager.instance.UnconsiousCriminals.Contains(this))
                            {
                                LevelManager.instance.UnconsiousCriminals.Remove(this);
                            }
                            else if (LevelManager.instance.ActiveCriminals.Contains(this))
                            {
                                LevelManager.instance.ActiveCriminals.Remove(this);
                            }

                            LevelManager.instance.DeadCriminals.Add(this);
                            break;
                        case Utility.Factions.Operator:
                            if (LevelManager.instance.UnconsiousOperators.Contains(this))
                            {
                                LevelManager.instance.UnconsiousOperators.Remove(this);
                            }
                            else if (LevelManager.instance.ActiveOperators.Contains(this))
                            {
                                LevelManager.instance.ActiveOperators.Remove(this);
                            }

                            LevelManager.instance.DeadOperators.Add(this);
                            break;
                    }
                    return;
                }
            }
        }

        if(healthLevel == Utility.healthLevel.Unconsious)
        {
            return;
        }

        Path();
        AIInfo.transform.position = transform.position;

        animator.SetFloat("Walk", 1);
        hurtboxAnimator.SetFloat("Walk", 1);
        nav.avoidancePriority = 50;
        switch (faction)
        {
            case Utility.Factions.Civilian:
                CivilianBrain();
                break;
            case Utility.Factions.Security:
                SecurityBrain();
                break;
            case Utility.Factions.Criminal:

                break;
            case Utility.Factions.Operator:

                break;
        }

    }

    public void FollowPath()
    {
        if (wanders && pathPause <= 0)
        {
            if (!nav.hasPath)
            {
                goalPos = pathPoints[currentPathPoint].transform.position;

                if (Vector3.Distance(pathPoints[currentPathPoint].transform.position, transform.position) < 3)
                {
                    currentPathPoint++;
                    pathPause = Random.Range(0.5f, 10);
                    if (currentPathPoint >= pathPoints.Count)
                    {
                        currentPathPoint = 0;
                    }
                }
            }
            else
            {
                if (Vector3.Distance(pathPoints[currentPathPoint].transform.position, transform.position) < 3)
                {
                    currentPathPoint++;
                    pathPause = Random.Range(0.5f, 10);
                    if (currentPathPoint >= pathPoints.Count)
                    {
                        currentPathPoint = 0;
                    }
                }
            }
        }
        else
        {
            pathPause -= Time.deltaTime;
            if(!nav.hasPath)
            {
                animator.SetFloat("Walk", 0);
                hurtboxAnimator.SetFloat("Walk", 0);
                nav.avoidancePriority = 0;

                if(sus > 50)
                {
                    float rot = 0;
                    
                    pathTurnTime += Time.deltaTime;
                    if(pathTurnTime > 4)
                    {
                        pathTurnAmount = Random.Range(0f,220f);
                        pathTurnTime = 0;
                        if(Random.Range(0, 2) == 0)
                        {
                            pathTurnRight = true;
                        }
                        else
                        {
                            pathTurnRight = false;
                        }

                    }

                    if(pathTurnRight)
                    {
                        transform.Rotate(0, (pathTurnAmount / 4f) * Time.deltaTime, 0);
                    }
                    else
                    {
                        transform.Rotate(0, -(pathTurnAmount / 4f) * Time.deltaTime, 0);
                    }

                    //    rot = Mathf.Lerp(transform.rotation.eulerAngles.y, pathTurnAmount, (pathTurnTime / 5));
                    //if(pathTurnRight)
                    //{
                    //    transform.rotation = Quaternion.Euler(0, rot, 0);
                    //}
                    //else
                    //{
                    //    transform.rotation = Quaternion.Euler(0, -rot, 0);
                    //}
                        
                }
            }
            
        }
    }

    public void TalkCheck()
    {
        if(talking)
        {
            talkDuration -= Time.deltaTime;
            if(talkPartner)
            {
                //Vector3 directionToTarget = (talkPartner.transform.position - eyes.transform.position).normalized;
                //Vector3 directionToTarget = (eyes.transform.position - talkPartner.transform.position).normalized;

                goalPos = transform.position;//talkPartner.transform.position + (directionToTarget * 2);
                transform.LookAt(talkPartner.transform);
                animator.SetFloat("Walk", 0);
                hurtboxAnimator.SetFloat("Walk", 0);
                nav.avoidancePriority = 0;
                //goalPos = talkPartner.transform.position;
            }
            if(talkDuration <=0)
            {
                talking = false;
                talkPartner = null;
            }
            else
            {
                if(Random.Range(0f, 100f) > 99.9f)
                {
                    if(male)
                    {

                        PlayAudio(Random.Range(0, 18), Utility.audioType.Talking);
                    }
                    else
                    {
                        PlayAudio(Random.Range(18, 36), Utility.audioType.Talking);
                    }
                }
            }
        }
        else
        {
            Collider[] rangeChecks = Physics.OverlapSphere(transform.position, 4);

            for (int i = 0; i < rangeChecks.Length; i++)
            {
                if (rangeChecks[i].tag == "AI" && rangeChecks[i].gameObject != gameObject)
                {
                    StateMachine ai = rangeChecks[i].GetComponent<StateMachine>();
                    if (Random.Range(0f, 100f) > 99.99f)
                    {
                        if(!ai.talking)
                        {
                            float Rand = Random.Range(2, 20);
                            ai.talkDuration = Rand;
                            talkDuration = Rand;
                            ai.talking = true;
                            talking = true;
                            talkPartner = ai.gameObject;
                            ai.talkPartner = gameObject;
                            transform.LookAt(talkPartner.transform);
                        }
                    }
                }
            }
        }
        
    }

    void FindHelp()
    {
        float dist = Mathf.Infinity;
        int closest = 0;
        bool alarmClosest = false;

        for (int i = 0; i < LevelManager.instance.ActiveSecurity.Count; i++)
        {
            float temp = Vector3.Distance(transform.position, LevelManager.instance.ActiveSecurity[i].transform.position);
            if (temp < dist)
            {
                dist = temp;
                closest = i;
            }


        }

        for (int i = 0; i < LevelManager.instance.ActiveAlarms.Count; i++)
        {
            float temp = Vector3.Distance(transform.position, LevelManager.instance.ActiveAlarms[i].transform.position);
            if (temp < dist)
            {
                dist = temp;
                closest = i;
                alarmClosest = true;
            }


        }

        if (!alarmClosest)
        {
            if (LevelManager.instance.ActiveSecurity.Count > 0)
            {
                if (LevelManager.instance.ActiveSecurity[closest] != null)
                {
                    goalPos = LevelManager.instance.ActiveSecurity[closest].transform.position;
                    if (dist <= 3)
                    {
                        LevelManager.instance.ActiveSecurity[closest].Alert();
                        attitude = Utility.Attitude.Afraid;
                    }
                }
            }
            else
            {
                attitude = Utility.Attitude.Afraid;
            }
        }
        else
        {
            if (LevelManager.instance.ActiveAlarms.Count > 0)
            {
                if (LevelManager.instance.ActiveAlarms[closest] != null)
                {
                    goalPos = LevelManager.instance.ActiveAlarms[closest].transform.position;
                    if (dist <= 3)
                    {
                        LevelManager.instance.ActiveAlarms[closest].Activate();
                        attitude = Utility.Attitude.Afraid;
                    }
                }
            }
            else
            {
                attitude = Utility.Attitude.Afraid;
            }
        }
    }

    public void CivilianBrain()
    {
        if (!statusEffects.Contains(Utility.StatusEffects.Stunned))
        {
            FieldOfViewCheck();
        }
            

        if(watchingPlayer)
        {
            if (Player.instance.tresspassing)
            {
                sus += 25 * Time.deltaTime;
            }
            if (Player.instance.armed)
            {
                sus += 30 * Time.deltaTime;
            }
            if (Player.instance.masked)
            {
                sus += 20 * Time.deltaTime;
            }
            if (sus > 50)
            {
                suspicion = Utility.Suspicion.Guarded;
                if (sus > 75)
                {
                    suspicion = Utility.Suspicion.Alert;
                    if (sus > 90)
                    {
                        suspicion = Utility.Suspicion.Hunting;
                    }
                }
            }
        }


        switch (attitude)
        {
            case Utility.Attitude.Afraid:
                int shortest = 0;
                float shortestDist = 0;

                for (int i = 0; i < LevelManager.instance.fleePoints.Count; i++)
                {
                    float dist = Vector3.Distance(LevelManager.instance.fleePoints[i].transform.position, lastSeenPos);
                    if (dist > shortestDist)
                    {
                        shortestDist = dist;
                        shortest = i;
                    }
                }
                //flee point furthest from last seen player position
                goalPos = LevelManager.instance.fleePoints[shortest].transform.position;

                shortest = 0;
                shortestDist = Mathf.Infinity;

                for (int i = 0; i < LevelManager.instance.fleePoints.Count; i++)
                {
                    float dist = Vector3.Distance(LevelManager.instance.fleePoints[i].transform.position, transform.position);
                    if (dist < shortestDist)
                    {
                        shortestDist = dist;
                        shortest = i;
                    }
                }
                if (shortestDist <= 3)
                {
                    LevelManager.instance.ActiveCivilians.Remove(this);
                    LevelManager.instance.FledCivilians.Add(this);
                    Alert();
                    gameObject.SetActive(false);
                }
                
                break;
            case Utility.Attitude.Friendly:

                FollowPath();


                if (watchingPlayer && Player.instance.armed && (int)suspicion >= 1)
                {
                    attitude = Utility.Attitude.Afraid;
                }

                if(sus > 50)
                {
                    FindHelp();
                }
                else
                {
                    TalkCheck();
                }
                    

                break;
            case Utility.Attitude.Calm:

                FollowPath();


                if (watchingPlayer && Player.instance.armed && (int)suspicion >= 1)
                {
                    attitude = Utility.Attitude.Afraid;
                } 

                if (sus > 50)
                {
                    FindHelp();


                }
                else
                {
                    TalkCheck();
                }

                break;
            case Utility.Attitude.Angry:
                FollowPath();

                float distA = Mathf.Infinity;
                int closestA = 0;

                for (int i = 0; i < LevelManager.instance.ActiveCriminals.Count; i++)
                    {
                    float temp = Vector3.Distance(transform.position, LevelManager.instance.ActiveCriminals[i].transform.position);
                    if (temp < distA)
                    {
                        distA = temp;
                        closestA = i;
                    }
                }
                if (LevelManager.instance.ActiveCriminals.Count > 0)
                {
                    if (LevelManager.instance.ActiveCriminals[closestA] != null)
                    {
                        goalPos = LevelManager.instance.ActiveCriminals[closestA].transform.position;
                        if (distA <= 3)
                        {
                            LevelManager.instance.ActiveCriminals[closestA].Alert();
                        }
                    }
                }
                else
                {
                    attitude = Utility.Attitude.Afraid;
                }

                TalkCheck();
                break;
        }
    }

    public void SecurityBrain()
    {
        if (!statusEffects.Contains(Utility.StatusEffects.Stunned))
        {
            FieldOfViewCheck();
        }


        if (watchingPlayer)
        {
            if (Player.instance.tresspassing)
            {
                sus += 25 * Time.deltaTime;
            }
            if (Player.instance.armed)
            {
                sus += 50 * Time.deltaTime;
            }
            if (Player.instance.masked)
            {
                sus += 50 * Time.deltaTime;
            }
            
        }

        if (sus > 50)
        {
            suspicion = Utility.Suspicion.Guarded;
            if (sus > 75)
            {
                Alert();
                if (sus > 90)
                {
                    suspicion = Utility.Suspicion.Hunting;
                }
            }
        }

        if (attitude == Utility.Attitude.Afraid)
        {
            int shortest = 0;
            float shortestDist = 0;

            for (int i = 0; i < LevelManager.instance.fleePoints.Count; i++)
            {
                float dist = Vector3.Distance(LevelManager.instance.fleePoints[i].transform.position, lastSeenPos);
                if (dist > shortestDist)
                {
                    shortestDist = dist;
                    shortest = i;
                }
            }
            //flee point furthest from last seen player position
            goalPos = LevelManager.instance.fleePoints[shortest].transform.position;

            shortest = 0;
            shortestDist = Mathf.Infinity;

            for (int i = 0; i < LevelManager.instance.fleePoints.Count; i++)
            {
                float dist = Vector3.Distance(LevelManager.instance.fleePoints[i].transform.position, transform.position);
                if (dist < shortestDist)
                {
                    shortestDist = dist;
                    shortest = i;
                }
            }
            if (shortestDist <= 3)
            {
                LevelManager.instance.ActiveSecurity.Remove(this);
                LevelManager.instance.FledSecurity.Add(this);
                Alert();
                gameObject.SetActive(false);
            }
        }
        else
        {
            FollowPath();

            if (watchingPlayer && Player.instance.armed && (int)suspicion >= 1)
            {
                attitude = Utility.Attitude.Angry;
            }
        }

        

        switch (suspicion)
        {
            case Utility.Suspicion.Unsuspecting:
                TalkCheck();
                break;
            case Utility.Suspicion.Guarded:
                TalkCheck();
                if(talking)
                {
                    talkDuration -= Time.deltaTime;
                    talkPartner.GetComponent<StateMachine>().talkDuration -= Time.deltaTime;
                }    
                if (watchingPlayer)
                {
                    Vector3 directionToTarget = (eyes.transform.position - Player.instance.transform.position).normalized;
                    transform.LookAt(Player.instance.transform);
                    goalPos = Player.instance.transform.position + (directionToTarget * 3);

                    if (attitude == Utility.Attitude.Angry)
                    {
                        Shoot();
                    }
                }
                break;
            case Utility.Suspicion.Alert:
                Alert();
                if (watchingPlayer)
                {
                    Vector3 directionToTarget = (eyes.transform.position - Player.instance.transform.position).normalized;
                    transform.LookAt(Player.instance.transform);
                    goalPos = Player.instance.transform.position + (directionToTarget * 3);

                    if(attitude == Utility.Attitude.Angry)
                    {
                        Shoot();
                    }
                }
                break;
            case Utility.Suspicion.Hunting:
                if (watchingPlayer)
                {
                    Vector3 directionToTarget = (eyes.transform.position - Player.instance.transform.position).normalized;
                    transform.LookAt(Player.instance.transform);
                    goalPos = Player.instance.transform.position + (directionToTarget * 3);

                    if (attitude == Utility.Attitude.Angry)
                    {
                        Shoot();
                    }
                }
                break;

        }
    }
    public void CriminalBrain()
    {

    }
    public void OperatorBrain()
    {

    }

    public void See()
    {

    }

    void Path()
    {
        nav.SetDestination(goalPos);
        nav.speed = speed;
        //WalkSound("sand");
    }
    public void Path(Vector3 pos)
    {
        nav.SetDestination(pos);
        nav.speed = speed;
        //WalkSound("sand");
    }
    void RunAway()
    {
        if (Vector3.Distance(goalPos, transform.position) <= 200)
        {
            var FromPlayer = goalPos - transform.position;
            nav.SetDestination(transform.position + (-FromPlayer.normalized * 5));
        }
        //WalkSound("sand");
    }
    //void StopMoving()
    //{
    //    nav.SetDestination(transform.position);
    //    nav.speed = speed;
    //    StopWalk();
    //}

    public void Hear(Vector3 v, float volume)
    {
        if (!statusEffects.Contains(Utility.StatusEffects.Stunned))
        {
            switch (faction)
            {
                case Utility.Factions.Civilian:
                    
                    if(volume > 100)
                    {
                        if(attitude != Utility.Attitude.Angry)
                        {
                            attitude = Utility.Attitude.Afraid;
                        }
                        sus += volume - 10;
                    }
                    else if(volume > 10)
                    {
                        //Path(v);
                        goalPos = v;
                        sus += volume - 10;
                    }
                    
                    break;
                case Utility.Factions.Security:
                    if (volume > 500)
                    {
                        if (attitude != Utility.Attitude.Angry)
                        {
                            attitude = Utility.Attitude.Afraid;
                        }
                        sus += volume - 10;
                    }
                    else if (volume > 10)
                    {
                        goalPos = v;
                        sus += volume - 10;
                    }
                    break;
                case Utility.Factions.Criminal:
                    break;
                case Utility.Factions.Operator:
                    break;
                default:
                    break;
            }

        }
    }

    private void FieldOfViewCheck()
    {
        Collider[] rangeChecks = Physics.OverlapSphere(transform.position, visionRange, playerMask);
        //Vector3 debugT = eyes.transform.forward;//vector = Quaternion.AngleAxis(-45, Vector3.up) * vector;
        Debug.DrawRay(eyes.position, ( Quaternion.AngleAxis(-(visionAngle / 2), Vector3.up) * eyes.transform.forward) * visionRange, Color.blue);
        Debug.DrawRay(eyes.position, (Quaternion.AngleAxis((visionAngle / 2), Vector3.up) * eyes.transform.forward) * visionRange, Color.blue);
        if (rangeChecks.Length != 0)
        {
            Transform target = rangeChecks[0].transform;
            Vector3 directionToTarget = (target.position - eyes.transform.position).normalized;

            if (Vector3.Angle(eyes.transform.forward, directionToTarget) < visionAngle / 2)
            {
                float distanceToTarget = Vector3.Distance(eyes.position, new Vector3(target.position.x, target.position.y + Random.Range(2, 6), target.position.z));
                Debug.DrawRay(eyes.position, directionToTarget * (distanceToTarget), Color.green, 1f);
                if (!Physics.Raycast(eyes.position, directionToTarget, distanceToTarget, obstructionMask))
                {
                    watchingPlayer = true;
                    lastSeenPos = Player.instance.transform.position;
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
            Vector3 directionToTarget = (target.position - eyes.transform.position).normalized;

            if (Vector3.Angle(transform.forward, directionToTarget) < visionAngle / 2)
            {
                float distanceToTarget = Vector3.Distance(eyes.position, new Vector3(target.position.x, target.position.y + Random.Range(2, 6), target.position.z));
                Debug.DrawRay(eyes.position, directionToTarget * (distanceToTarget), Color.green, 0.1f);
                if (!Physics.Raycast(eyes.position, directionToTarget, distanceToTarget, obstructionMask))
                {
                    Info info = target.GetComponentInParent<Info>();
                    if (info != null)
                    {
                        sus += info.concern;
                        if(faction == Utility.Factions.Security || faction == Utility.Factions.Criminal)
                        {
                            if(info.angering)
                            {
                                attitude = Utility.Attitude.Angry;
                            }
                        }
                    }
                    //sus += 20 * Time.deltaTime;
                    //set computer screens linked to the camera to convern layer
                }

            }
        }
    }


    //private IEnumerator FOVRoutine()
    //{
    //    WaitForSeconds wait = new WaitForSeconds(0.1f);

    //    while (true)
    //    {
    //        yield return wait;
    //        if (!statusEffects.Contains(Utility.StatusEffects.Stunned))
    //        {
    //            FieldOfViewCheck();
    //        }
    //    }
    //}
    public void Alert()
    {
        suspicion = Utility.Suspicion.Alert;

        reactionTime -= Time.deltaTime;

        if(reactionTime <= 0)
        {
            if (faction == Utility.Factions.Security || faction == Utility.Factions.Civilian)
            {

                for (int i = 0; i < LevelManager.instance.ActiveSecurity.Count; i++)
                {
                    if((int)LevelManager.instance.ActiveSecurity[i].suspicion < 2)
                    {
                        LevelManager.instance.ActiveSecurity[i].Alert();
                    }
                    
                }
            }

            if (faction == Utility.Factions.Criminal)
            {
                for (int i = 0; i < LevelManager.instance.ActiveCriminals.Count; i++)
                {
                    if ((int)LevelManager.instance.ActiveCriminals[i].suspicion < 2)
                    {
                        LevelManager.instance.ActiveCriminals[i].Alert();
                    }
                }
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
                lastAduioType = type;
                lastClip = index;
                temp.Play();
                
                break;
            case Utility.audioType.Walking:
                temp.clip = AudioUtility.walkSounds[index];
                lastAduioType = type;
                lastClip = index;
                temp.Play();
            
                break;
            case Utility.audioType.Useing:
                temp.clip = AudioUtility.useSounds[index];
                lastAduioType = type;
                lastClip = index;
                temp.Play();
            
                break;
            default:
                break;
        }


            //Audio.Play();
        
    }

    public void MakeSound(float volume)
    {
        Collider[] rangeChecks = Physics.OverlapSphere(transform.position, volume);

        for (int i = 0; i < rangeChecks.Length; i++)
        {
            if(rangeChecks[i].tag == "AI")
            {
                StateMachine ai = rangeChecks[i].GetComponent<StateMachine>();
                ai.Hear(transform.position, volume);
            }
        }


    }
    //void WalkSound(string ground)
    //{
    //    if (walkAudio.isPlaying)
    //    {
    //        return;
    //    }
    //    AudioClip temp = null;
    //    switch (ground)
    //    {
    //        case "sand":
    //            temp = AudioUtility.sounds[Random.Range(77, 85)];
    //            break;
    //        case "stone":
    //            temp = AudioUtility.sounds[Random.Range(87, 95)];
    //            break;
    //        case "wood":
    //            temp = AudioUtility.sounds[Random.Range(97, 99)];
    //            break;
    //        case "water":
    //            temp = AudioUtility.sounds[Random.Range(95, 97)];
    //            break;
    //        default:
    //            temp = AudioUtility.sounds[Random.Range(77, 85)];
    //            break;
    //    }
    //    walkAudio.clip = temp;
    //    walkAudio.Play();
    //}
    //public void StopWalk()
    //{
    //    walkAudio.Stop();
    //}

    void Shoot()
    {
        Gun gun = (Gun)AIHeld;
        if (gun.useTime <= 0 && gun.uses == 0)
        {
            gun.Reload();
            PlayAudio(gun.reloadClip, Utility.audioType.Useing);
        }
        else
        {
            Vector3 directionToTarget = (Player.instance.transform.position - gun.tip.transform.position).normalized;
            gun.transform.right = directionToTarget;
            if(gun.Use())
            {
                PlayAudio(gun.useClip, Utility.audioType.Useing);
                MakeSound(gun.useVolume);
                if (gun.uses == 0)
                {
                    PlayAudio(gun.emptyClip, Utility.audioType.Useing);
                }
            }
            
        }
    }

    public virtual void DropItem()
    {
        if(AIHeld != null)
        {
            AIHeld.transform.parent = null;
            //AIHeld.transform.position += Vector3.up;
            AIHeld.GetComponent<Rigidbody>().isKinematic = false;
            AIHeld = null;
        }
    }

    public virtual void Ragdoll()
    {
        AIInfo.gameObject.layer = 8;
        AIInfo.GetComponent<Info>().concern = 40 * Time.deltaTime;
        


        animator.enabled = false;
        animatorIK.ikActive = false;
        animator.GetComponentInChildren<SkinnedMeshRenderer>().updateWhenOffscreen = true;
        
        List<Transform> ragdoll = new List<Transform>();
        List<Transform> hurtbox = new List<Transform>();
        foreach (Transform g in animator.GetComponentsInChildren<Transform>())
        {
            if(g.name != "Jaw")
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

        AIInfo.transform.position = ragdoll[3].position;
        AIInfo.transform.rotation = ragdoll[3].rotation;

        if (Vector3.Distance(Player.instance.transform.position, AIInfo.transform.position) < 3)
        {
            AIInfo.GetComponent<Info>().angering = true;
        }
        else
        {
            AIInfo.GetComponent<Info>().angering = false;
        }
    }
}
