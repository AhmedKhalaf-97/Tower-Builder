using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WorldViewer : MonoBehaviour
{
    public float cameraRotationSpeed = 10f;
    Transform myTransform;

    void Start()
    {
        myTransform = transform;
    }


    void Update()
    {
        myTransform.Rotate(Vector3.up, cameraRotationSpeed * Time.deltaTime);
    }
}
