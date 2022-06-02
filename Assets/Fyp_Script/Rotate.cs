using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Rotate : MonoBehaviour
{
    [Range(1, 100)]
    public int rotationSpeed;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKey("x"))
        {
            transform.Rotate(rotationSpeed * Time.deltaTime, 0, 0);
        }
        else if(Input.GetKey("y"))
        {
            transform.Rotate(0, rotationSpeed * Time.deltaTime, 0);
        }
        else if(Input.GetKey("z"))
        {
            transform.Rotate(0, 0, rotationSpeed * Time.deltaTime);
        }

        
    }
}
