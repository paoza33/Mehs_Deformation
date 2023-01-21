using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraMovement : MonoBehaviour
{
    private Vector3 velocity;

    private bool QkeyIsDown = false;
    private bool DkeyIsDown = false;
    private bool SkeyIsDown = false;
    private bool ZkeyIsDown = false;

    private float sensitivity = 500f;

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Q)) QkeyIsDown = true;
        else if (Input.GetKeyUp(KeyCode.Q)) QkeyIsDown = false;
        

        if (Input.GetKeyDown(KeyCode.D)) DkeyIsDown = true;
        else if (Input.GetKeyUp(KeyCode.D)) DkeyIsDown = false;
        

        if (Input.GetKeyDown(KeyCode.S)) SkeyIsDown = true;
        else if (Input.GetKeyUp(KeyCode.S)) SkeyIsDown = false;
        

        if (Input.GetKeyDown(KeyCode.Z)) ZkeyIsDown = true;
        else if (Input.GetKeyUp(KeyCode.Z)) ZkeyIsDown = false;
    }

    private void FixedUpdate()
    {
        if (QkeyIsDown)
        {
            Vector3 vect = new Vector3(transform.position.x - 1, transform.position.y, transform.position.z);
            transform.position = Vector3.SmoothDamp(transform.position, vect, ref velocity, 0.1f);
        }

        if (DkeyIsDown)
        {
            Vector3 vect = new Vector3(transform.position.x + 1, transform.position.y, transform.position.z);
            transform.position = Vector3.SmoothDamp(transform.position, vect, ref velocity, 0.1f);
        }

        if (SkeyIsDown)
        {
            Vector3 vect = new Vector3(transform.position.x, transform.position.y, transform.position.z - 1);
            transform.position = Vector3.SmoothDamp(transform.position, vect, ref velocity, 0.1f);
        }

        if (ZkeyIsDown)
        {
            Vector3 vect = new Vector3(transform.position.x, transform.position.y, transform.position.z + 1);
            transform.position = Vector3.SmoothDamp(transform.position, vect, ref velocity, 0.1f);
        }

        float mouseX = Input.GetAxis("Mouse X") * sensitivity * Time.deltaTime;
        float mouseY = Input.GetAxis("Mouse Y") * sensitivity * Time.deltaTime;

        transform.eulerAngles += new Vector3(-mouseY, mouseX, 0f);
    }
}
