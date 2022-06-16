using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class NavMeshAI : MonoBehaviour
{
    private NavMeshAgent navMeshAgent;

    LocationManager locationManager;
    LocationManager.Paths path;
    LocationManager.Locations location;
    Transform moveTo;
    Transform trainBoarding;
    GameManager gameManager = GameManager.instance;
    Queue<LocationManager.Locations> locationStack;

    bool board = true;

    void Start()
    {
        navMeshAgent = GetComponent<NavMeshAgent>();
        locationManager = LocationManager.instance;
        path = locationManager.PathsList[Random.Range(0, locationManager.PathsList.Count)];
        locationStack = new Queue<LocationManager.Locations>(path.SpotsList);
    }

    void Update()
    {
        if(trainBoarding!=null && Vector3.Distance(transform.position, trainBoarding.position)<1){
            Destroy(gameObject);
        
        }
        else if(location != null && location.last){
            if(board && gameManager.available){
                trainBoarding = gameManager.trainBoarding.GetComponentsInChildren<Transform>()[Random.Range(1, gameManager.trainBoarding.GetComponentsInChildren<Transform>().Length)];
                navMeshAgent.destination = trainBoarding.position;
                board = false;
            }
            else if(!board && !gameManager.available){
                navMeshAgent.destination = moveTo.position;
                board = true;
            }
            
            
        }
        else if (!navMeshAgent.pathPending && navMeshAgent.remainingDistance <= navMeshAgent.stoppingDistance && (!navMeshAgent.hasPath || navMeshAgent.velocity.sqrMagnitude == 0f)){
            if(location != null && locationStack.Count>0){    
                StartCoroutine(timer(location.Wait));
            }
            if(locationStack.Count>0){
                location = locationStack.Dequeue();
                if(location != null){
                    moveTo = location.Spots.GetComponentsInChildren<Transform>()[Random.Range(1, location.Spots.GetComponentsInChildren<Transform>().Length)];
                    navMeshAgent.destination = moveTo.position;
                }
            }
        }
    }

     IEnumerator timer(float time){
        navMeshAgent.isStopped = true;
        yield return new WaitForSecondsRealtime(time);
        navMeshAgent.isStopped = false;
    }
}
