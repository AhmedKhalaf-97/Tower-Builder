using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraFollow : MonoBehaviour
{
    public Transform towerTransform;
    public float smothness = 5f;

    [Header("Shaking")]
    public float shaknessRange = 0.2f;
    public float shaknessTimer = 0.5f;
    float shaknessTimerCountdown;

    [Header("Zooming")]
    public float zoomingSmothness = 2f;
    public bool isZoomingButtonPressed;
    bool shouldZoomOut;

    public LayerMask defaultLayers;
    public LayerMask zoomingOutLayers;

    float zoomingZPosValue;
    float zoomingStartPoint;
    float zoomingEndPoint;

    Transform targetTransform;

    int towerChildsCount;

    Vector3 lerpPos;
    Vector3 targetPos;
    Vector3 offset;

    Vector3 startPos;

    Transform myTransform;
    Camera mainCamera;

    void Start()
    {
        mainCamera = Camera.main;

        myTransform = transform;

        startPos = myTransform.position;

        zoomingZPosValue = startPos.z;

        SetTheCameraOffset();
    }

    void Update()
    {
        Zooming();

        towerChildsCount = (towerTransform.childCount -1);
        targetTransform = towerTransform.GetChild(towerChildsCount);

        targetPos = targetTransform.position + offset;

        lerpPos = Vector3.Lerp(myTransform.position, targetPos, smothness * Time.deltaTime);

        myTransform.position = lerpPos;
    }

    public void SetTheCameraOffset()
    {
        offset = myTransform.position - towerTransform.GetChild(0).position;
    }

    public void ShakeCamera()
    {
        StartCoroutine(ShakeCameraCoroutine());
    }

    IEnumerator ShakeCameraCoroutine()
    {
        shaknessTimerCountdown = shaknessTimer;

        while (true)
        {
            shaknessTimerCountdown -= Time.deltaTime;

            if (shaknessTimerCountdown > 0)
            {
                myTransform.position += Random.insideUnitSphere * shaknessRange;
            }
            else
            {
                break;
            }

            yield return null;
        }
    }

    void Zooming()
    {
        shouldZoomOut = isZoomingButtonPressed;

        if (shouldZoomOut)
        {
            if(mainCamera.cullingMask != zoomingOutLayers)
                mainCamera.cullingMask = zoomingOutLayers;

            zoomingStartPoint = myTransform.position.z;
            zoomingEndPoint = startPos.z + 30f;
        }
        else
        {
            if (mainCamera.cullingMask != defaultLayers && (int)zoomingZPosValue == startPos.z)
                mainCamera.cullingMask = defaultLayers;

            zoomingStartPoint = myTransform.position.z;
            zoomingEndPoint = startPos.z;
        }

        zoomingZPosValue = Mathf.Lerp(zoomingStartPoint, zoomingEndPoint, Time.deltaTime * zoomingSmothness);
    }
}
