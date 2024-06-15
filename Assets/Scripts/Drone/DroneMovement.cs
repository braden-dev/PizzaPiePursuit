using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DroneMovement : MonoBehaviour
{
    public float hoverHeight = 5.0f; // Desired hover height above the ground
    public float obstacleAvoidanceHeight = 2.0f; // Height to ascend when encountering obstacles
    public float moveSpeed = 5.0f; // Speed of the drone's forward movement
    public float ascendSpeed = 2.0f; // Speed of ascending
    public float descendSpeed = 2.0f; // Speed of descending
    public float turnInterval = 10.0f; // Distance interval for turning
    public float turnAngle = 90.0f; // Angle to turn
    public LayerMask groundLayer; // LayerMask to identify ground

    private float distanceTravelled = 0.0f; // Track the distance traveled

    // Update is called once per frame
    void Update()
    {
        RaycastHit hit;

        // Raycast downward to maintain hover height
        if (Physics.Raycast(transform.position, Vector3.down, out hit, Mathf.Infinity, groundLayer))
        {
            float distanceToGround = hit.distance;
            Debug.Log("Distance to ground: " + distanceToGround);

            if (distanceToGround > hoverHeight)
            {
                // Descend to reach hover height
                transform.position -= new Vector3(0, descendSpeed * Time.deltaTime, 0);
            }
            else if (distanceToGround < hoverHeight)
            {
                // Ascend to reach hover height
                transform.position += new Vector3(0, ascendSpeed * Time.deltaTime, 0);
            }
        }
        else
        {
            Debug.Log("No ground detected below.");
        }

        // Raycast forward to detect obstacles
        if (Physics.Raycast(transform.position, transform.forward, out hit, obstacleAvoidanceHeight))
        {
            Debug.Log("Obstacle detected ahead at distance: " + hit.distance);
            // Ascend to avoid obstacle
            transform.position += new Vector3(0, ascendSpeed * Time.deltaTime, 0);
        }
        else
        {
            Debug.Log("No obstacle detected ahead.");
        }

        // Move forward
        float distanceThisFrame = moveSpeed * Time.deltaTime;
        transform.position += transform.forward * distanceThisFrame;
        distanceTravelled += distanceThisFrame;

        // Check if it's time to turn
        if (distanceTravelled >= turnInterval)
        {
            transform.Rotate(0, turnAngle, 0);
            distanceTravelled = 0.0f;
        }
    }
}
