using UnityEngine;

public class Laser : MonoBehaviour
{
    public float lifeTime = 5.0f; // Lifetime of the laser before it gets destroyed

    void Start()
    {
        Destroy(gameObject, lifeTime); // Destroy the laser after a set time
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
            // Handle damage to the player
            //Debug.Log("HIT PLAYER");
        }
        Destroy(gameObject); // Destroy the laser on impact with anything other than the drone
    }
}
