using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Menu : MonoBehaviour
{
    [SerializeField] string keyName = "1";
    public GameObject gameControlsObject;
    CanvasGroup canvasGroup;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    void Awake()
    {
        canvasGroup = GetComponent<CanvasGroup>();
        // code referenced here: https://forum.unity.com/threads/getcomponent-doesnt-works.516839/
        if (canvasGroup == null)
        {
            Debug.Log("Error, CanvasGroup not found");
        }
        else
        {
            Hide();
        }
    }

    // Update is called once per frame
    void Update()
    {
        if (ToggleTrigger())
        {
            if (IsShown())
            {
                Hide();
            }
            else
            {
                Show();
            }
        }
    }

    public void Hide()
    {
        canvasGroup.interactable = false;
        canvasGroup.blocksRaycasts = false;
        canvasGroup.alpha = 0f;
        Time.timeScale = 1f;

        Cursor.visible = false;
        Cursor.lockState = CursorLockMode.Locked;
    }

    public void Show()
    {
        canvasGroup.interactable = true;
        canvasGroup.blocksRaycasts = true;
        canvasGroup.alpha = 1f;
        Time.timeScale = 0f;

        Cursor.visible = true;
        Cursor.lockState = CursorLockMode.Confined;
    }

    public bool IsShown()
    {
        return canvasGroup.interactable;
    }

    public bool ToggleTrigger()
    {
        return Input.GetKeyDown(keyName);
    }
}


