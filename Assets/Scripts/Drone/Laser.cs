using UnityEngine;

public class Laser : MonoBehaviour
{
    public float lifeTime = 5.0f; // Lifetime of the laser before it gets destroyed
    public PlayerHealthManager playerHealthManager; // Reference to the object with the PlayerHealthManager script

    void Start()
    {
        Destroy(gameObject, lifeTime); // Destroy the laser after a set time

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

    void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.CompareTag("Drone"))
        {
            // Do nothing if the laser hits the drone
            return;
        }
        if (collision.gameObject.CompareTag("Player"))
        {
            //Debug.Log("HIT PLAYER");
            playerHealthManager.TakeDamage();
        }
        Destroy(gameObject); // Destroy the laser on impact with anything other than the drone
    }
}
