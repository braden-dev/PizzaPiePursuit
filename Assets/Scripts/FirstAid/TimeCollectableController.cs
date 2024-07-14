using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TimeCollectableController : MonoBehaviour
{
    private PlayerHealthManager playerHealthManager;

    void Start()
    {
        // Find the player and get the PlayerHealthManager component
        GameObject player = GameObject.FindWithTag("Player");
        if (player != null)
        {
            playerHealthManager = player.GetComponent<PlayerHealthManager>();
            if (playerHealthManager == null)
            {
                Debug.LogError("PlayerHealthManager component not found on Player.");
            }
        }
        else
        {
            Debug.LogError("Player object not found.");
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            // Add time to the timer
            AddTimeToTimer(10f);

            // Play the health collectable sound effect
            PlayCollectableSound();

            // Destroy the collectable object immediately
            Destroy(gameObject);
        }
    }

    private void AddTimeToTimer(float timeToAdd)
    {
        // Ensure the new timer value does not exceed maxTimer
        GameControls gameControlsScript = playerHealthManager.gameControlsScript;
        if (gameControlsScript != null)
        {
            gameControlsScript.timer = Mathf.Min(gameControlsScript.timer + timeToAdd, gameControlsScript.maxTimer);
        }
        else
        {
            Debug.LogError("GameControls script is not assigned in PlayerHealthManager.");
        }
    }

    private void PlayCollectableSound()
    {
        if (playerHealthManager != null && playerHealthManager.healthCollectableSound != null)
        {
            playerHealthManager.healthCollectableSound.Play();
        }
        else
        {
            Debug.LogError("healthCollectableSound is not assigned in PlayerHealthManager.");
        }
    }
}
