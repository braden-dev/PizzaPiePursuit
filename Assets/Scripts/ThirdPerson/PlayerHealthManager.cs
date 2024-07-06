using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerHealthManager : MonoBehaviour
{
    private int playerHealth = 5;
    private int playerMaxHealth = 5;
    public GameControls gameControlsScript;

    void Update()
    {
        if(playerHealth <= 0)
        {
            gameControlsScript.EndGame();
        }
    }

    public void TakeDamage()
    {
        //Debug.Log("Taking damage");
        playerHealth -= 1;
    }

    public int GetHealth()
    {
        return playerHealth;
    }

    public int GetMaxHealth()
    {
        return playerMaxHealth;
    }

    public void Heal(int healAmount)
    {
        playerHealth += healAmount;
    }
}
