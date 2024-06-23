using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ButtonHelperFunctions : MonoBehaviour
{
    public GameObject referenceGameObject;
    CanvasGroup canvasGroup;
    GameControls gameControls;

    // Start is called before the first frame update
    void Start()
    {
        gameControls = GameObject.Find("GameController").GetComponent<GameControls>();
        if (referenceGameObject != null)
        {
            canvasGroup = referenceGameObject.GetComponent<CanvasGroup>();
        }
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void ResumeGameplay()
    {
        gameControls.HideCanvas(canvasGroup);
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
}
