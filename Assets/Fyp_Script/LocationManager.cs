using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LocationManager : MonoBehaviour
{
    public static LocationManager instance;

    [System.Serializable]
    public class Locations{
        public string Name;
        public Transform Spots;
        public float Wait;
        public bool last;
    }
    public List<Locations> LocationsList;
    
    [System.Serializable]
    public class Paths{
        [Dropdown("LocationsList","Name")]
        public List<Locations> SpotsList;
    }
    public List<Paths> PathsList;

    // private void Awake() {
    //     instance = this;
    // }

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
        
    }

    void Update()
    {
        
    }
}
