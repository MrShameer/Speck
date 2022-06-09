using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    public static GameManager instance;

    [Header("Train Setting")]
    public GameObject trainPrefab;
    public Transform trainSpawner;
    public Transform trainDestination;
    public Transform trainDestroy;
    public float TrainWaitTime;
    GameObject train;
    Transform target;

    [Header("Train Boarding")]
    public Transform trainBoarding;
    
    [HideInInspector]
    public bool available = false;

    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
        }
        else if (instance != this)
        {
            Debug.Log("Instance already exists, destroying object!");
            Destroy(this);
        }
    }

    void Start()
    {
        InvokeRepeating("repeat", 1, 15+TrainWaitTime);
    }

    void Update()
    {
        if(train){
            train.transform.position = Vector3.MoveTowards(train.transform.position, target.position,  Time.deltaTime * 10);
            if(Vector3.Distance(trainDestroy.position, train.transform.position) < 0.1f){
                Destroy(train);
            }else if(Vector3.Distance(trainDestination.position, train.transform.position) < 0.1f){
                available = true;
                StartCoroutine(timer());
            }
        }
    }

    void repeat(){
        train = Instantiate(trainPrefab,trainSpawner.position, Quaternion.Euler(0,0,0)) as GameObject;
        target = trainDestination;
    }

    IEnumerator timer(){
        yield return new WaitForSecondsRealtime(TrainWaitTime);
        target = trainDestroy;
        available = false;
    }
}
