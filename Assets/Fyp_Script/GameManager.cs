using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

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


    
    [System.Serializable]
    public class SpawnState {
        public Transform Location;
        public bool Burst;
        // [Range(1, 100)]
        // public int npcCount;
    }
    [Header("Spawner Setting")]
    public List<SpawnState> spawning;
    public List<GameObject> Ai;

    public GameObject menu;
    public InputField npcCount;
    public InputField totalFaceMask;
    public InputField Duration;

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
        // InvokeRepeating("repeat", 1, 15+TrainWaitTime);
    }


    public void spawn(){
        InvokeRepeating("repeat", 1, TrainWaitTime*3);
        int npcArea = int.Parse(npcCount.text)/spawning.Count;
        menu.SetActive(false);
        foreach (SpawnState spawn in spawning)
        {
            if(Ai.Count > npcArea){
                for (int i = 0; i < npcArea; i++)
                {
                    GameObject newAI = Instantiate(Ai[i], spawn.Location.position, Quaternion.identity);
                }
            }
            else{
                int mod = npcArea % Ai.Count;
                int div = npcArea / Ai.Count;

                for (int i = 0; i < mod; i++)
                {
                    for (int j = 0; j < div+1; j++)
                    {
                        GameObject newAI = Instantiate(Ai[i], spawn.Location.position, Quaternion.identity);
                    }
                }

                for (int i = mod; i < Ai.Count; i++)
                {
                    for (int j = 0; j < div; j++)
                    {
                        GameObject newAI = Instantiate(Ai[i], spawn.Location.position, Quaternion.identity);
                    }
                }
            }
        }
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
