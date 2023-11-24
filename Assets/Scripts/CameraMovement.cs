using Cinemachine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraMovement : MonoBehaviour
{
    // Start is called before the first frame update
    public float cameraSpeed;
    public float upLimit, downLimit, leftLimit, rightLimit;
    private Vector2 movementVector;
    public GameObject freeFlightCamera;
    public float remainingDistance;
    public float minZoom, maxZoom;
    public float zoomSpeed;

    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        checkKeyboardInput();
        checkMouseWheel();

        if(remainingDistance > 0)
        {
            freeFlightCamera.GetComponent<CinemachineVirtualCamera>().m_Lens.OrthographicSize += zoomSpeed;
            remainingDistance -= zoomSpeed;
        }
         else if(remainingDistance < 0)
        {
            freeFlightCamera.GetComponent<CinemachineVirtualCamera>().m_Lens.OrthographicSize -= zoomSpeed;
            remainingDistance += zoomSpeed;
        }

        if(Mathf.Abs(remainingDistance) < zoomSpeed)
        {
            remainingDistance = 0;
        }

        if(freeFlightCamera.GetComponent<CinemachineVirtualCamera>().m_Lens.OrthographicSize > maxZoom)
        {
            freeFlightCamera.GetComponent<CinemachineVirtualCamera>().m_Lens.OrthographicSize = maxZoom;
            remainingDistance = 0;
        }

        if(freeFlightCamera.GetComponent<CinemachineVirtualCamera>().m_Lens.OrthographicSize < minZoom)
        {
            freeFlightCamera.GetComponent<CinemachineVirtualCamera>().m_Lens.OrthographicSize = minZoom;
            remainingDistance = 0;
        }

        
    }

    //Movimiento de la cámara con WASD
    private void checkKeyboardInput()
    {
        movementVector = new Vector2(0, 0);

        if(Input.GetKey(KeyCode.W) && transform.position.y < upLimit) 
        {
            movementVector.y = 1;
            //transform.Translate(Vector2.up * cameraSpeed);
        }
        else if (Input.GetKey(KeyCode.S) && transform.position.y > downLimit)
        {
            //transform.Translate(Vector2.down * cameraSpeed);
            movementVector.y = -1;
        }
        
        if (Input.GetKey(KeyCode.D) && transform.position.x < rightLimit)
        {
            //transform.Translate(Vector2.right * cameraSpeed);
            movementVector.x = 1;
        }
        else if(Input.GetKey(KeyCode.A) && transform.position.x > leftLimit) 
        {
            //transform.Translate(Vector2.left * cameraSpeed);
            movementVector.x = -1;
        }

        transform.Translate(movementVector * cameraSpeed);

    }

    //Zoom in y zoom out de la cámara
    private void checkMouseWheel()
    {
        if(Input.mouseScrollDelta.y < 0)
        {
            if((freeFlightCamera.GetComponent<CinemachineVirtualCamera>().m_Lens.OrthographicSize + remainingDistance ) < maxZoom)
            {
                remainingDistance += 1;
            } 
        }
        else if(Input.mouseScrollDelta.y > 0)
        {
            if ((freeFlightCamera.GetComponent<CinemachineVirtualCamera>().m_Lens.OrthographicSize - remainingDistance) > minZoom)
            {
                remainingDistance -= 1;
            }
        }
    }
}
