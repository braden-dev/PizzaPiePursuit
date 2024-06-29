using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using TMPro;

public class GameControls : MonoBehaviour
{
    // Game Variables.
    [Header("Game Variables")]
    public string sceneName = "Level_1";
    public float timer = 500f;
    public float timerRate = 0.01f;
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
    public string tutorialSceneName = "Level_1";
    CanvasGroup startMenuCanvasGroup;
    bool sceneIsStartScreen = false;

    // Level Select Menu Variables.
    [Header("Level Select Menu Variables")]
    public GameObject levelSelectMenu;
    CanvasGroup levelSelectMenuCanvasGroup;
    public string level1SceneName = "MainWorld";
    public string level2SceneName = "Tutorial";

    // Win Menu Variables.
    [Header("Win Menu Variables")]
    public GameObject winMenu;
    CanvasGroup winMenuCanvasGroup;
    bool winMenuShowing = false;


    // Start is called before the first frame update
    void Start()
    {
        
    }

    void Awake()
    {
        // Make sure gameplay is paused.
        PauseGameplayShort();

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
            ShowCanvas(hudCanvasGroup);
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
    }

    // Update is called once per frame
    void Update()
    {
        if (sceneIsStartScreen == false)
        {
            if (Input.GetKeyDown(pauseMenuKey) && gameOverMenuShowing == false && winMenuShowing == false)
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
                    timer = timer - timerRate*Time.deltaTime;
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

    public void StartLevel1()
    {
        SceneManager.LoadScene(level1SceneName);
    }

    public void StartLevel2()
    {
        SceneManager.LoadScene(level2SceneName);
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
    }

}
