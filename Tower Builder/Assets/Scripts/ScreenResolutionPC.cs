using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ScreenResolutionPC : MonoBehaviour
{
    public void SetResolutionTo886x1920()
    {
        Screen.SetResolution(886, 1920, false);
    }

    public void SetResolutionTo1080x1920()
    {
        Screen.SetResolution(1080, 1920, false);
    }

    public void SetResolutionTo1200x1600()
    {
        Screen.SetResolution(1200, 1600, false);
    }
}
