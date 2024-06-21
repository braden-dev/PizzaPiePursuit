using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using TMPro;

public class GameControls : MonoBehaviour
{
    // Game Variables.
    [Header("Game Variables")]
    public string startScene = "Level_1";
    public float timer = 500f;
    public float timerRate = 0.01f;
    bool gamePaused = true;
    
    // Pause Menu Variables.
    [Header("Pause Menu Variables")]
    public GameObject pauseMenu;
    public string pauseMenuKey = "1";
    CanvasGroup pauseMenuCanvasGroup;

    // Game Over Menu Variables.
    [Header("Game Over Menu Variables")]
    public GameObject gameOverMenu;
    CanvasGroup gameOverMenuCanvasGroup;

    // HUD Variables.
    [Header("HUD Variables")]
    public GameObject hud;
    CanvasGroup hudCanvasGroup;
    public TextMeshProUGUI hudTimerText;

    

    // Start is called before the first frame update
    void Start()
    {
        
    }

    void Awake()
    {
        // Make sure gameplay is paused.
        PauseGameplay();

        // Initialize Pause Menu
        pauseMenuCanvasGroup = pauseMenu.GetComponent<CanvasGroup>();
        // code referenced here: https://forum.unity.com/threads/getcomponent-doesnt-works.516839/
        if (pauseMenuCanvasGroup == null)
        {
            Debug.Log("Error, pauseMenuCanvasGroup not found");
        }
        HideCanvas(pauseMenuCanvasGroup);

        // Initialize Pause Menu
        gameOverMenuCanvasGroup = gameOverMenu.GetComponent<CanvasGroup>();
        // code referenced here: https://forum.unity.com/threads/getcomponent-doesnt-works.516839/
        if (gameOverMenuCanvasGroup == null)
        {
            Debug.Log("Error, gameOverMenuCanvasGroup not found");
        }
        HideCanvas(gameOverMenuCanvasGroup);

        // Initialize HUD.
        hudCanvasGroup = hud.GetComponent<CanvasGroup>();
        // code referenced here: https://forum.unity.com/threads/getcomponent-doesnt-works.516839/
        if (hudCanvasGroup == null)
        {
            Debug.Log("Error, hudCanvasGroup not found");
        }
        ShowCanvas(hudCanvasGroup);


        // Resume Gameplay.
        ResumeGameplay();
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(pauseMenuKey))
        {
            if (pauseMenuCanvasGroup.interactable)
            {
                HideCanvas(pauseMenuCanvasGroup);
                ResumeGameplay();
            }
            else
            {
                ShowCanvas(pauseMenuCanvasGroup);
                PauseGameplay();
            }
        }

        if (gamePaused == false)
        {
            if (timer > 0)
            {
                timer = timer - timerRate;
                hudTimerText.text = Mathf.RoundToInt(timer).ToString();
            }
            else
            {
                ShowCanvas(gameOverMenuCanvasGroup);
                PauseGameplay();
            }
        }
    }

    public void PauseGameplay()
    {
        Time.timeScale = 0f;
        Cursor.visible = true;
        Cursor.lockState = CursorLockMode.Confined;
        gamePaused = true;
    }

    public void ResumeGameplay()
    {
        Cursor.visible = false;
        Cursor.lockState = CursorLockMode.Locked;
        gamePaused = false;
        Time.timeScale = 1f;
    }

    public void StartGame()
    {
        SceneManager.LoadScene(startScene);
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

}
