using UnityEngine;
using UnityEngine.EventSystems;

public class ZoomingButton : MonoBehaviour, IPointerDownHandler, IPointerExitHandler, IPointerUpHandler
{
    CameraFollow cameraFollow;

    void Awake()
    {
        cameraFollow = Camera.main.GetComponent<CameraFollow>();
    }

    public void OnPointerDown(PointerEventData eventData)
    {
        cameraFollow.isZoomingButtonPressed = true;
    }

    public void OnPointerUp(PointerEventData eventData)
    {
        cameraFollow.isZoomingButtonPressed = false;
    }

    public void OnPointerExit(PointerEventData eventData) //OnPointerDown is also required to receive OnPointerUp callbacks.
    {
    }
}