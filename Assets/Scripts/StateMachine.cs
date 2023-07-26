using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class StateMachine : MonoBehaviour
{
    public Utility.Factions faction;
    public Utility.Attitude attitude;
    public Utility.healthLevel healthLevel;
    public Utility.Suspicion suspicion;
    public List<Utility.StatusEffects> statusEffects;
    public List<float> statusEffectLength;

    public float health = 100;
    public float sus = 0;
    public float speed = 0;
    float reactionTime = 1;

    public Animator animator;
    public Animator hurtboxAnimator;
    public IKController animatorIK;
    public IKController hurtboxAnimatorIK;
    public GameObject models;

    [Header("Audio")]
    AudioSource Audio;
    [SerializeField]
    AudioSource walkAudio;

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
    List<GameObject> pathPoints = new List<GameObject>();
    int currentPathPoint = 0;
    public Transform eyes;
    public float visionRange = 10;
    public float visionAngle = 10;
    public LayerMask playerMask;
    public LayerMask concernMask;
    public LayerMask obstructionMask;
    Vector3 lastSeenPos;
    [Header("AIInventory")]
    public Item AIHeld;
    public List<Item> AIItems;


    // Start is called before the first frame update
    void Start()
    {
        animatorIK = animator.GetComponent<IKController>();
        hurtboxAnimatorIK = hurtboxAnimator.GetComponent<IKController>();

        nav = GetComponent<NavMeshAgent>();
        switch(faction)
        {
            case Utility.Factions.Civilian:
                LevelManager.instance.ActiveCivilians.Add(this);

                int pointAmount = Random.Range(1, 20);
                for (int i = 0; i < pointAmount; i++)
                {
                    pathPoints.Add(LevelManager.instance.pathPoints[Random.Range(0, LevelManager.instance.pathPoints.Count)]);
                }


                break;
            case Utility.Factions.Security:
                LevelManager.instance.ActiveSecurity.Add(this);

                int pointAmount2 = Random.Range(1, 20);
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
    void Update()
    {
        if(healthLevel == Utility.healthLevel.Dead || !gameObject.activeSelf)
        {
            return;
        }

        if(health < 50)
        {
            healthLevel = Utility.healthLevel.Wounded;
            attitude = Utility.Attitude.Afraid;
            if (health < 10)
            {
                healthLevel = Utility.healthLevel.Unconsious;
                nav.destination = transform.position;

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
                    nav.destination = transform.position;

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

        nav.destination = goalPos;
        animator.SetFloat("Walk", 1);
        hurtboxAnimator.SetFloat("Walk", 1);
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
                if(sus > 75)
                {
                    suspicion = Utility.Suspicion.Alert;
                    if(sus > 90)
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

                goalPos = pathPoints[currentPathPoint].transform.position;

                if(Vector3.Distance(goalPos, transform.position) < 3)
                {
                    currentPathPoint++;
                    if(currentPathPoint >= pathPoints.Count)
                    {
                        currentPathPoint = 0;

                    }
                }


                if (watchingPlayer && Player.instance.armed && (int)suspicion >= 1)
                {
                    attitude = Utility.Attitude.Afraid;
                }

                if(sus > 50)
                {
                    float dist = Mathf.Infinity;
                    int closest = 0;

                    for (int i = 0; i < LevelManager.instance.ActiveSecurity.Count; i++)
                    {
                        float temp = Vector3.Distance(transform.position, LevelManager.instance.ActiveSecurity[i].transform.position);
                        if (temp < dist)
                        {
                            dist = temp;
                            closest = i;
                        }


                    }
                    

                    if (LevelManager.instance.ActiveSecurity.Count > 0)
                    {
                        if (LevelManager.instance.ActiveSecurity[closest] != null)
                        {
                            goalPos = LevelManager.instance.ActiveSecurity[closest].transform.position;
                            if (dist <= 3)
                            {
                                LevelManager.instance.ActiveSecurity[closest].Alert();
                            }
                        }
                    }
                    else
                    {
                        attitude = Utility.Attitude.Afraid;
                    }
                }
                    

                break;
            case Utility.Attitude.Calm:

                goalPos = pathPoints[currentPathPoint].transform.position;

                if (Vector3.Distance(goalPos, transform.position) < 3)
                {
                    currentPathPoint++;
                    if (currentPathPoint >= pathPoints.Count)
                    {
                        currentPathPoint = 0;

                    }
                }


                if (watchingPlayer && Player.instance.armed && (int)suspicion >= 1)
                {
                    attitude = Utility.Attitude.Afraid;
                }

                if (sus > 50)
                {
                    float dist = Mathf.Infinity;
                    int closest = 0;

                    for (int i = 0; i < LevelManager.instance.ActiveSecurity.Count; i++)
                    {
                        float temp = Vector3.Distance(transform.position, LevelManager.instance.ActiveSecurity[i].transform.position);
                        if (temp < dist)
                        {
                            dist = temp;
                            closest = i;
                        }


                    }


                    if (LevelManager.instance.ActiveSecurity.Count > 0)
                    {
                        if (LevelManager.instance.ActiveSecurity[closest] != null)
                        {
                            goalPos = LevelManager.instance.ActiveSecurity[closest].transform.position;
                            if (dist <= 3)
                            {
                                LevelManager.instance.ActiveSecurity[closest].Alert();
                            }
                        }
                    }
                    else
                    {
                        attitude = Utility.Attitude.Afraid;
                    }
                }

                break;
            case Utility.Attitude.Angry:
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
                    Alert();
                    if (sus > 90)
                    {
                        suspicion = Utility.Suspicion.Hunting;
                    }
                }
            }
        }

        if(attitude == Utility.Attitude.Afraid)
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
            goalPos = pathPoints[currentPathPoint].transform.position;

            if (Vector3.Distance(goalPos, transform.position) < 3)
            {
                currentPathPoint++;
                if (currentPathPoint >= pathPoints.Count)
                {
                    currentPathPoint = 0;

                }
            }
        }

        switch(suspicion)
        {
            case Utility.Suspicion.Unsuspecting:

            break;
            case Utility.Suspicion.Guarded:
                if(watchingPlayer)
                {
                    Vector3 directionToTarget = (eyes.transform.position - Player.instance.transform.position).normalized;

                    goalPos = Player.instance.transform.position + (directionToTarget * 3);
                }
                break;
            case Utility.Suspicion.Alert:
                Alert();
                if (watchingPlayer)
                {
                    Vector3 directionToTarget = (eyes.transform.position - Player.instance.transform.position).normalized;

                    goalPos = Player.instance.transform.position + (directionToTarget * 3);
                    Shoot();
                }
                break;
            case Utility.Suspicion.Hunting:
                if (watchingPlayer)
                {
                    Vector3 directionToTarget = (eyes.transform.position - Player.instance.transform.position).normalized;
                    transform.LookAt(Player.instance.transform);
                    goalPos = Player.instance.transform.position + (directionToTarget * 3);
                    Shoot();
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
    void StopMoving()
    {
        nav.SetDestination(transform.position);
        nav.speed = speed;
        StopWalk();
    }

    public void Hear(Vector3 v)
    {
        if (!statusEffects.Contains(Utility.StatusEffects.Stunned))
        {
            nav.SetDestination(v);
            nav.speed = speed;
        }
    }

    private void FieldOfViewCheck()
    {
        Collider[] rangeChecks = Physics.OverlapSphere(transform.position, visionRange, playerMask);

        if (rangeChecks.Length != 0)
        {
            Transform target = rangeChecks[0].transform;
            Vector3 directionToTarget = (target.position - eyes.transform.position).normalized;

            if (Vector3.Angle(transform.forward, directionToTarget) < visionAngle / 2)
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

        if (rangeChecks2.Length != 0)
        {
            Transform target = rangeChecks2[0].transform;
            Vector3 directionToTarget = (target.position - eyes.transform.position).normalized;

            if (Vector3.Angle(transform.forward, directionToTarget) < visionAngle / 2)
            {
                float distanceToTarget = Vector3.Distance(eyes.position, new Vector3(target.position.x, target.position.y + Random.Range(2, 6), target.position.z));
                Debug.DrawRay(eyes.position, directionToTarget * (distanceToTarget), Color.green, 0.1f);
                if (!Physics.Raycast(eyes.position, directionToTarget, distanceToTarget, obstructionMask))
                {
                    sus += 20 * Time.deltaTime;
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

    public void PlayAudio(int index)
    {
        Audio.clip = AudioUtility.sounds[index];
        Audio.Play();
    }
    void WalkSound(string ground)
    {
        if (walkAudio.isPlaying)
        {
            return;
        }
        AudioClip temp = null;
        switch (ground)
        {
            case "sand":
                temp = AudioUtility.sounds[Random.Range(77, 85)];
                break;
            case "stone":
                temp = AudioUtility.sounds[Random.Range(87, 95)];
                break;
            case "wood":
                temp = AudioUtility.sounds[Random.Range(97, 99)];
                break;
            case "water":
                temp = AudioUtility.sounds[Random.Range(95, 97)];
                break;
            default:
                temp = AudioUtility.sounds[Random.Range(77, 85)];
                break;
        }
        walkAudio.clip = temp;
        walkAudio.Play();
    }
    public void StopWalk()
    {
        walkAudio.Stop();
    }

    void Shoot()
    {
        Gun gun = (Gun)AIHeld;
        if (gun.uses == 0)
        {
            gun.Reload();
        }
        else
        {
            Vector3 directionToTarget = (Player.instance.transform.position - gun.tip.transform.position).normalized;
            gun.transform.right = directionToTarget;
            gun.Use();
        }
    }

    public virtual void DropItem()
    {

    }
}
