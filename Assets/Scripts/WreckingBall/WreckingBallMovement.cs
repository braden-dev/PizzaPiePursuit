using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WreckingBallMovement : MonoBehaviour
{
    // Class initially copied from DroneMovement.cs and modified for wrecking ball behavior.

    //[Header("Waypoint Settings")]
    public GameObject[] waypoints; // Array of waypoints
    float waypointThreshold = 1.0f; // Distance to waypoint to consider it reached
    //private int currentWaypointIndex = 0; // Index of the current waypoint

    private float speed = 5.0f;
    private float chaseSpeed = 10.0f;
    private float hoverHeight = 1.5f;

    //[Header("Movement Settings")]
    public LayerMask groundLayer; // LayerMask to identify ground
    float rotationSpeed = 2.0f; // Speed of rotation towards the target 
    private List<Vector3> smoothedWaypoints; // List of smoothed waypoints
    private int currentSmoothedIndex = 0; // Index of the current smoothed waypoint
    private bool avoidingObstacle = false; // Flag to indicate if the wreckingBall is avoiding an obstacle

    //[Header("Chase Settings")]
    float chaseTriggerDistance = 10.0f; // Distance within which the wreckingBall will start chasing the player
    float chaseDistance = 0.0f;
    public GameObject player; // Reference to the player
    private Vector3 velocity = Vector3.zero;
    private float chaseTimer = 0.0f;

    // Return settings.
    private float returnDuration = 5.0f;
    private float returnTimer = 0.0f;
    private float returnProbability = 0.25f;
    private float returnLastCheckTime = 0.0f;
    private float returnTimeBetweenChecks = 1.0f;

    public enum WreckingBallState
    {
        Patrol,
        Return,
        Chase
    };

    WreckingBallState wreckingBallState;

    void Start()
    {
        returnTimer = 0.0f;
        chaseTimer = 0.0f;
        returnLastCheckTime = 0.0f;
        wreckingBallState = WreckingBallState.Patrol;
        smoothedWaypoints = GenerateSmoothPath();
        if (waypoints.Length == 0)
        {
            Debug.LogError("No waypoints set for the wreckingBall.");
            return;
        }
    }

    void Update()
    {
        switch (wreckingBallState)
        {
            case WreckingBallState.Patrol:
                Patrol();
                break;
            case WreckingBallState.Chase:
                ChasePlayer();
                break;
            case WreckingBallState.Return:
                ReturnToPatrol();
                break;
            default:
                break;
        }
        AvoidObstacles();
        MaintainHoverHeight();
    }

    private void OnDrawGizmos()
    {
        // Draw the separation radius
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, chaseTriggerDistance);
    }

    private void Patrol()
    {
        // Move to waypoint.
        MoveAlongSmoothedPath();
        // Check if player is close enough.
        CheckForPlayer();
    }

    private void ReturnToPatrol()
    {
        // Increment timer tracking return time.
        returnTimer += Time.deltaTime;
        if (returnTimer > returnDuration)
        {
            wreckingBallState = WreckingBallState.Patrol;
            return;
        }
        // Move to waypoitn.
        MoveAlongSmoothedPath();
    }

    private void CheckForPlayer()
    {
        // Check if player is close enough.
        if ((transform.position - player.transform.position).magnitude < chaseTriggerDistance)
        {
            wreckingBallState = WreckingBallState.Chase;
            chaseTimer = 0.0f;
            returnLastCheckTime = 0.0f;
        }
    }

    private void ChasePlayer()
    {
        // Move towards the player.
        Vector3 direction = (transform.position - player.transform.position).normalized;
        float distanceToPlayer = Vector3.Distance(transform.position, player.transform.position);
        Quaternion toRotation = Quaternion.LookRotation(direction, Vector3.up);
        transform.rotation = Quaternion.Slerp(transform.rotation, toRotation, rotationSpeed * Time.deltaTime);

        chaseTimer += Time.deltaTime;

        // This sets up checking to randomly break from chasing player at set intervals not based on framerate.
        if (returnTimeBetweenChecks + returnLastCheckTime <= chaseTimer)
        {
            returnLastCheckTime = chaseTimer;
        }

        if (distanceToPlayer > chaseTriggerDistance || (returnLastCheckTime == chaseTimer && Random.Range(0f, 1.0f) < returnProbability))
        {
            wreckingBallState = WreckingBallState.Return;
            returnTimer = 0.0f;
            returnLastCheckTime = 0.0f;
        }
        else if (distanceToPlayer > chaseDistance)
        {
            Vector3 targetPosition = player.transform.position - direction * chaseDistance;

            // Ensure the wreckingBall maintains its hover height
            targetPosition.y = transform.position.y;

            // Apply separation force to avoid bouncing
            Vector3 finalPosition = Vector3.MoveTowards(transform.position, targetPosition, chaseSpeed * Time.deltaTime);

            transform.position = finalPosition;
        }
    }

    private List<Vector3> GenerateSmoothPath()
    {
        List<Vector3> pathPoints = new List<Vector3>();
        for (int i = 0; i < waypoints.Length; i++)
        {
            Vector3 p0 = waypoints[(i - 1 + waypoints.Length) % waypoints.Length].transform.position;
            Vector3 p1 = waypoints[i].transform.position;
            Vector3 p2 = waypoints[(i + 1) % waypoints.Length].transform.position;
            Vector3 p3 = waypoints[(i + 2) % waypoints.Length].transform.position;

            for (float t = 0; t < 1; t += 0.1f)
            {
                Vector3 point = CalculateCatmullRom(p0, p1, p2, p3, t);
                pathPoints.Add(point);
            }
        }
        return pathPoints;
    }

    private Vector3 CalculateCatmullRom(Vector3 p0, Vector3 p1, Vector3 p2, Vector3 p3, float t)
    {
        float t2 = t * t;
        float t3 = t2 * t;

        Vector3 result = 0.5f * (
            2.0f * p1 +
            (-p0 + p2) * t +
            (2.0f * p0 - 5.0f * p1 + 4.0f * p2 - p3) * t2 +
            (-p0 + 3.0f * p1 - 3.0f * p2 + p3) * t3
        );

        return result;
    }

    private void MoveAlongSmoothedPath()
    {
        if (currentSmoothedIndex >= smoothedWaypoints.Count)
        {
            currentSmoothedIndex = 0;
        }

        Vector3 targetPosition = new Vector3(smoothedWaypoints[currentSmoothedIndex].x, transform.position.y, smoothedWaypoints[currentSmoothedIndex].z);
        Vector3 direction = (targetPosition - transform.position).normalized;

        // Rotate towards the target direction
        if (direction != Vector3.zero)
        {
            Quaternion toRotation = Quaternion.LookRotation(direction, Vector3.up);
            transform.rotation = Quaternion.Slerp(transform.rotation, toRotation, rotationSpeed * Time.deltaTime);
        }

        transform.position += direction * speed * Time.deltaTime;

        // Check if the wreckingBall is close to the current smoothed waypoint
        if (Vector3.Distance(transform.position, targetPosition) <= waypointThreshold)
        {
            currentSmoothedIndex++;
        }
    }

    private void MaintainHoverHeight()
    {
        RaycastHit hit;
        float checkDistance = 10.0f; // Distance to check if the wreckingBall is floating between buildings

        // Perform the raycast
        bool isGroundDetected = Physics.Raycast(transform.position, Vector3.down, out hit, checkDistance, groundLayer);

        // Draw the raycast for visualization
        Debug.DrawRay(transform.position, Vector3.down * checkDistance, isGroundDetected ? Color.green : Color.red);

        if (isGroundDetected)
        {
            float currentHeight = hit.distance;
            float heightDifference = hoverHeight - currentHeight;

            if (!avoidingObstacle)
            {
                if (heightDifference > 0)
                {
                    // Ascend to hover height
                    transform.position += Vector3.up * speed * Time.deltaTime;
                    // Tilt up
                    transform.rotation = Quaternion.Slerp(transform.rotation, Quaternion.Euler(-15, transform.rotation.eulerAngles.y, transform.rotation.eulerAngles.z), Time.deltaTime);
                }
                else if (heightDifference < 0)
                {
                    // Descend to hover height
                    transform.position += Vector3.down * speed * Time.deltaTime;
                    // Tilt down
                    transform.rotation = Quaternion.Slerp(transform.rotation, Quaternion.Euler(15, transform.rotation.eulerAngles.y, transform.rotation.eulerAngles.z), Time.deltaTime);
                }
                else
                {
                    // Level out
                    transform.rotation = Quaternion.Slerp(transform.rotation, Quaternion.Euler(0, transform.rotation.eulerAngles.y, transform.rotation.eulerAngles.z), Time.deltaTime);
                }
            }
        }
        else
        {
            // If no ground is detected within the check distance, maintain current height
            transform.position = new Vector3(transform.position.x, transform.position.y, transform.position.z);
        }
    }

    private void AvoidObstacles()
    {
        float rayDistance = 3.0f; // Uniform raycast distance
        float sideRayDistance = 1.5f;

        // Raycasts in all directions
        Vector3[] directions = new Vector3[]
        {
        transform.forward,
        -transform.forward,
        transform.right,
        -transform.right,
        Quaternion.AngleAxis(45, transform.right) * transform.forward,
        Quaternion.AngleAxis(-45, transform.right) * transform.forward,
        Quaternion.AngleAxis(45, transform.right) * -transform.forward,
        Quaternion.AngleAxis(-45, transform.right) * -transform.forward,
        Vector3.up,
        Vector3.down
        };

        // Track whether an obstacle is detected in each direction
        Dictionary<Vector3, bool> obstacleHits = new Dictionary<Vector3, bool>();

        foreach (Vector3 direction in directions)
        {
            RaycastHit hit;
            if (direction == transform.right || direction == -transform.right)
            {
                bool obstacleHit = Physics.Raycast(transform.position, direction, out hit, sideRayDistance);
                obstacleHits[direction] = obstacleHit;
                Debug.DrawRay(transform.position, direction * sideRayDistance, obstacleHit ? Color.red : Color.green);
            }
            else
            {
                bool obstacleHit = Physics.Raycast(transform.position, direction, out hit, rayDistance);
                obstacleHits[direction] = obstacleHit;
                Debug.DrawRay(transform.position, direction * rayDistance, obstacleHit ? Color.red : Color.green);
            }
        }

        // Determine the best way to avoid obstacles based on raycast results
        if (obstacleHits[transform.forward])
        {
            avoidingObstacle = true;

            // Prioritize moving up, left, right, or down if front is blocked
            if (!obstacleHits[Vector3.up])
            {
                // Ascend
                transform.position += Vector3.up * speed * Time.deltaTime;
                // Tilt up
                transform.rotation = Quaternion.Slerp(transform.rotation, Quaternion.Euler(-15, transform.rotation.eulerAngles.y, transform.rotation.eulerAngles.z), Time.deltaTime);
            }
            else if (!obstacleHits[transform.right])
            {
                // Move left
                transform.position += -transform.right * speed * Time.deltaTime;
                // Tilt left
                transform.rotation = Quaternion.Slerp(transform.rotation, Quaternion.Euler(0, transform.rotation.eulerAngles.y - 15, transform.rotation.eulerAngles.z), Time.deltaTime);
            }
            else if (!obstacleHits[-transform.right])
            {
                // Move right
                transform.position += transform.right * speed * Time.deltaTime;
                // Tilt right
                transform.rotation = Quaternion.Slerp(transform.rotation, Quaternion.Euler(0, transform.rotation.eulerAngles.y + 15, transform.rotation.eulerAngles.z), Time.deltaTime);
            }
            else if (!obstacleHits[Vector3.down])
            {
                // Descend
                transform.position += Vector3.down * speed * Time.deltaTime;
                // Tilt down
                transform.rotation = Quaternion.Slerp(transform.rotation, Quaternion.Euler(15, transform.rotation.eulerAngles.y, transform.rotation.eulerAngles.z), Time.deltaTime);
            }
            else if (!obstacleHits[Quaternion.AngleAxis(45, transform.right) * transform.forward])
            {
                // Move backward to avoid obstacles in front
                transform.position += -transform.forward * speed * Time.deltaTime;
            }
        }
        else
        {
            avoidingObstacle = false;
        }
    }
}
