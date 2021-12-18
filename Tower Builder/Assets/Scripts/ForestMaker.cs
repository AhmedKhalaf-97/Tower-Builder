using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ForestMaker : MonoBehaviour
{
    public GameObject spotLightPrefab;
    public Transform streetLightsParent;

    Transform myTransform;

    void Start()
    {
        myTransform = transform;

        for (int i = 0; i < myTransform.childCount; i++)
        {
            GameObject spotLight01GO = Instantiate(spotLightPrefab, myTransform.GetChild(i));
            GameObject spotLight02GO = Instantiate(spotLightPrefab, myTransform.GetChild(i));

            spotLight01GO.transform.localPosition = new Vector3(0f, 6f, 14f);
            spotLight02GO.transform.localPosition = new Vector3(0f, -6f, 14f);

            spotLight01GO.transform.localEulerAngles = new Vector3(0f, 180f, 0f);
            spotLight02GO.transform.localEulerAngles = new Vector3(0f, 180f, 0f);

            spotLight01GO.transform.SetParent(streetLightsParent, true);
            spotLight02GO.transform.SetParent(streetLightsParent, true);

            spotLight01GO.transform.localEulerAngles = new Vector3(90f, 0f, 0f);
            spotLight02GO.transform.localEulerAngles = new Vector3(90f, 0f, 0f);

            spotLight01GO.transform.localScale = Vector3.one;
            spotLight02GO.transform.localScale = Vector3.one;
        }
    }
}
