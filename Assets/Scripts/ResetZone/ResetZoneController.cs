using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ResetZoneController : MonoBehaviour
{
    public GameControls gameControlsScript;

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            if (gameControlsScript != null)
                gameControlsScript.EndGame();
            else
                Debug.LogWarning("GameControls scipt reference is missing.");
        }
    }
}
