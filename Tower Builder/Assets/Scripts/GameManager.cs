using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using TMPro;

public class GameManager : MonoBehaviour
{
    [Header("Instantiating")]
    public GameObject craneUnitPrefab;

    public Vector3 craneStartingPos = new Vector3(0f, 6f, 0f);
    public Vector3 craneOffsetPos = new Vector3(0f, 5f, 0f);
    public Quaternion craneRotation = new Quaternion();

    public Transform towerTransform;
    int towerChildsCount;
    Vector3 lastbuildingBlockPos;
    Vector3 offset;

    public Text blocksCountText;

    public Vector3 craneCurrentPos;
    Vector3 craneInstantiatePos;

    Crane lastInstantiatedCrane;
    public GameObject lastInstantiatedCraneGO;

    Transform towerBlocksParentTransform;
    Transform towerBlocksParentTransformPrefab;

    [Header("Difficulty Configuration")]
    public int difficultyLevelInt;
    public DifficultyLevel difficultyLevel;
    public float drag;
    public float angularDrag;
    public float forceProportionalFactor;
    public float torqueProportionalFactor;

    public TMP_Dropdown difficultyLevelDropdown;

    public enum DifficultyLevel { Easy, Normal, Hard }

    [Header("BuildingBlocks IsKinematic")]
    public int blocksCountThreshold = 8;
    int lastKinematicBlockChildInt;
    bool isInvokingKinematicFalse;

    [Header("Coins")]
    public int coinsAmount;
    public int coinsPerBlock;

    public int totalCoinsEarned;

    public Text[] totalCoinsEarnedTexts;
    public Text[] coinsAmountTexts;

    [Header("Highscore")]
    public bool isHighscoreRunning;
    public float initialAllowedTime = 3f;
    public int consecutiveBlocksJoiningCount;
    int normalCoinsPerBlock;
    float currentRunningTime;
    public GameObject highscoreTimerUIParent;
    public Text consecutiveBlockCountText;
    public Image[] highscoreLoadingBarsImages;

    [Header("Lost Blocks")]
    public bool isGameFinished;
    bool isHeadBlockInstantiated;
    public int lostBlocksAmount;
    public int permittedLostBlocks = 3;
    public Transform lostBlocksImagesParent;
    public VerticalLayoutGroup lostBlockImagesLayoutGroup;

    public Transform[] headBlocksPrefabs;

    [Header("Game Finished")]
    public float gameFinishedTransitionTimeLength = 2.5f;

    public GameObject gameplayPanel;
    public GameObject gameFinishedPanel;

    [Header("Upgrades")]
    public int lostBlocksUpgradeCoinsPrice = 100;
    public int maximumPermittedLostBlocksUpgrade = 10;
    public int lostBlocksUpgradeCoinsPriceFactor = 150;


    public Image lostBlockUpgradeIndicatorImage;
    public Text lostBlocksUpgradeCoinsPriceText;
    public Text permittedLostBlockCountText;
    public Button lostBlockUpgradeButton;

    [Header("Block Unlocking")]
    public int blocksCoinsPrice;
    public int blocksCoinsPriceFactor = 200;
    public int[] unlockedBlocksInts;

    public GameObject lockImageGameobject;
    public Button goButton;
    public Text blocksCoinsPriceText;

    [Header("Locations Unlocking")]
    public int locationCoinsPrice;
    public int locationCoinsPriceFactor = 300;
    public int[] unlockedLocationsInts;

    public GameObject lockImageGameobject_Location;
    public Button playButton;
    public Text locationCoinsPriceText;

    [Header("Error Messages")]
    public float messageShowupTimelength = 3f;
    public GameObject noEnoughCoinsMessageGO;

    Coroutine noEnoughCoinsMessageCoroutine;

    [Header("Tower Blocks Selector")]
    public int towerBlockInt;

    [Header("Tower Location Selector")]
    public bool isTowerAlignedOnXaxis;
    public int towerLocationInt;
    public float cameraMoveSpeed = 5f;
    bool shouldCameraMove;
    public TrafficManager trafficManager;
    public List<TowerLocationsProperties> towerLocationsProperties = new List<TowerLocationsProperties>();

    Transform mainCameraTransform;

    [System.Serializable]
    public class TowerLocationsProperties
    {
        public string towerLoactionName = "Tower";

        [Header("Camera")]
        public Vector3 cameraPosition;
        public Vector3 cameraRotation;

        [Header("Crane")]
        public Vector3 cranePosition;
        public Quaternion craneRotation;

        [Header("Tower")]
        public bool isAlignedOnXaxis;
        public Vector3 towerPosition;
        public Vector3 towerRotation;

        [Header("Traffic")]
        public Transform trafficCheckpoint;
    }

    void Awake()
    {
        LoadSavedData();

        mainCameraTransform = Camera.main.transform;

        towerBlocksParentTransformPrefab = craneUnitPrefab.transform.GetChild(2);

        GetTowerBlockInt();

        SelectTowerLocation();

        craneCurrentPos = craneStartingPos;

        offset = craneStartingPos;

        SetDifficultyLevelParameters();

        normalCoinsPerBlock = coinsPerBlock;

        ReloadTowerLocationSelection();

        SetLostBlocksUI();

        SetCoinsPriceForLostBlockUpgrade();

        OpenArrayForUnlockedBlockInt();
        OpenArrayForUnlockedLocationInt();
    }

    void Start()
    {
        InstantiateCraneUnit();
    }

    void Update()
    {
        SetCraneCurrentPos();
        UpdateBuildingBlocksCountText();
        UpdateCoinsUI();
        UpdateUpgradesUI();
        SetCoinsPriceForTowerBlocksUnlocking();
        UpdateBlocksUnlockingUI();
        SetCoinsPriceForLocationsUnlocking();
        UpdateLocationsUnlockingUI();

        if (isGameFinished && lastInstantiatedCraneGO == null && !isHeadBlockInstantiated)
        {
            isHeadBlockInstantiated = true;
            FinalizeTowerWithProperHeadBlock();
        }

        if (shouldCameraMove)
        {
            var selectedTowerLocationProperties = towerLocationsProperties[towerLocationInt];
            MoveCamera(selectedTowerLocationProperties.cameraPosition, selectedTowerLocationProperties.cameraRotation);
        }

        if (isHighscoreRunning)
            StartHighscoreRewardTimer();
    }

    void LoadSavedData()
    {
        if (DataSaveManager.IsDataExist("TotalCoinsEarned"))
            totalCoinsEarned = (int)DataSaveManager.LoadData("TotalCoinsEarned");

        if (DataSaveManager.IsDataExist("PermittedLossBlocksCount"))
            permittedLostBlocks = (int)DataSaveManager.LoadData("PermittedLossBlocksCount");

        if (DataSaveManager.IsDataExist("UnlockedBlocksInts"))
            unlockedBlocksInts = (int[])DataSaveManager.LoadData("UnlockedBlocksInts");

        if (DataSaveManager.IsDataExist("UnlockedLocationsInts"))
            unlockedLocationsInts = (int[])DataSaveManager.LoadData("UnlockedLocationsInts");

        if (DataSaveManager.IsDataExist("TowerLocationInt"))
            towerLocationInt = (int)DataSaveManager.LoadData("TowerLocationInt");

        if (DataSaveManager.IsDataExist("DifficultyLevelInt"))
            difficultyLevelDropdown.value = (int)DataSaveManager.LoadData("DifficultyLevelInt");
    }

    void SaveData()
    {
        DataSaveManager.SaveData("TotalCoinsEarned", totalCoinsEarned);
        DataSaveManager.SaveData("PermittedLossBlocksCount", permittedLostBlocks);
        DataSaveManager.SaveData("UnlockedBlocksInts", unlockedBlocksInts);
        DataSaveManager.SaveData("UnlockedLocationsInts", unlockedLocationsInts);
    }

    public void StartGame()
    {
        SetLostBlocksUI();
        shouldCameraMove = false;

        TinySauce.OnGameStarted();
    }

    public void InstantiateCraneUnit() //Called also From Crane.
    {
        craneInstantiatePos = craneCurrentPos + craneOffsetPos;

        lastInstantiatedCraneGO = Instantiate(craneUnitPrefab, craneInstantiatePos, craneRotation);
        towerBlocksParentTransform = lastInstantiatedCraneGO.transform.GetChild(2);
        lastInstantiatedCrane = lastInstantiatedCraneGO.transform.GetComponent<Crane>();
        lastInstantiatedCrane.gameManager = this;

        lastInstantiatedCrane.shouldPulldownCrane = true;
    }

    void SetCraneCurrentPos()
    {
        towerChildsCount = (towerTransform.childCount - 1);
        lastbuildingBlockPos = towerTransform.GetChild(towerChildsCount).position;

        craneCurrentPos = new Vector3(lastbuildingBlockPos.x, (lastbuildingBlockPos + offset).y, lastbuildingBlockPos.z);
    }

    public void DetachBuildingBlockFromCrane() //Called from UI button.
    {
        MakeBuildingBlocksIsKinematic();

        if (lastInstantiatedCrane != null)
            lastInstantiatedCrane.DetachBuildingBlock();
    }

    void UpdateBuildingBlocksCountText()
    {
        blocksCountText.text = towerChildsCount.ToString();
    }

    public void ReloadScene()
    {
        GameEnded();

        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }

    public void SetDifficultyLevelParameters()
    {
        difficultyLevelInt = difficultyLevelDropdown.value;
        difficultyLevel = (DifficultyLevel)difficultyLevelInt;

        DataSaveManager.SaveData("DifficultyLevelInt", difficultyLevelInt);

        if (difficultyLevel == DifficultyLevel.Easy)
        {
            drag = 0.2f;
            angularDrag = 6f;

            forceProportionalFactor = 20000f;
            torqueProportionalFactor = 20000f;

            blocksCountThreshold = 8;
        }

        if (difficultyLevel == DifficultyLevel.Normal)
        {
            drag = 0.1f;
            angularDrag = 3f;

            forceProportionalFactor = 16000f;
            torqueProportionalFactor = 20000f;

            blocksCountThreshold = 8;
        }

        if (difficultyLevel == DifficultyLevel.Hard)
        {
            drag = 0f;
            angularDrag = 0.05f;

            forceProportionalFactor = 14000f;
            torqueProportionalFactor = 18000f;

            blocksCountThreshold = 8;
        }
    }

    void MakeBuildingBlocksIsKinematic()
    {
        if ((towerChildsCount + 1) >= blocksCountThreshold)
        {
            lastKinematicBlockChildInt = ((towerChildsCount + 1) - blocksCountThreshold);

            towerTransform.GetChild(lastKinematicBlockChildInt).GetComponent<BuildingBlock>().SetRigidbodyToIsKinematic();
        }
    }

    public void InvokeSetIsKinematicToFalse()
    {
        if (!isInvokingKinematicFalse)
        {
            isInvokingKinematicFalse = true;
            Invoke("SetIsKinematicToFalse", 0.2f);
        }
    }

    void SetIsKinematicToFalse() //After Joint Break, Called from OnJointBreak in BuildingBlock.
    {
        if ((towerChildsCount + 1) >= blocksCountThreshold)
        {
            lastKinematicBlockChildInt = ((towerChildsCount + 1) - blocksCountThreshold);

            for (int i = lastKinematicBlockChildInt; i < (towerChildsCount - 1); i++)
            {
                towerTransform.GetChild(i).GetComponent<BuildingBlock>().SetIsKinematicToFalse();
            }

            isInvokingKinematicFalse = false;
        }
    }

    void SelectTowerLocation()
    {
        var selectedTowerLocationProperties = towerLocationsProperties[towerLocationInt];

        if (!shouldCameraMove)
        {
            mainCameraTransform.position = selectedTowerLocationProperties.cameraPosition;
            mainCameraTransform.eulerAngles = selectedTowerLocationProperties.cameraRotation;
        }

        craneStartingPos = selectedTowerLocationProperties.cranePosition;
        craneRotation = selectedTowerLocationProperties.craneRotation;
        if (lastInstantiatedCrane != null)
        {
            Destroy(lastInstantiatedCraneGO);
            Invoke("InstantiateCraneUnit", 1f);
        }

        isTowerAlignedOnXaxis = selectedTowerLocationProperties.isAlignedOnXaxis;
        towerTransform.position = selectedTowerLocationProperties.towerPosition;
        towerTransform.eulerAngles = selectedTowerLocationProperties.towerRotation;

        trafficManager.checkpointsParent = selectedTowerLocationProperties.trafficCheckpoint;
    }

    public void BrowsingTowerLocations(int buttonIndex)
    {
        if (buttonIndex == 0)
        {
            if (towerLocationInt > 0)
            {
                towerLocationInt--;

                ReloadTowerLocationSelection();
            }
        }

        if (buttonIndex == 1)
        {
            if (towerLocationInt < (towerLocationsProperties.Count - 1))
            {
                towerLocationInt++;

                ReloadTowerLocationSelection();
            }
        }
    }

    void ReloadTowerLocationSelection()
    {
        shouldCameraMove = true;
        SelectTowerLocation();

        trafficManager.ReloadTraffic();

        DataSaveManager.SaveData("TowerLocationInt", towerLocationInt);
    }

    void MoveCamera(Vector3 cameraPos, Vector3 cameraRot)
    {
        mainCameraTransform.position = Vector3.Lerp(mainCameraTransform.position, cameraPos, cameraMoveSpeed * Time.deltaTime);
        mainCameraTransform.eulerAngles = Vector3.Lerp(mainCameraTransform.eulerAngles, cameraRot, cameraMoveSpeed * Time.deltaTime);

        mainCameraTransform.GetComponent<CameraFollow>().SetTheCameraOffset();
    }

    void GetTowerBlockInt()
    {
        for (int i = 1; i < towerBlocksParentTransformPrefab.childCount; i++)
        {
            if (towerBlocksParentTransformPrefab.GetChild(i).gameObject.activeSelf)
                towerBlockInt = i;
        }
    }

    public void BrowsingTowerBlocks(int buttonIndex)
    {
        if (buttonIndex == 0)
        {
            if (towerBlockInt > 1)
            {
                towerBlockInt--;

                SelectTowerBlock();
            }
        }

        if (buttonIndex == 1)
        {
            if (towerBlockInt < (towerBlocksParentTransform.childCount - 1))
            {
                towerBlockInt++;

                SelectTowerBlock();
            }
        }
    }

    void SelectTowerBlock()
    {
        //For Current Instantiated Crane.
        for (int i = 1; i < towerBlocksParentTransform.childCount; i++)
        {
            towerBlocksParentTransform.GetChild(i).gameObject.SetActive(false);
        }

        towerBlocksParentTransform.GetChild(towerBlockInt).gameObject.SetActive(true);




        for (int i = 1; i < towerBlocksParentTransformPrefab.childCount; i++)
        {
            towerBlocksParentTransformPrefab.GetChild(i).gameObject.SetActive(false);
        }

        towerBlocksParentTransformPrefab.GetChild(towerBlockInt).gameObject.SetActive(true);
    }

    public void CalculateCoins() //Called from BuildingBlock.
    {
        coinsAmount += coinsPerBlock;
    }

    public void TowerBlocksBreaked() //Called from BuildingBlock.
    {
        print("Block Break");

        coinsAmount -= normalCoinsPerBlock;
    }

    void AddTheEarnedCoins()
    {
        totalCoinsEarned += coinsAmount;

        SaveData();
    }

    void UpdateCoinsUI()
    {
        foreach (Text coinsAmountText in coinsAmountTexts)
            coinsAmountText.text = coinsAmount.ToString();

        foreach (Text totalCoinsEarnedText in totalCoinsEarnedTexts)
            totalCoinsEarnedText.text = totalCoinsEarned.ToString();
    }

    void SetLostBlocksUI()
    {
        for (int i = 0; i < lostBlocksImagesParent.childCount; i++) //Disable All Objects.
        {
            lostBlocksImagesParent.GetChild(i).gameObject.SetActive(false);
        }
        for (int i = 0; i < permittedLostBlocks; i++) //Enable the permitted One.
        {
            lostBlocksImagesParent.GetChild(i).gameObject.SetActive(true);
        }

        lostBlockImagesLayoutGroup.spacing = -(1000 - (permittedLostBlocks * 100));
    }

    public void BlockIsLost() //Called from BuildingBlock.
    {
        lostBlocksAmount++;

        if ((lostBlocksAmount - 1) < lostBlocksImagesParent.childCount)
            lostBlocksImagesParent.GetChild(lostBlocksAmount - 1).GetComponent<Image>().color = Color.red;

        if (lostBlocksAmount >= permittedLostBlocks && !isGameFinished)
            isGameFinished = true;
    }

    void FinalizeTowerWithProperHeadBlock()
    {
        if (towerTransform.childCount > 1)
        {
            Instantiate(headBlocksPrefabs[(towerBlockInt - 1)], (lastbuildingBlockPos + (Vector3.up * 25)), towerTransform.GetChild(1).rotation);
        }

        Invoke("GameFinished", gameFinishedTransitionTimeLength);
    }

    void GameFinished() //Callled from FinalizeTowerWithProperHeadBlock.
    {
        gameplayPanel.SetActive(false);
        gameFinishedPanel.SetActive(true);
    }

    void GameEnded() // Game ended by player (main menu button)
    {
        AddTheEarnedCoins();

        TinySauce.OnGameFinished(coinsAmount);
    }

    public void CheckForConsecutiveBlocksJoining() //Called from BuildingBlock.
    {
        consecutiveBlocksJoiningCount++;

        if (consecutiveBlocksJoiningCount >= 1)
        {
            isHighscoreRunning = true;

            coinsPerBlock *= (consecutiveBlocksJoiningCount + 1);
            currentRunningTime = initialAllowedTime;

            StartHighscoreRewardTimer();
        }
    }

    void StartHighscoreRewardTimer()
    {
        if (currentRunningTime < 0 || isGameFinished)
        {
            highscoreTimerUIParent.SetActive(false);

            consecutiveBlocksJoiningCount = 0;
            coinsPerBlock = normalCoinsPerBlock;

            isHighscoreRunning = false;
        }
        else
        {
            currentRunningTime -= Time.deltaTime;

            highscoreTimerUIParent.SetActive(true);

            UpdateHighscoreTimerUI();
        }
    }

    void UpdateHighscoreTimerUI()
    {
        foreach (Image loadingBarImage in highscoreLoadingBarsImages)
        {
            loadingBarImage.fillAmount = (currentRunningTime / initialAllowedTime);
        }

        consecutiveBlockCountText.text = consecutiveBlocksJoiningCount.ToString();
    }

    void SetCoinsPriceForLostBlockUpgrade()
    {
        lostBlocksUpgradeCoinsPrice = permittedLostBlocks * lostBlocksUpgradeCoinsPriceFactor;
    }

    public void UpgradeLostBlockCount()
    {
        if (permittedLostBlocks < maximumPermittedLostBlocksUpgrade)
        {
            if (totalCoinsEarned >= lostBlocksUpgradeCoinsPrice)
            {
                totalCoinsEarned -= lostBlocksUpgradeCoinsPrice;

                permittedLostBlocks++;

                SetCoinsPriceForLostBlockUpgrade();

                SaveData();
            }
            else
            {
                if (noEnoughCoinsMessageCoroutine != null)
                    StopCoroutine(noEnoughCoinsMessageCoroutine);

                noEnoughCoinsMessageCoroutine = StartCoroutine(NoEnoughCoins());
            }
        }
    }

    void UpdateUpgradesUI()
    {
        lostBlockUpgradeIndicatorImage.fillAmount = ((float)permittedLostBlocks / (float)maximumPermittedLostBlocksUpgrade);
        lostBlocksUpgradeCoinsPriceText.text = lostBlocksUpgradeCoinsPrice.ToString();
        permittedLostBlockCountText.text = permittedLostBlocks.ToString();

        if (permittedLostBlocks == maximumPermittedLostBlocksUpgrade)
            lostBlockUpgradeButton.gameObject.SetActive(false);
    }

    IEnumerator NoEnoughCoins()
    {
        noEnoughCoinsMessageGO.SetActive(true);

        yield return new WaitForSeconds(messageShowupTimelength);

        noEnoughCoinsMessageGO.SetActive(false);
    }

    void OpenArrayForUnlockedBlockInt()
    {
        if (!DataSaveManager.IsDataExist("UnlockedBlocksInts"))
        {
            unlockedBlocksInts = new int[(towerBlocksParentTransformPrefab.childCount - 1)];
            unlockedBlocksInts[0] = 1;
        }
    }

    void SetCoinsPriceForTowerBlocksUnlocking()
    {
        blocksCoinsPrice = towerBlockInt * blocksCoinsPriceFactor;
    }

    public void UnlockTowerBlocks()
    {
        if (totalCoinsEarned >= blocksCoinsPrice)
        {
            totalCoinsEarned -= blocksCoinsPrice;

            unlockedBlocksInts[(towerBlockInt - 1)] = towerBlockInt;

            SaveData();
        }
        else
        {
            if (noEnoughCoinsMessageCoroutine != null)
                StopCoroutine(noEnoughCoinsMessageCoroutine);

            noEnoughCoinsMessageCoroutine = StartCoroutine(NoEnoughCoins());
        }
    }

    void UpdateBlocksUnlockingUI()
    {
        if (unlockedBlocksInts[(towerBlockInt - 1)] == 0)
        {
            lockImageGameobject.SetActive(true);
            blocksCoinsPriceText.text = blocksCoinsPrice.ToString();
            goButton.interactable = false;
        }
        else
        {
            lockImageGameobject.SetActive(false);
            goButton.interactable = true;
        }
    }

    void OpenArrayForUnlockedLocationInt()
    {
        if (!DataSaveManager.IsDataExist("UnlockedLocationsInts"))
        {
            unlockedLocationsInts = new int[towerLocationsProperties.Count];
            unlockedLocationsInts[0] = 1;
        }
    }

    void SetCoinsPriceForLocationsUnlocking()
    {
        locationCoinsPrice = towerBlockInt * locationCoinsPriceFactor;
    }

    public void UnlockTowerLocations()
    {
        if (totalCoinsEarned >= locationCoinsPrice)
        {
            totalCoinsEarned -= locationCoinsPrice;

            unlockedLocationsInts[towerLocationInt] = towerLocationInt;

            SaveData();
        }
        else
        {
            if (noEnoughCoinsMessageCoroutine != null)
                StopCoroutine(noEnoughCoinsMessageCoroutine);

            noEnoughCoinsMessageCoroutine = StartCoroutine(NoEnoughCoins());
        }
    }

    void UpdateLocationsUnlockingUI()
    {
        if (unlockedLocationsInts[towerLocationInt] == 0)
        {
            lockImageGameobject_Location.SetActive(true);
            locationCoinsPriceText.text = locationCoinsPrice.ToString();
            playButton.interactable = false;
        }
        else
        {
            lockImageGameobject_Location.SetActive(false);
            playButton.interactable = true;
        }
    }
}