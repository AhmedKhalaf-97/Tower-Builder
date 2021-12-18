using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BuildingBlock : MonoBehaviour
{
    bool isCollisionWorking = true;
    bool startDetroying;

    public bool isJointBroke;

    public bool isHeadBlock;

    bool isBlockBroke;

    bool isBlockLost;

    float timerToDestroy = 1.5f;

    float proximityValue;
    float forceProportionalFactor = 800;
    float torqueProportionalFactor = 1000;
    float jointBreakForce;
    float jointBreakTorque;

    Transform towerTransform;
    Transform myTransform;
    FixedJoint fixedJoint;
    Rigidbody myRigidbody;
    ParticleSystem dustParticleSystem;

    CameraFollow cameraFollow;

    Rigidbody incomingRigidbody;
    Transform incomingTransform;
    BuildingBlock incomingBuildingBlock;

    GameManager gameManager;

    void OnCollisionEnter(Collision collision)
    {
        if (isCollisionWorking)
        {
            if (collision.transform.tag == "BuildingBlock" && collision.transform.parent == null && myTransform.tag == "Untagged")
            {
                if (((collision.transform.position.y) - (myTransform.position.y)) > 3.8f) //To Prevent Sticking From Sides.
                {
                    fixedJoint = gameObject.AddComponent<FixedJoint>();
                    incomingTransform = collision.transform;
                    incomingRigidbody = incomingTransform.GetComponent<Rigidbody>();
                    incomingBuildingBlock = incomingTransform.GetComponent<BuildingBlock>();

                    incomingBuildingBlock.PlayDustParticleSystem();

                    incomingTransform.parent = towerTransform;
                    incomingTransform.tag = "Untagged";

                    if (gameManager.isTowerAlignedOnXaxis)
                        incomingTransform.position = new Vector3(incomingTransform.position.x, incomingTransform.position.y, gameManager.craneStartingPos.z);
                    else
                        incomingTransform.position = new Vector3(gameManager.craneStartingPos.x, incomingTransform.position.y, incomingTransform.position.z);

                    incomingTransform.eulerAngles = myTransform.eulerAngles;

                    incomingRigidbody.freezeRotation = false;
                    fixedJoint.connectedBody = incomingRigidbody;

                    if (gameManager.isTowerAlignedOnXaxis)
                        proximityValue = Mathf.Abs(incomingTransform.position.x - myTransform.position.x);
                    else
                        proximityValue = Mathf.Abs(incomingTransform.position.z - myTransform.position.z);

                    fixedJoint.breakForce = GetFixedJointBreakForce();
                    fixedJoint.breakTorque = GetFixedJointBreakToque();

                    cameraFollow.ShakeCamera();
                    gameManager.CalculateCoins();

                    if (proximityValue < 0.6)
                        gameManager.CheckForConsecutiveBlocksJoining();

                    isCollisionWorking = false;
                }
            }
        }

        if (collision.transform.tag == "Ground")
        {
            startDetroying = true;
        }
    }

    void OnJointBreak()
    {
        incomingBuildingBlock.isJointBroke = true;
        isCollisionWorking = true;

        if (towerTransform.GetChild(0) != myTransform) //If not first block
            SetIsKinematicToFalse();

        Invoke("PullDownCraneAfterJointBreak", 0.5f);

        gameManager.InvokeSetIsKinematicToFalse();
    }

    void Awake()
    {
        myTransform = transform;
        myRigidbody = GetComponent<Rigidbody>();
        dustParticleSystem = GetComponentInChildren<ParticleSystem>();
        towerTransform = GameObject.Find("Tower").transform;

        cameraFollow = Camera.main.GetComponent<CameraFollow>();

        gameManager = GameObject.Find("GameManager").transform.GetComponent<GameManager>();
    }

    void Start()
    {
        SetTheParameters();
    }

    void Update()
    {
        if (myTransform.tag == "Untagged" && myTransform.parent != towerTransform && !isJointBroke) //Sometimes it doesn't parent.
            myTransform.parent = towerTransform;

        if (isJointBroke)
        {
            myTransform.parent = null;

            if (fixedJoint != null)
                Destroy(fixedJoint);

            if (incomingTransform != null)
                incomingBuildingBlock.isJointBroke = true;

            if (!isBlockBroke)
            {
                isBlockBroke = true;
                gameManager.TowerBlocksBreaked();
            }
        }

        if (myTransform.parent == null && !isHeadBlock) // If not attached to tower it will be destroyed.
        {
            if (timerToDestroy < 0)
            {
                if (isCollisionWorking && !isJointBroke && !isBlockLost)
                {
                    isBlockLost = true;
                    gameManager.BlockIsLost();
                }

                startDetroying = true;
            }
            else
            {
                timerToDestroy -= Time.deltaTime;
            }
        }

        if (startDetroying)
            StartDestroying();
    }

    void StartDestroying()
    {
        myTransform.gameObject.layer = 9; // Destruction layer int.

        Destroy(gameObject, 2f);
    }

    float GetFixedJointBreakForce() //Depending On Proximity.
    {
        jointBreakForce = (forceProportionalFactor / (Mathf.Pow((proximityValue + 0.001f), 2f)));

        return jointBreakForce;
    }

    float GetFixedJointBreakToque() //Depending On Proximity.
    {
        jointBreakTorque = (torqueProportionalFactor / (Mathf.Pow((proximityValue + 0.001f), 2f)));

        return jointBreakTorque;
    }

    void PullDownCraneAfterJointBreak() //Called in OnJointBrake.
    {
        gameManager.lastInstantiatedCraneGO.GetComponent<Crane>().shouldPulldownCrane = true;
    }

    void SetTheParameters()
    {
        myRigidbody.drag = gameManager.drag;
        myRigidbody.angularDrag = gameManager.angularDrag;

        forceProportionalFactor = gameManager.forceProportionalFactor;
        torqueProportionalFactor = gameManager.torqueProportionalFactor;
    }

    public void SetRigidbodyToIsKinematic() //Called From GameManager.
    {
        if (!myRigidbody.isKinematic)
            myRigidbody.isKinematic = true;

        if (fixedJoint.breakForce != Mathf.Infinity && fixedJoint.breakTorque != Mathf.Infinity)
        {
            fixedJoint.breakForce = Mathf.Infinity;
            fixedJoint.breakTorque = Mathf.Infinity;
        }
    }

    public void SetIsKinematicToFalse()
    {
        if (myRigidbody.isKinematic)
            myRigidbody.isKinematic = false;
    }

    public void PlayDustParticleSystem()
    {
        dustParticleSystem.Play();

        Invoke("DisableDustParticleSystem", 3f);
    }

    void DisableDustParticleSystem()
    {
        dustParticleSystem.gameObject.SetActive(false);
    }
}