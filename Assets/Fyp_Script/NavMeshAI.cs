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

    Stack<LocationManager.Locations> locationStack;

    void Start()
    {
        navMeshAgent = GetComponent<NavMeshAgent>();
        locationManager = LocationManager.instance;
        path = locationManager.PathsList[Random.Range(0, locationManager.PathsList.Count)];
        path.SpotsList.Reverse();
        locationStack = new Stack<LocationManager.Locations>(path.SpotsList);
    }

    void Update()
    {
        if (!navMeshAgent.pathPending && navMeshAgent.remainingDistance <= navMeshAgent.stoppingDistance && (!navMeshAgent.hasPath || navMeshAgent.velocity.sqrMagnitude == 0f)){
            if(locationStack.Count>0){
                moveTo = locationStack.Peek().SpotsList[Random.Range(0, locationStack.Pop().SpotsList.Count)];
                navMeshAgent.destination = moveTo.position;
                // navMeshAgent.Stop();
                //delay timer (fix or random)
            }
        }
        else{}
    }
}
