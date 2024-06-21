using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class PauseMenuButtonControls : MonoBehaviour
{
    Menu menu;

    // Start is called before the first frame update
    void Start()
    {
        menu = GameObject.FindWithTag("PauseMenu").GetComponent<Menu>();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void Resume()
    {
        menu.Hide();
    }

    public void StartGame()
    {
        SceneManager.LoadScene("Level_1");
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
}
