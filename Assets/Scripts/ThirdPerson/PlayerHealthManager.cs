using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerHealthManager : MonoBehaviour
{
    private int playerHealth = 5;
    private int playerMaxHealth = 5;
    public GameControls gameControlsScript;
    public AudioSource damageSound;

    private bool isInvulnerable = false;
    private float invulnerabilityDuration = 0.2f;

    public AudioSource healthCollectableSound;

    private void Start()
    {
        if(damageSound == null)
        {
            damageSound = GetComponent<AudioSource>();
        }

        if (healthCollectableSound == null)
        {
            healthCollectableSound = GetComponent<AudioSource>();
        }
    }

    void Update()
    {
        if(playerHealth <= 0)
        {
            gameControlsScript.EndGame();
        }
    }

    public void TakeDamage()
    {
        if (!isInvulnerable)
        {
            StartCoroutine(HandleDamage());
        }
    }

    private IEnumerator HandleDamage()
    {
        //Debug.Log("Player health before damage: " + playerHealth);
        playerHealth -= 1;
        if (damageSound != null)
        {
            damageSound.Play();
        }
        //Debug.Log("Player health after damage: " + playerHealth);

        isInvulnerable = true;
        yield return new WaitForSeconds(invulnerabilityDuration);
        isInvulnerable = false;
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

        if (healthCollectableSound != null)
        {
            healthCollectableSound.Play();
        }
    }

    public void SetInvulnerability(bool state)
    {
        isInvulnerable = state;
    }

    public bool IsInvulnerable()
    {
        return isInvulnerable;
    }
}
