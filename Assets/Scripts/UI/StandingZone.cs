using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StandingZone : MonoBehaviour
{
    private GameControls gameControls;

    void Start()
    {
        gameControls = FindObjectOfType<GameControls>();
        if (gameControls == null)
        {
            Debug.LogError("GameControls not found in the scene.");
        }
    }

    void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            gameControls.ShowStartLevelMenu();
        }
    }
}
