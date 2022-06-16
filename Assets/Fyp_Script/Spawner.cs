using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class Spawner : MonoBehaviour
{
    //spawn delay

    [System.Serializable]
    public class SpawnState {
        public Transform Location;
        public bool Burst;
        [Range(1, 100)]
        public int npcCount;
    }

    public List<SpawnState> spawning;
    public List<GameObject> Ai;

    void Start()
    {
        // foreach (SpawnState spawn in spawning)
        // {
        //     if(Ai.Count > spawn.npcCount){
        //         for (int i = 0; i < spawn.npcCount; i++)
        //         {
        //             GameObject newAI = Instantiate(Ai[i], spawn.Location.position, Quaternion.identity);
        //         }
        //     }
        //     else{
        //         int mod = spawn.npcCount % Ai.Count;
        //         int div = spawn.npcCount / Ai.Count;

        //         for (int i = 0; i < mod; i++)
        //         {
        //             for (int j = 0; j < div+1; j++)
        //             {
        //                 GameObject newAI = Instantiate(Ai[i], spawn.Location.position, Quaternion.identity);
        //             }
        //         }

        //         for (int i = mod; i < Ai.Count; i++)
        //         {
        //             for (int j = 0; j < div; j++)
        //             {
        //                 GameObject newAI = Instantiate(Ai[i], spawn.Location.position, Quaternion.identity);
        //             }
        //         }
        //     }
        // }
    }

    //3/3 = 1 1 1
    //4/3 = 2 1 1
    //5/3 = 2 2 1
    //6/3 = 2 2 2
    //7/3 = 3 2 2
    //8//3 = 3 3 2
    //9/3 = 3 3 3
    //10/3 = 4 3 3
}
