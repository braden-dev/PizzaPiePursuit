using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using TMPro;

public class GameControls : MonoBehaviour
{
    string targetName = "Player";
    string pizzaBagName = "PizzaBag";
    GameObject pizzaBag;

    // Game Variables.
    [Header("Game Variables")]
    public string sceneName = "Tutorial";
    public float maxTimer = 500f;
    public float timer = 500f;
    public float timerRate = 0.01f;
    private float currentRate = 0.0f;
    bool gamePaused = true;
    
    // Pause Menu Variables.
    [Header("Pause Menu Variables")]
    public GameObject pauseMenu;
    public string pauseMenuKey = "1";
    public string startMenuSceneName = "GameStart";
    CanvasGroup pauseMenuCanvasGroup;

    // Game Over Menu Variables.
    [Header("Game Over Menu Variables")]
    public GameObject gameOverMenu;
    CanvasGroup gameOverMenuCanvasGroup;
    bool gameOverMenuShowing = false;

    // HUD Variables.
    [Header("HUD Variables")]
    public GameObject hud;
    CanvasGroup hudCanvasGroup;
    public TextMeshProUGUI hudTimerText;

    // Start Menu Variables.
    [Header("Start Menu Variables")]
    public GameObject startMenu;
    public string tutorialSceneName = "Tutorial";
    CanvasGroup startMenuCanvasGroup;
    bool sceneIsStartScreen = false;

    // Level Select Menu Variables.
    [Header("Level Select Menu Variables")]
    public GameObject levelSelectMenu;
    CanvasGroup levelSelectMenuCanvasGroup;
    public string level1SceneName = "MainWorld";
    public string level2SceneName = "Tutorial";
    public string mainWorldSceneName = "MainWorld";

    // Win Menu Variables.
    [Header("Win Menu Variables")]
    public GameObject winMenu;
    CanvasGroup winMenuCanvasGroup;
    bool winMenuShowing = false;

    // Start Level Menu Variables.
    [Header("Start Level Menu Variables")]
    public GameObject startLevelMenu;
    CanvasGroup startLevelMenuCanvasGroup;
    bool startLevelMenuShowing = false;
    public TextMeshProUGUI delivery1ButtonText;
    public TextMeshProUGUI delivery2ButtonText;
    public TextMeshProUGUI delivery3ButtonText;
    public TextMeshProUGUI delivery4ButtonText;
    public TextMeshProUGUI delivery1BestTimeText;
    public TextMeshProUGUI delivery2BestTimeText;
    public TextMeshProUGUI delivery3BestTimeText;
    public TextMeshProUGUI delivery4BestTimeText;
    private float lastShowStartLevelMenuTime = -1f;
    private const float showStartLevelMenuCooldown = 1.0f;

    [Header("Miscellaneous Variables")]
    private MissionController missionController;


    // Start is called before the first frame update
    void Start()
    {
        GameObject player = GameObject.Find(targetName);
        pizzaBag = GameObject.Find(pizzaBagName);
        pizzaBag.SetActive(false);
        missionController = player.GetComponent<MissionController>();
        if(missionController == null)
        {
            Debug.LogError("MisisonController script not set in GameControls script.");
        }
    }

    void Awake()
    {
        // Make sure gameplay is paused.
        PauseGameplayShort();

        timer = maxTimer;

        // Initialize Pause Menu
        if (pauseMenu != null)
        {
            pauseMenuCanvasGroup = pauseMenu.GetComponent<CanvasGroup>();
            // code referenced here: https://forum.unity.com/threads/getcomponent-doesnt-works.516839/
            if (pauseMenuCanvasGroup == null)
            {
                Debug.Log("Error, pauseMenuCanvasGroup not found");
            }
            HideCanvas(pauseMenuCanvasGroup);
        }

        // Initialize Game Over Menu
        if (gameOverMenu != null)
        {
            gameOverMenuCanvasGroup = gameOverMenu.GetComponent<CanvasGroup>();
            if (gameOverMenuCanvasGroup == null)
            {
                Debug.Log("Error, gameOverMenuCanvasGroup not found");
            }
            HideCanvas(gameOverMenuCanvasGroup);
        }

        // Initialize HUD.
        if (hud != null)
        {
            hudCanvasGroup = hud.GetComponent<CanvasGroup>();
            if (hudCanvasGroup == null)
            {
                Debug.Log("Error, hudCanvasGroup not found");
            }
            HideCanvas(hudCanvasGroup);
            currentRate = 0.0f;
        }

        // Initialize Start Menu
        if (startMenu != null)
        {
            startMenuCanvasGroup = startMenu.GetComponent<CanvasGroup>();
            if (startMenuCanvasGroup == null)
            {
                Debug.Log("Error, startMenuCanvasGroup not found");
            }
            ShowCanvas(startMenuCanvasGroup);
            sceneIsStartScreen = true;
        }

        // Initialize Level Select Menu
        if (levelSelectMenu != null)
        {
            levelSelectMenuCanvasGroup = levelSelectMenu.GetComponent<CanvasGroup>();
            if (levelSelectMenuCanvasGroup == null)
            {
                Debug.Log("Error, levelSelectMenuCanvasGroup not found");
            }
            HideCanvas(levelSelectMenuCanvasGroup);
        }
            
        if (sceneIsStartScreen == false)
        {
            // Resume Gameplay.
            ResumeGameplayShort();
        }

        // Initialize Win Menu
        if (winMenu != null)
        {
            winMenuCanvasGroup = winMenu.GetComponent<CanvasGroup>();
            if (winMenuCanvasGroup == null)
            {
                Debug.Log("Error, winMenuCanvasGroup not found");
            }
            HideCanvas(winMenuCanvasGroup);
        }

        // Initialize Start Level Menu
        if (startLevelMenu != null)
        {
            startLevelMenuCanvasGroup = startLevelMenu.GetComponent<CanvasGroup>();
            if (startLevelMenuCanvasGroup == null)
            {
                Debug.Log("Error, startLevelMenuCanvasGroup not found");
            }
            HideCanvas(startLevelMenuCanvasGroup);
        }
    }

    // Update is called once per frame
    void Update()
    {
        if (sceneIsStartScreen == false)
        {
            if (Input.GetKeyDown(pauseMenuKey) && gameOverMenuShowing == false && winMenuShowing == false && startLevelMenuShowing == false)
            {
                if (pauseMenuCanvasGroup.interactable)
                {
                    ResumeGameplay();
                }
                else
                {
                    PauseGameplay();
                }
            }

            if (gamePaused == false)
            {
                if (timer > 0)
                {
                    timer = timer - currentRate*Time.deltaTime;
                    hudTimerText.text = Mathf.RoundToInt(timer).ToString();
                }
                else
                {
                    EndGame();
                }
            }
        }
    }

    public void EndGame()
    {
        ShowCanvas(gameOverMenuCanvasGroup);
        gameOverMenuShowing = true;
        PauseGameplayShort();
    }

    public void PauseGameplayShort()
    {
        Time.timeScale = 0f;
        Cursor.visible = true;
        Cursor.lockState = CursorLockMode.Confined;
        gamePaused = true;
    }

    public void PauseGameplay()
    {
        PauseGameplayShort();
        ShowCanvas(pauseMenuCanvasGroup);
    }

    public void ResumeGameplayShort()
    {
        Cursor.visible = false;
        Cursor.lockState = CursorLockMode.Locked;
        gamePaused = false;
        Time.timeScale = 1f;
    }

    public void ResumeGameplay()
    {
        HideCanvas(pauseMenuCanvasGroup);
        ResumeGameplayShort();
    }

    public void StartGame()
    {
        SceneManager.LoadScene(sceneName);
        //Time.timeScale = 1f;
    }

    public void QuitGame()
    {
        #if UNITY_EDITOR
                UnityEditor.EditorApplication.isPlaying = false;
        #else
                Application.Quit();
        #endif
    }

    public void HideCanvas(CanvasGroup canvasGroup)
    {
        canvasGroup.interactable = false;
        canvasGroup.blocksRaycasts = false;
        canvasGroup.alpha = 0f;
    }

    public void ShowCanvas(CanvasGroup canvasGroup)
    {
        canvasGroup.interactable = true;
        canvasGroup.blocksRaycasts = true;
        canvasGroup.alpha = 1f;
    }

    public void StartLevel(string levelName)
    {
        SceneManager.LoadScene(levelName);
    }

    public void StartTutorial()
    {
        SceneManager.LoadScene(tutorialSceneName);
    }

    public void LoadMainWorld()
    {
        SceneManager.LoadScene(mainWorldSceneName);
    }

    public void StartLevelGeneral()
    {
        HideStartLevelMenu();
        ShowCanvas(hudCanvasGroup);
        pizzaBag.SetActive(true);
        timer = maxTimer;
        currentRate = timerRate;
    }

    public void StartLevel1()
    {
        missionController.SetIsInMission(true, 1);
        StartLevelGeneral();
    }

    public void StartLevel2()
    {
        missionController.SetIsInMission(true, 2);
        StartLevelGeneral();
    }

    public void StartLevel3()
    {
        missionController.SetIsInMission(true, 3);
        StartLevelGeneral();
    }

    public void StartLevel4()
    {
        missionController.SetIsInMission(true, 4);
        StartLevelGeneral();
    }

    public void RestartCurrentLevel()
    {
        
    }

    public void HideMenus()
    {

    }

    public void LevelSelectMenu()
    {
        HideCanvas(startMenuCanvasGroup);
        ShowCanvas(levelSelectMenuCanvasGroup);
    }

    public void StartMenu()
    {
        if (sceneIsStartScreen)
        {
            HideCanvas(levelSelectMenuCanvasGroup);
            ShowCanvas(startMenuCanvasGroup);
        }
        else
        {
            SceneManager.LoadScene(startMenuSceneName);
        }
    }

    public void ShowWinMenu()
    {
        ShowCanvas(winMenuCanvasGroup);
        winMenuShowing = true;
        PauseGameplayShort();
        missionController.SetIsInMission(false, -1);
    }

    public void ShowStartLevelMenu()
    {
        if (Time.time - lastShowStartLevelMenuTime < showStartLevelMenuCooldown)
        {
            return;
        }

        lastShowStartLevelMenuTime = Time.time;

        if (missionController.GetIsInMission() == false)
        {
            if (missionController.GetMission1Complete())
                delivery1ButtonText.text = "Delivery #1 (Complete)";
            else
                delivery1ButtonText.text = "Delivery #1";
            if (missionController.GetMission2Complete())
                delivery2ButtonText.text = "Delivery #2 (Complete)";
            else
                delivery2ButtonText.text = "Delivery #2";
            if (missionController.GetMission3Complete())
                delivery3ButtonText.text = "Delivery #3 (Complete)";
            else
                delivery3ButtonText.text = "Delivery #3";
            if (missionController.GetMission4Complete())
                delivery4ButtonText.text = "Delivery #4 (Complete)";
            else
                delivery4ButtonText.text = "Delivery #4";

            if (missionController.GetMission1BestTime() == 1000.0f)
                delivery1BestTimeText.text = "N/A";
            else
            {
                float delivery1BestTime = missionController.GetMission1BestTime();
                string formattedString = string.Format("{0:F3}s", delivery1BestTime);
                delivery1BestTimeText.text = formattedString;
            }
            if (missionController.GetMission2BestTime() == 1000.0f)
                delivery2BestTimeText.text = "N/A";
            else
            {
                float delivery2BestTime = missionController.GetMission2BestTime();
                string formattedString = string.Format("{0:F3}s", delivery2BestTime);
                delivery2BestTimeText.text = formattedString;
            }
            if (missionController.GetMission3BestTime() == 1000.0f)
                delivery3BestTimeText.text = "N/A";
            else
            {
                float delivery3BestTime = missionController.GetMission3BestTime();
                string formattedString = string.Format("{0:F3}s", delivery3BestTime);
                delivery3BestTimeText.text = formattedString;
            }
            if (missionController.GetMission4BestTime() == 1000.0f)
                delivery4BestTimeText.text = "N/A";
            else
            {
                float delivery4BestTime = missionController.GetMission4BestTime();
                string formattedString = string.Format("{0:F3}s", delivery4BestTime);
                delivery4BestTimeText.text = formattedString;
            }

            ShowCanvas(startLevelMenuCanvasGroup);
            startLevelMenuShowing = true;
            PauseGameplayShort();
        }
    }

    public void HideStartLevelMenu()
    {
        HideCanvas(startLevelMenuCanvasGroup);
        startLevelMenuShowing = false;
        ResumeGameplayShort();
    }
}
