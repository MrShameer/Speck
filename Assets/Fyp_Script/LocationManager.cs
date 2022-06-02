using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LocationManager : MonoBehaviour
{
    public static LocationManager instance;

    [System.Serializable]
    public class Locations{
        public string Name;
        public List<Transform> SpotsList;
    }
    public List<Locations> LocationsList;
    
    [System.Serializable]
    public class Paths{
        [Dropdown("LocationsList","Name")]
        public List<Locations> SpotsList;
    }
    public List<Paths> PathsList;

    private void Awake() {
        instance = this;
    }

    void Start()
    {
        
    }

    void Update()
    {
        
    }
}
