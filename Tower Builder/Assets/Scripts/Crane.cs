using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Crane : MonoBehaviour
{

    public GameManager gameManager;

    public Transform towerTransform;

    bool shouldPullupCrane;
    public float pullupSpeed = 1f;
    public float destroyTimerCountdown = 0.5f;

    public bool shouldPulldownCrane;
    public float pulldownSpeed = 1f;
    public Vector3 craneCurrentPos;
    public bool isInstantiateCommandSent;

    public float swingingAnimationDelayTime = 5f;
    Animator craneAnimator;

    float timerCountdown = 1.5f;

    bool canDetach;

    Transform myTransform;
    Transform craneHolderTransfrom;
    Transform buildingBlockTransform;

    void Awake()
    {
        myTransform = transform;
        craneHolderTransfrom = myTransform.GetChild(1);
        buildingBlockTransform = myTransform.GetChild(2);

        craneAnimator = myTransform.GetComponentInChildren<Animator>();

        towerTransform = GameObject.Find("Tower").transform;
    }

    void Start()
    {
        Invoke("DelayCraneSwingingAnimation", swingingAnimationDelayTime);
    }

    void Update()
    {
        if (timerCountdown > 0)
        {
            timerCountdown -= Time.deltaTime;
        }

        if (timerCountdown <= 0)
        {
            canDetach = true;
        }

        if (shouldPullupCrane)
            PullupTheCraneUnit();

        if (shouldPulldownCrane) //Called From GameManager.
            PulldownTheCraneUnit();
    }

    void DelayCraneSwingingAnimation() //Called in invoke method in Start function.
    {
        craneAnimator.enabled = true;
    }

    public void DetachBuildingBlock()
    {
        if (!canDetach)
            return;

        buildingBlockTransform.gameObject.layer = 0; //Set for Default Layer.
        for (int i = 0; i < buildingBlockTransform.childCount; i++)
        {
            Transform towerBlock = buildingBlockTransform.GetChild(i);
            towerBlock.gameObject.layer = 0;

            for (int x = 0; x < towerBlock.childCount; x++)
            {
                towerBlock.GetChild(x).gameObject.layer = 0;
            }
        }

        craneHolderTransfrom.gameObject.SetActive(false);

        buildingBlockTransform.parent = null;

        shouldPullupCrane = true;
    }

    void PullupTheCraneUnit()
    {
        shouldPulldownCrane = false;

        if (!gameManager.isGameFinished)
        {
            if (!isInstantiateCommandSent)
            {
                gameManager.InstantiateCraneUnit();
                isInstantiateCommandSent = true;
            }
        }

        destroyTimerCountdown -= Time.deltaTime;

        myTransform.Translate(Vector3.up * Time.deltaTime * pullupSpeed);

        if (destroyTimerCountdown < 0)
            Destroy(myTransform.gameObject);
    }

    void PulldownTheCraneUnit()
    {
        craneCurrentPos = gameManager.craneCurrentPos;

        if ((int)myTransform.position.x != (int)craneCurrentPos.x || (int)myTransform.position.y != (int)craneCurrentPos.y ||
            (int)myTransform.position.z != (int)craneCurrentPos.z)
        {
            myTransform.position = Vector3.Lerp(myTransform.position, craneCurrentPos, pulldownSpeed * Time.deltaTime);
        }
        else
        {
            shouldPulldownCrane = false;
        }
    }
}