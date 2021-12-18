using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TrafficManager : MonoBehaviour
{
    public float carSpeed = 1f;
    public GameObject[] carPrefabs;

    public Material[] regularCarMaterials;
    public Material[] minivanMaterials;

    public Transform checkpointsParent;

    Transform myTransform;

    Transform[] instantiatedCars;

    List<Tracks> tracksList = new List<Tracks>();

    [System.Serializable]
    public class Tracks
    {
        public string trackName;
        public Transform[] checkpointChildsArray;
        public int nextCheckpoint = 1;
    }

    void Awake()
    {
        myTransform = transform;
    }

    void Start()
    {
        FillTheTracksList();

        InstantiateCars();
    }

    void Update()
    {
        CarsGoToNextCheckpoint();
    }

    void FillTheTracksList()
    {
        for (int i = 0; i < checkpointsParent.childCount; i++)
        {
            tracksList.Add(new Tracks());
            Transform trackTransform = checkpointsParent.GetChild(i);
            tracksList[i].checkpointChildsArray = new Transform[trackTransform.childCount];
            tracksList[i].trackName = "Track (" + (i + 1) + ")";


            for (int x = 0; x < trackTransform.childCount; x++)
            {
                tracksList[i].checkpointChildsArray[x] = trackTransform.GetChild(x);
            }
        }
    }

    void InstantiateCars()
    {
        instantiatedCars = new Transform[tracksList.Count];

        for (int i = 0; i < tracksList.Count; i++)
        {
            Transform startingCheckpoint = tracksList[i].checkpointChildsArray[0];
            int randomCar = Random.Range(0, 2);

            GameObject instantiatedCarGO = Instantiate(carPrefabs[randomCar], startingCheckpoint.position, startingCheckpoint.rotation, myTransform);

            instantiatedCars[i] = instantiatedCarGO.transform;

            if (randomCar == 0)
                instantiatedCarGO.GetComponentInChildren<Renderer>().material = regularCarMaterials[Random.Range(0, regularCarMaterials.Length)];
            else
                instantiatedCarGO.GetComponentInChildren<Renderer>().material = minivanMaterials[Random.Range(0, regularCarMaterials.Length)];
        }
    }

    void CarsGoToNextCheckpoint()
    {
        for (int i = 0; i < instantiatedCars.Length; i++)
        {
            Transform car = instantiatedCars[i];

            var track = tracksList[i];
            var checkpoints = track.checkpointChildsArray;
            int nextCheckpoint = track.nextCheckpoint;

            if (Vector3.Distance(car.position, checkpoints[nextCheckpoint].position) == 0)
            {
                car.eulerAngles = checkpoints[nextCheckpoint].eulerAngles;

                if ((nextCheckpoint + 1) == checkpoints.Length)
                {
                    car.position = checkpoints[0].position;
                    car.eulerAngles = checkpoints[0].eulerAngles;
                    track.nextCheckpoint = 1;
                }
                else
                {
                    track.nextCheckpoint++;
                }
            }
            else
            {
                car.position = Vector3.MoveTowards(car.position, checkpoints[nextCheckpoint].position, carSpeed);
            }
        }
    }

    public void ReloadTraffic() //Called from GameManager.
    {
        if(instantiatedCars == null)
            return;

        foreach (Transform instantiatedCar in instantiatedCars)
        {
            Destroy(instantiatedCar.gameObject);
        }
        instantiatedCars = new Transform[0];

        tracksList.Clear();

        FillTheTracksList();

        InstantiateCars();
    }
}