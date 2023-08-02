using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LevelManager : MonoBehaviour
{
    public static LevelManager instance;
    public GameObject AIInfos;

    [Header("LevelInfo")]
    public List<GameObject> extractions;
    public List<GameObject> fleePoints;
    public List<GameObject> guardPoints;
    public List<GameObject> patrolPoints;
    public List<GameObject> pathPoints;
    public List<GameObject> camPoints;
    public List<GameObject> laserPoints;

    [Header("MissionInfo")]
    public int Difficulty = 1;


    [Header("ActiveUnits")]
    public List<StateMachine> ActiveSecurity = new List<StateMachine>();
    public List<StateMachine> ActiveCriminals = new List<StateMachine>();
    public List<StateMachine> ActiveCivilians = new List<StateMachine>();
    public List<StateMachine> ActiveOperators = new List<StateMachine>();

    [Header("DeadUnits")]
    public List<StateMachine> DeadSecurity = new List<StateMachine>();
    public List<StateMachine> DeadCriminals= new List<StateMachine>();
    public List<StateMachine> DeadCivilians= new List<StateMachine>();
    public List<StateMachine> DeadOperators = new List<StateMachine>();

    [Header("UnconsiousUnits")]
    public List<StateMachine> UnconsiousSecurity = new List<StateMachine>();
    public List<StateMachine> UnconsiousCriminals= new List<StateMachine>();
    public List<StateMachine> UnconsiousCivilians= new List<StateMachine>();
    public List<StateMachine> UnconsiousOperators = new List<StateMachine>();

    [Header("FledUnits")]
    public List<StateMachine> FledSecurity = new List<StateMachine>();
    public List<StateMachine> FledCriminals= new List<StateMachine>();
    public List<StateMachine> FledCivilians= new List<StateMachine>();
    public List<StateMachine> FledOperators = new List<StateMachine>();

    // Start is called before the first frame update
    void Awake()
    {
        instance = this;
        fleePoints = new List<GameObject>();
        guardPoints = new List<GameObject>();
        patrolPoints = new List<GameObject>();
        pathPoints = new List<GameObject>();
        camPoints = new List<GameObject>();

        Transform temp = transform.Find("PatrolPoints");
        for (int i = 0; i < temp.childCount; i++)
        {
            patrolPoints.Add(temp.GetChild(i).gameObject);
        }
        temp = transform.Find("PathPoints");
        for (int i = 0; i < temp.childCount; i++)
        {
            pathPoints.Add(temp.GetChild(i).gameObject);
        }
        temp = transform.Find("FleePoints");
        for (int i = 0; i < temp.childCount; i++)
        {
            fleePoints.Add(temp.GetChild(i).gameObject);
        }
        temp = transform.Find("GuardPoints");
        for (int i = 0; i < temp.childCount; i++)
        {
            guardPoints.Add(temp.GetChild(i).gameObject);
        }
        temp = transform.Find("CamPoints");
        for (int i = 0; i < temp.childCount; i++)
        {
            camPoints.Add(temp.GetChild(i).gameObject);
        }
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
