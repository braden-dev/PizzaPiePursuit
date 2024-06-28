using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ButtonHelperFunctions : MonoBehaviour
{
    GameControls gameControls;

    // Start is called before the first frame update
    void Start()
    {
        gameControls = GameObject.Find("GameController").GetComponent<GameControls>();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void ResumeGameplay()
    {
        gameControls.ResumeGameplay();
    }

    public void StartGame()
    {
        gameControls.StartGame();
    }

    public void QuitGame()
    {
        gameControls.QuitGame();
    }

    public void RestartCurrentLevel()
    {
        gameControls.RestartCurrentLevel();
    }

    public void StartTutorial()
    {
        gameControls.StartTutorial();
    }

    public void StartLevel1()
    {
        gameControls.StartLevel1();
    }

    public void StartLevel2()
    {
        gameControls.StartLevel2();
    }

    public void LevelSelectMenu()
    {
        gameControls.LevelSelectMenu();
    }

    public void StartMenu()
    {
        gameControls.StartMenu();
    }
}
