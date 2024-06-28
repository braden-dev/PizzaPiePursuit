using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class HealthBarController : MonoBehaviour
{
    public PlayerHealthManager playerHealthManager; 
    public Image healthBar; 
    private int maxHealth;

    void Start()
    {
        if (healthBar == null)
        {
            Debug.LogError("Health bar image is not set.");
        }
        if (playerHealthManager == null)
        {
            Debug.LogError("PlayerHealthManager is not set.");
        }

        maxHealth = playerHealthManager.GetMaxHealth();
    }

    void Update()
    {
        // Update the health bar fill amount based on the player's health
        if (playerHealthManager != null && healthBar != null)
        {
            healthBar.fillAmount = (float)playerHealthManager.GetHealth() / maxHealth;
        }
    }
}
