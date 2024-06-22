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

    void Start()
    {
        playerTransform = GameObject.FindGameObjectWithTag("Player").transform;
        droneMovement = GetComponent<DroneMovement>();
        previousPlayerPosition = playerTransform.position;
    }

    void Update()
    {
        // Calculate player velocity
        playerVelocity = (playerTransform.position - previousPlayerPosition) / Time.deltaTime;
        previousPlayerPosition = playerTransform.position;

        if (droneMovement.droneState == DroneMovement.DroneState.Chase && CanSeePlayer(sightRange))
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
    }

    Vector3 PredictFuturePosition()
    {
        Vector3 playerChestPosition = playerTransform.position + chestOffset;
        Vector3 toPlayer = playerChestPosition - firePoint.position;
        float distance = toPlayer.magnitude;
        float timeToReach = distance / laserSpeed;

        // If the player is not moving, predict future position as the current position
        if (playerVelocity.magnitude < 0.1f)
        {
            return playerChestPosition;
        }

        return playerChestPosition + playerVelocity * timeToReach;
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
}
