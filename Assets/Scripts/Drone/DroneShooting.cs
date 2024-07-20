using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DroneShooting : MonoBehaviour
{
    public GameObject laserPrefab; // Laser prefab to be instantiated
    public Transform firePoint; // Point from where the laser will be fired
    public float fireRate = 2.0f; // Rate of fire
    public float laserSpeed = 30.0f; // Speed of the laser
    public float sightRange = 20.0f; // Range within which the drone can see the player
    public LayerMask obstructionMask; // LayerMask for obstructions
    public Vector3 chestOffset = new Vector3(0, 1.5f, 0); // Offset to aim at the player's chest

    private float nextFireTime = 0.0f;
    private Transform playerTransform;
    private DroneMovement droneMovement; // Reference to the DroneMovement script
    private Vector3 previousPlayerPosition;
    private Vector3 playerVelocity;

    private float predictionInaccuracy = 1f;

    public AudioSource laserBulletSound;

    void Start()
    {
        playerTransform = GameObject.FindGameObjectWithTag("Player").transform;
        droneMovement = GetComponent<DroneMovement>();
        previousPlayerPosition = playerTransform.position;

        if(laserBulletSound == null)
        {
            laserBulletSound = GetComponent<AudioSource>();
        }
    }

    void Update()
    {
        // Calculate player velocity
        playerVelocity = (playerTransform.position - previousPlayerPosition) / Time.deltaTime;
        previousPlayerPosition = playerTransform.position;

        PlayerHealthManager playerHealth = playerTransform.GetComponent<PlayerHealthManager>();

        if (droneMovement.droneState == DroneMovement.DroneState.Chase && CanSeePlayer(sightRange) && playerHealth != null && !playerHealth.IsInvulnerable())
        {
            if (Time.time > nextFireTime)
            {
                Fire();
                nextFireTime = Time.time + 1.0f / fireRate;
            }
        }
    }

    void Fire()
    {
        // Calculate the future position of the player
        Vector3 futurePosition = PredictFuturePosition();

        // Calculate aim direction towards the future position
        Vector3 aimDirection = (futurePosition - firePoint.position).normalized;

        // Create and launch the laser with additional 90 degrees rotation on x-axis
        Quaternion rotation = Quaternion.LookRotation(aimDirection) * Quaternion.Euler(90, 0, 0);
        GameObject laser = Instantiate(laserPrefab, firePoint.position, rotation);
        Rigidbody rb = laser.GetComponent<Rigidbody>();
        if (rb != null)
        {
            rb.velocity = aimDirection * laserSpeed;
        }

        if (laserBulletSound != null)
        {
            laserBulletSound.Play();
        }
    }

    Vector3 PredictFuturePosition()
    {
        Vector3 playerChestPosition = playerTransform.position + chestOffset;
        Vector3 toPlayer = playerChestPosition - firePoint.position;
        float distance = toPlayer.magnitude;
        float timeToReach = distance / laserSpeed;

        Vector3 randomOffset = new Vector3(
            Random.Range(-predictionInaccuracy, predictionInaccuracy),
            Random.Range(-predictionInaccuracy, predictionInaccuracy),
            Random.Range(-predictionInaccuracy, predictionInaccuracy)
        );

        // If the player is not moving, predict future position as the current position
        if (playerVelocity.magnitude < 0.1f)
        {
            return playerChestPosition + randomOffset;
        }

        Vector3 predictedPosition = playerChestPosition + playerVelocity * timeToReach;

        return predictedPosition + randomOffset;
    }

    public bool CanSeePlayer(float sightRange)
    {
        Vector3 targetPosition = playerTransform.position + chestOffset;
        Vector3 directionToPlayer = (targetPosition - firePoint.position).normalized;
        float distanceToPlayer = Vector3.Distance(firePoint.position, targetPosition);

        // Draw the raycast line for visualization
        Debug.DrawLine(firePoint.position, firePoint.position + directionToPlayer * distanceToPlayer, Color.red);

        if (distanceToPlayer < sightRange)
        {
            RaycastHit hit;
            if (Physics.Raycast(firePoint.position, directionToPlayer, out hit, distanceToPlayer))
            {
                if (hit.collider.transform == playerTransform)
                {
                    return true;
                }
            }
        }
        return false;
    }

    public void ResetFireTime()
    {
        // Introduce a longer lag by adding a fixed delay or a larger random range
        float delay = 1.0f; 
        nextFireTime = Time.time + delay + Random.Range(0.0f, 1.0f); // Adding randomness if needed
    }

}
