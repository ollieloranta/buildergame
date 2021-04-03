using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraMovement : MonoBehaviour
{
    public float Speed = 4.0f;
    public float cameraDistanceMax = -2f;
    public float cameraDistanceMin = -25f;
    public float scrollSpeed = 2.5f;
 
    void Update()
    {
        float cameraDistance = transform.position.z;
        cameraDistance += Input.GetAxis("Mouse ScrollWheel") * scrollSpeed;
        cameraDistance = Mathf.Clamp(cameraDistance, cameraDistanceMin, cameraDistanceMax);

        float distanceAmp = cameraDistance * -0.1f;
        float xAxisValue = Input.GetAxis("Horizontal") * Speed * Time.deltaTime * distanceAmp;
        float yAxisValue = Input.GetAxis("Vertical") * Speed * Time.deltaTime * distanceAmp;

        transform.position = new Vector3(transform.position.x + xAxisValue, transform.position.y + yAxisValue, cameraDistance);
    }
}
