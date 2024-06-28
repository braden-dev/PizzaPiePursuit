using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerHealthManager : MonoBehaviour
{
    private int playerHealth;
    public GameControls gameControlsScript;

    // Start is called before the first frame update
    void Start()
    {
        playerHealth = 5;
    }

    // Update is called once per frame
    void Update()
    {
        if(playerHealth <= 0)
        {
            gameControlsScript.EndGame();
        }
    }

    public void TakeDamage()
    {
        Debug.Log("Taking damage");
        playerHealth -= 1;
    }

    public int GetHealth()
    {
        return playerHealth;
    }

    public void Heal(int healAmount)
    {
        playerHealth += healAmount;
    }
}
