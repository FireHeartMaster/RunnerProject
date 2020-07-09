using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using System.Runtime.Serialization.Formatters.Binary;
using System.IO;
using UnityEngine.SceneManagement;

public class GameplayManager : MonoBehaviour
{
    [SerializeField] float minimumTimeToFirstInvertGravity = 10f;

    [SerializeField] float minTimeLimitForGravityInversion = 10f;
    [SerializeField] float maxTimeLimitForGravityInversion = 30f;

    float timeSinceLastGravityInversion = 0;

    float currentGravityInversionInterval;

    Vector3 originalGravity;
    Vector3 fullGravity;

    [SerializeField] float gravityInversionSpeed = 2f;

    [SerializeField] GameObject gravityInversionAnimation;
    [SerializeField] Slider gravitySlider;

    [SerializeField] PlayerStats stats;

    public static GameplayManager gameplayManager;

    bool canInvertGravity = true;

    public bool canStartGame = false;

    [SerializeField] CameraFollow cameraFollow;

    TutorialState tutorialState = TutorialState.state0;

    [ContextMenu("Start Game")]
    public void StartGame()
    {
        canStartGame = true;

        currentGravityInversionInterval = minimumTimeToFirstInvertGravity + Random.Range(minTimeLimitForGravityInversion, maxTimeLimitForGravityInversion);

        Physics.gravity = new Vector3(0f, -9.81f, 0f);

        originalGravity = Physics.gravity;
        fullGravity = Physics.gravity;

        gravityInversionAnimation.SetActive(false);

        gravitySlider.value = (Physics.gravity.y + 1) * 0.5f;

        deathScreen.SetActive(false);
        deathScreenNewRecordLabel.SetActive(false);

        stats.GetComponent<MovePlayer>().StartMoving();
        stats.GetComponent<DetectPlayerCollision>().StartDetectingCollisions();
        SoundManager.soundManager.StartBackgroundMusic();
        cameraFollow.CameraCanFollow();
        obstacleManager.StartObstacleManager();
    }

    public void ResetGameplaymanager()
    {
        timeSinceLastGravityInversion = 0;
        canInvertGravity = true;

        if (gravityInversionCoroutine != null)
        {
            StopCoroutine(gravityInversionCoroutine);
        }

        if(deathScreenDelayedActivationCoroutine != null)
        {
            StopCoroutine(deathScreenDelayedActivationCoroutine);
        }

        gravityInversionAnimation.SetActive(false);

        deathScreen.SetActive(false);
        deathScreenNewRecordLabel.SetActive(false);

        tutorialState = TutorialState.state0;
        if(tutorialCoroutine != null)
        {
            StopCoroutine(tutorialCoroutine);
            for (int i = 0; i < tutorialObjects.Length; i++)
            {
                tutorialObjects[i].SetActive(false);
            }
        }

        StartGame();
    }

    [SerializeField] ObstacleManager obstacleManager;
    public void ResetAll()
    {
        SoundManager.soundManager.ResetSounds();
        stats.ResetPlayerStats();
        stats.GetComponent<DetectPlayerCollision>().ResetDetectCollisions();
        cameraFollow.CameraFollowReset();
        stats.GetComponent<MovePlayer>().ResetMovePlayer();
        Pooling.pooling.ResetPooling();
        obstacleManager.ResetObstacleManager();

        ResetGameplaymanager();
    }

    private void Awake()
    {
        if(gameplayManager == null)
        {
            gameplayManager = this;
        }
        else
        {
            Destroy(gameObject);
        }
    }

    private void Start()
    {
        //StartGame();
        //currentGravityInversionInterval = minimumTimeToFirstInvertGravity + Random.Range(minTimeLimitForGravityInversion, maxTimeLimitForGravityInversion);

        //Physics.gravity = new Vector3(0f, -9.81f, 0f);

        //originalGravity = Physics.gravity;
        //fullGravity = Physics.gravity;

        //gravityInversionAnimation.SetActive(false);

        //gravitySlider.value = (Physics.gravity.y + 1) * 0.5f;

        //deathScreen.SetActive(false);
        //deathScreenNewRecordLabel.SetActive(false);
    }

    [SerializeField] bool simulate = false;
    private void Update()
    {
        if (canStartGame)
        {
            if (canInvertGravity)
            {
                timeSinceLastGravityInversion += Time.deltaTime;

                if (timeSinceLastGravityInversion >= currentGravityInversionInterval)
                {
                    //invert gravity
                    timeSinceLastGravityInversion = 0;

                    currentGravityInversionInterval = Random.Range(minTimeLimitForGravityInversion, maxTimeLimitForGravityInversion);

                    //Physics.gravity *= -1;
                    if (gravityInversionCoroutine != null)
                    {
                        StopCoroutine(gravityInversionCoroutine);
                    }

                    gravityInversionCoroutine = InvertGravity();
                    StartCoroutine(gravityInversionCoroutine);
                }
            }


            if(Time.timeScale == 0f && 
                ((!simulate && Input.touchCount > 0 && Input.GetTouch(0).phase == TouchPhase.Began) ||
                simulate && Input.GetMouseButtonDown(0)))
            {
                Time.timeScale = 1f;
            }

            //Debug.Log("gravity: " + Physics.gravity.y);
        }
    }

    IEnumerator gravityInversionCoroutine;
    IEnumerator InvertGravity()
    {
        //fullGravity = originalGravity * Mathf.Sign(Physics.gravity.y);
        fullGravity *= -1;

        gravityInversionAnimation.SetActive(true);

        Vector3 animationScale = gravityInversionAnimation.transform.localScale;
        animationScale.y = Mathf.Sign(fullGravity.y);
        gravityInversionAnimation.transform.localScale = animationScale;

        while (Physics.gravity.y * Mathf.Sign(fullGravity.y) < fullGravity.y * Mathf.Sign(fullGravity.y))
        {
            Vector3 currentGravity = Physics.gravity;
            currentGravity.y += gravityInversionSpeed * Time.deltaTime * Mathf.Sign(fullGravity.y);
            Physics.gravity = currentGravity;

            gravitySlider.value = Remap01(Physics.gravity.y, -9.81f * Mathf.Sign(fullGravity.y), 9.81f * Mathf.Sign(fullGravity.y));
            //gravitySlider.value = Remap01(Physics.gravity.y, -9.81f, 9.81f);
            //gravitySlider.value = (Physics.gravity.y + 1) * 0.5f;

            yield return new WaitForSeconds(Time.deltaTime);
        }

        Physics.gravity = fullGravity;

        gravityInversionAnimation.SetActive(false);
    }


    float Remap01(float value, float minLimit, float maxLimit)
    {
        return (value - minLimit) / (maxLimit - minLimit);
    }

    IEnumerator deathScreenDelayedActivationCoroutine;
    public void DeathScreen()
    {
        canInvertGravity = false;

        if (deathScreenDelayedActivationCoroutine != null)
        {
            StopCoroutine(deathScreenDelayedActivationCoroutine);
        }
        deathScreenDelayedActivationCoroutine = DeathScreenDelayedActivation();
        StartCoroutine(deathScreenDelayedActivationCoroutine);
    }

    [SerializeField] float timeToDelayDeathScreen = 1f;
    IEnumerator DeathScreenDelayedActivation()
    {
        yield return new WaitForSeconds(timeToDelayDeathScreen);
        ActivateDeathScreen();
    }

    [SerializeField] GameObject deathScreen;
    [SerializeField] TextMeshProUGUI deathScreenPoints;
    [SerializeField] TextMeshProUGUI deathScreenRecordPoints;
    [SerializeField] GameObject deathScreenNewRecordLabel;
    void ActivateDeathScreen()
    {
        gravityInversionAnimation.SetActive(false);

        deathScreen.SetActive(true);
        deathScreenPoints.text = stats.AmountOfPoints.ToString();

        HandleNewRecord();
    }

    void HandleNewRecord()
    {
        SaveData loadedData = LoadDataFromMemory();

        float best = loadedData.maxPoints;
        if (stats.AmountOfPoints > loadedData.maxPoints)
        {
            SaveNewRecord(stats.AmountOfPoints);
            deathScreenNewRecordLabel.SetActive(true);
            best = stats.AmountOfPoints;
        }

        deathScreenRecordPoints.text = best.ToString();
    }

    void SaveNewRecord(float points)
    {
        BinaryFormatter bf = new BinaryFormatter();
        //FileStream file = File.Create(Application.persistentDataPath
        //             + "/PlayerBest.dat");
        FileStream file = File.Create(Path.Combine(Application.persistentDataPath, "PlayerBest.dat"));
        SaveData data = new SaveData();
        data.maxPoints = points;
        bf.Serialize(file, data);
        file.Close();
    }

    SaveData LoadDataFromMemory()
    {
        SaveData loadedData = new SaveData();
        //if (File.Exists(Application.persistentDataPath
        //           + "/PlayerBest.dat"))
        if (File.Exists(Path.Combine(Application.persistentDataPath, "PlayerBest.dat")))
        {
            BinaryFormatter bf = new BinaryFormatter();
            //FileStream file =
            //           File.Open(Application.persistentDataPath
            //           + "/PlayerBest.dat", FileMode.Open);
            FileStream file =
                       File.Open(Path.Combine(Application.persistentDataPath, "PlayerBest.dat"), FileMode.Open);
            SaveData data = (SaveData)bf.Deserialize(file);
            file.Close();
            loadedData.maxPoints = data.maxPoints;
        }

        return loadedData;
    }

    public void ReloadScene()
    {
        //Scene scene = SceneManager.GetActiveScene();
        //SceneManager.LoadScene(scene.buildIndex);
        ResetAll();
    }

    public float amountToIncreaseSpeedAtEachCollectibleGrabbed = 0.5f;
    public void IncreaseSpeed(float increaseValue)
    {
        stats.GetComponent<MovePlayer>().speed += increaseValue;
        stats.GetComponent<DetectPlayerCollision>().speed += increaseValue;
    }

    [ContextMenu("Time scale 0")]
    void TimeScaleZero()
    {
        Time.timeScale = 0f;
    }

    [ContextMenu("Time scale 1")]
    void TimeScaleOne()
    {
        Time.timeScale = 1f;
    }

    [SerializeField] GameObject[] tutorialObjects;

    [SerializeField] float timeBetweenTutorialTips = 3f;
    [SerializeField] float initialTimeForTutorialTips = 3f;
    IEnumerator tutorialCoroutine;

    public void ShowTutorial()
    {
        if(tutorialCoroutine != null)
        {
            StopCoroutine(tutorialCoroutine);
        }

        for (int i = 0; i < tutorialObjects.Length; i++)
        {
            tutorialObjects[i].SetActive(false);
        }

        tutorialCoroutine = ShowTutorialCoroutine();
        StartCoroutine(tutorialCoroutine);
    }

    IEnumerator ShowTutorialCoroutine()
    {
        yield return new WaitForSeconds(initialTimeForTutorialTips);
        for (int i = 0; i < tutorialObjects.Length; i++)
        {
            //if(i != 0) tutorialObjects[i - 1].SetActive(false);
            tutorialObjects[i].SetActive(true);
            TimeScaleZero();
            
            yield return new WaitForSeconds(0.8f);
            tutorialObjects[i].SetActive(false);
            
            yield return new WaitForSeconds(timeBetweenTutorialTips);
        }

        //tutorialObjects[tutorialObjects.Length - 1].SetActive(false);
    }
}

public enum TutorialState
{
    state0,
    state1,
    state2,
    state3,
    state4
}



[System.Serializable]
public class SaveData
{
    public float maxPoints;

    public SaveData()
    {
        maxPoints = 0f;
    }
}