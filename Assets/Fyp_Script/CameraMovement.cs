using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraMovement : MonoBehaviour
{
    [SerializeField] private Camera cam;
    [SerializeField] private Transform target;
    [SerializeField] private float distanceToTarget = 10;
    
    private Vector3 previousPosition;
    private float rotationX;
    private float rotationY;

    private Vector3 position;
    void Start(){
        mousePress();
        mouseHold();
    }
    void Update()
    {
        if (Input.GetMouseButtonDown(0))
        {
            mousePress();
        }
        else if (Input.GetMouseButton(0))
        {
            mouseHold();
        }
        else if(Input.GetMouseButton(2))
        {
            float mouseX = Input.GetAxis("Mouse X");
            float mouseY = Input.GetAxis("Mouse Y");
            position += target.right * (mouseX * -1) ;
            position += target.forward * (mouseY * -1);
            target.position = position;
        }
        else if(Input.GetAxis("Mouse ScrollWheel") != 0)
        {
            float mouseScroll = Input.GetAxis("Mouse ScrollWheel");
            distanceToTarget += mouseScroll*-10;
            cam.transform.position = target.position;
            cam.transform.Translate(new Vector3(0, 0, -distanceToTarget));
        }
    }

    void mousePress(){
        previousPosition = cam.ScreenToViewportPoint(Input.mousePosition);
    }

    //PROBLEM CLAMPPING
    void mouseHold(){
        Vector3 newPosition = cam.ScreenToViewportPoint(Input.mousePosition);
        Vector3 direction = previousPosition - newPosition;
        
        float rotationAroundYAxis = -direction.x * 180; // camera moves horizontally
        float rotationAroundXAxis = direction.y * 180; // camera moves vertically
        
        cam.transform.position = target.position;
        cam.transform.Rotate(new Vector3(1, 0, 0), rotationAroundXAxis);
        target.transform.Rotate(new Vector3(0, 1, 0), rotationAroundYAxis, Space.World); // <— This is what makes it work!
        
        cam.transform.Translate(new Vector3(0, 0, -distanceToTarget));
        
        previousPosition = newPosition;
    }
}
