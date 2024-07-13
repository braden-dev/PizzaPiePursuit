using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class ButtonHelperFunctions : MonoBehaviour
{
    GameControls gameControls;
    public AudioSource buttonClick;

    // Start is called before the first frame update
    void Start()
    {
        gameControls = GameObject.Find("GameController").GetComponent<GameControls>();
        if(buttonClick == null)
        {
            buttonClick = GameObject.Find("GameController").GetComponent<AudioSource>();
        }
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private IEnumerator PlayClickSoundAndWait(Action action)
    {
        if (buttonClick != null)
        {
            buttonClick.Play();
        }
        yield return new WaitForSecondsRealtime(0.2f);
        action.Invoke();
    }

    public void ResumeGameplay()
    {
        StartCoroutine(PlayClickSoundAndWait(gameControls.ResumeGameplay));
    }

    public void StartGame()
    {
        StartCoroutine(PlayClickSoundAndWait(gameControls.StartGame));
    }

    public void QuitGame()
    {
        StartCoroutine(PlayClickSoundAndWait(gameControls.QuitGame));
    }

    public void RestartCurrentLevel()
    {
        StartCoroutine(PlayClickSoundAndWait(gameControls.RestartCurrentLevel));
    }

    public void StartTutorial()
    {
        StartCoroutine(PlayClickSoundAndWait(gameControls.StartTutorial));
    }

    public void StartLevel1()
    {
        StartCoroutine(PlayClickSoundAndWait(gameControls.StartLevel1));
    }

    public void StartLevel2()
    {
        StartCoroutine(PlayClickSoundAndWait(gameControls.StartLevel2));
    }

    public void StartLevel3()
    {
        StartCoroutine(PlayClickSoundAndWait(gameControls.StartLevel3));
    }

    public void LevelSelectMenu()
    {
        StartCoroutine(PlayClickSoundAndWait(gameControls.LevelSelectMenu));
    }

    public void StartMenu()
    {
        StartCoroutine(PlayClickSoundAndWait(gameControls.StartMenu));
    }

    public void LoadMainWorld()
    {
        StartCoroutine(PlayClickSoundAndWait(gameControls.LoadMainWorld));
    }
}
