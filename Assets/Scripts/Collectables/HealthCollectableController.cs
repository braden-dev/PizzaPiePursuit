using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HealthCollectableController : MonoBehaviour
{
    public PlayerHealthManager playerHealthManager;

    void Start()
    {
        GameObject player = GameObject.FindWithTag("Player");
        if (player != null)
        {
            // Get the PlayerHealthManager component from the player object
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
        if(other.CompareTag("Player"))
        {
            int healAmount = playerHealthManager.GetMaxHealth() - playerHealthManager.GetHealth();
            playerHealthManager.Heal(healAmount);

            Destroy(gameObject);
        }
    }
}
