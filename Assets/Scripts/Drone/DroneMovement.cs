using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DroneMovement : MonoBehaviour
{
    [Header("Waypoint Settings")]
    public GameObject[] waypoints; // Array of waypoints
    public float waypointThreshold = 1.0f; // Distance to waypoint to consider it reached
    //private int currentWaypointIndex = 0; // Index of the current waypoint

    [Header("Movement Settings")]
    public float hoverHeight = 5.0f; // Desired hover height above the ground
    public float obstacleAvoidanceHeight = 2.0f; // Height to ascend when encountering obstacles
    public float moveSpeed = 3.0f; // Speed of the drone's forward movement 
    public float ascendSpeed = 2.0f; // Speed of ascending 
    public float descendSpeed = 2.0f; // Speed of descending 
    public LayerMask groundLayer; // LayerMask to identify ground
    public float rotationSpeed = 2.0f; // Speed of rotation towards the target 
    private List<Vector3> smoothedWaypoints; // List of smoothed waypoints
    private int currentSmoothedIndex = 0; // Index of the current smoothed waypoint
    private bool avoidingObstacle = false; // Flag to indicate if the drone is avoiding an obstacle

    [Header("Scan Settings")]
    private float scanAngle;
    private float initialRotationY; // Store the initial Y rotation of the drone at the start of the scan
    private float scanDuration = 5.0f; // Duration to complete scan
    private bool isScanning = false; // Flag to indicate if the drone is scanning
    private float scanTimer = 0.0f; // Timer to track the scanning duration
    private float timeSinceLastScan = 0.0f; // Timer to track the time since the last scan
    private float nextScanTime; // Time interval to the next scan
    private bool isPausedBeforeScan = false;
    private bool isPausedAfterScan = false;
    private float pauseTimer = 0.0f;
    private float pauseDuration = 0.5f;

    [Header("Chase Settings")]
    public float chaseSpeed = 5.0f; // Speed when chasing the player
    public float chaseDistance = 10.0f; // Distance within which the drone will start chasing the player
    public GameObject player; // Reference to the player
    private Vector3 velocity = Vector3.zero;

    [Header("Separation Settings")]
    public float separationRadius = 2.0f; // Radius to keep drones separated
    public float separationStrength = 1.0f; // Strength of the separation force

    [Header("Search Settings")]
    private Vector3 lastKnownPlayerPosition;
    private float searchDuration = 10.0f; // Duration the drone will search for the player
    private float searchTimer = 0.0f;
    private Quaternion initialRotation;

    [Header("Stunned Settings")]
    private float stunEndTime = 0.0f;
    public GameObject stunParticles;
    public AudioSource stunSound;

    public enum DroneState
    {
        Patrol,
        Chase,
        Search,
        Stunned
    };

    public DroneState droneState;

    void Start()
    {
        droneState = DroneState.Patrol;
        SetNextScanTime();
        smoothedWaypoints = GenerateSmoothPath();
    }

    void Update()
    {
        switch (droneState)
        {
            case DroneState.Patrol:
                timeSinceLastScan += Time.deltaTime;

                if (waypoints.Length == 0)
                {
                    Debug.LogError("No waypoints set for the drone.");
                    return;
                }

                if (isScanning)
                {
                    PerformScan();
                }
                else
                {
                    //AvoidObstacles();
                    MoveAlongSmoothedPath();
                    TryStartScan();
                }

                if (avoidingObstacle)
                    moveSpeed = 1.5f;
                else
                    moveSpeed = 3.0f;

                CheckForPlayer();
                break;
            case DroneState.Chase:
                ChasePlayer();
                break;
            case DroneState.Search:
                //Debug.Log("Searching for player");
                SearchForPlayer();
                break;
            case DroneState.Stunned:
                if (Time.time >= stunEndTime)
                {
                    droneState = DroneState.Patrol;
                    stunParticles.SetActive(false);
                    stunSound.Stop();
                }
                break;
            default:
                break;
        }
        AvoidObstacles();
        MaintainHoverHeight();
    }

    private void CheckForPlayer()
    {
        Vector3 boxCenter = transform.position + transform.forward * chaseDistance / 2;
        Vector3 boxSize = new Vector3(chaseDistance, chaseDistance, chaseDistance);

        Collider[] hitColliders = Physics.OverlapBox(boxCenter, boxSize / 2, transform.rotation, LayerMask.GetMask("Default"));
        foreach (Collider hitCollider in hitColliders)
        {
            if (hitCollider.gameObject == player)
            {
                lastKnownPlayerPosition = player.transform.position;
                droneState = DroneState.Chase;
                return;
            }
        }

        if (droneState == DroneState.Chase)
        {
            droneState = DroneState.Search;
            searchTimer = 0.0f;
            initialRotation = transform.rotation; // Store the initial rotation
        }
    }

    private void SearchForPlayer()
    {
        searchTimer += Time.deltaTime;

        if (searchTimer > searchDuration)
        {
            droneState = DroneState.Patrol;
            return;
        }

        Vector3 directionToLastKnown = (lastKnownPlayerPosition - transform.position).normalized;
        Vector3 extendedPosition = lastKnownPlayerPosition + directionToLastKnown * 7.0f; // 7 meters beyond the last known position
        Vector3 targetPosition = extendedPosition; // Move directly to the extended position

        // Maintain the current height of the drone
        targetPosition.y = transform.position.y;

        // Move towards the extended position at chaseSpeed
        transform.position = Vector3.MoveTowards(transform.position, targetPosition, chaseSpeed * Time.deltaTime);

        // Maintain the initial rotation while moving
        transform.rotation = Quaternion.Slerp(transform.rotation, initialRotation, rotationSpeed * Time.deltaTime);

        // Check if the drone has reached the extended position
        Vector2 temp1 = new Vector2(transform.position.x, transform.position.z);
        Vector2 temp2 = new Vector2(extendedPosition.x, extendedPosition.z);
        // Using Vector2 because we don't care about the height difference
        if (Vector2.Distance(temp1, temp2) < 0.1f)
        {
            // Perform scan only after reaching the extended position
            if (!isScanning)
            {
                initialRotationY = transform.rotation.eulerAngles.y; // Store the initial rotation once
                isScanning = true; // Start scanning
            }
            PerformScan();
        }
        else
        {
            CheckForPlayer();
        }
    }

    private void ChasePlayer()
    {
        Vector3 direction = (player.transform.position - transform.position).normalized;
        float distanceToPlayer = Vector3.Distance(transform.position, player.transform.position);
        Quaternion toRotation = Quaternion.LookRotation(direction, Vector3.up);
        transform.rotation = Quaternion.Slerp(transform.rotation, toRotation, rotationSpeed * Time.deltaTime);

        Vector3 separationForce = CalculateSeparationForce();

        if (distanceToPlayer > 10.0f)
        {
            droneState = DroneState.Search;
            searchTimer = 0.0f;
            lastKnownPlayerPosition = player.transform.position;
        }
        else if (distanceToPlayer > 7.0f)
        {
            Vector3 targetPosition = player.transform.position - direction * 7.0f;

            // Ensure the drone maintains its hover height
            targetPosition.y = transform.position.y;

            // Apply separation force to avoid bouncing
            Vector3 finalPosition = Vector3.MoveTowards(transform.position, targetPosition, chaseSpeed * Time.deltaTime) + separationForce;

            // If the separation force is causing back and forth movement, apply a lateral force
            if (separationForce.magnitude > 0.1f)
            {
                Vector3 lateralDirection = Vector3.Cross(direction, Vector3.up).normalized;
                finalPosition += lateralDirection * separationStrength * Time.deltaTime;
            }

            transform.position = finalPosition;
        }

        // Maintain hover height during the chase
        //MaintainHoverHeight();
    }

    private Vector3 CalculateSeparationForce()
    {
        Vector3 separationForce = Vector3.zero;
        Collider[] nearbyDrones = Physics.OverlapSphere(transform.position, separationRadius);

        foreach (Collider droneCollider in nearbyDrones)
        {
            if (droneCollider.gameObject != gameObject && droneCollider.CompareTag("Drone"))
            {
                Vector3 directionAwayFromDrone = transform.position - droneCollider.transform.position;
                float distance = directionAwayFromDrone.magnitude;
                float forceMagnitude = Mathf.Clamp01(1.0f - distance / separationRadius) * separationStrength;
                separationForce += directionAwayFromDrone.normalized * forceMagnitude;
            }
        }

        return separationForce;
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.red;
        // Draw the scan box in front of the drone
        Vector3 boxCenter = transform.position + transform.forward * chaseDistance / 2;
        Vector3 boxSize = new Vector3(chaseDistance, chaseDistance, chaseDistance);
        Gizmos.DrawWireCube(boxCenter, boxSize);

        // Draw the separation radius
        Gizmos.color = Color.blue;
        Gizmos.DrawWireSphere(transform.position, separationRadius);
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

        transform.position += direction * moveSpeed * Time.deltaTime;

        // Check if the drone is close to the current smoothed waypoint
        if (Vector3.Distance(transform.position, targetPosition) <= waypointThreshold)
        {
            currentSmoothedIndex++;
        }
    }

    private void PerformScan()
    {
        if (isPausedBeforeScan)
        {
            pauseTimer += Time.deltaTime;
            if (pauseTimer >= pauseDuration)
            {
                isPausedBeforeScan = false;
                pauseTimer = 0.0f;
            }
            return;
        }

        scanTimer += Time.deltaTime;

        if (scanTimer >= scanDuration)
        {
            isScanning = false;
            isPausedAfterScan = true; // Start the pause after scanning
            scanTimer = 0.0f;
            return;
        }

        // Calculate the scan progress as a percentage of the scan duration
        float scanProgress = scanTimer / scanDuration;

        // Only determine the scan angle once at the start of the scan
        if (scanTimer == Time.deltaTime)
        {
            scanAngle = Random.Range(30.0f, 90.0f);
        }

        // Rotate to the left, back to center, then to the right, and back to center
        if (scanProgress < 0.25f) // First quarter of the scan duration
        {
            float rotationAngle = Mathf.Lerp(0, -scanAngle, scanProgress / 0.25f);
            transform.rotation = Quaternion.Euler(0, initialRotationY + rotationAngle, 0);
        }
        else if (scanProgress < 0.5f) // Second quarter of the scan duration
        {
            float rotationAngle = Mathf.Lerp(-scanAngle, 0, (scanProgress - 0.25f) / 0.25f);
            transform.rotation = Quaternion.Euler(0, initialRotationY + rotationAngle, 0);
        }
        else if (scanProgress < 0.75f) // Third quarter of the scan duration
        {
            float rotationAngle = Mathf.Lerp(0, scanAngle, (scanProgress - 0.5f) / 0.25f);
            transform.rotation = Quaternion.Euler(0, initialRotationY + rotationAngle, 0);
        }
        else // Last quarter of the scan duration
        {
            float rotationAngle = Mathf.Lerp(scanAngle, 0, (scanProgress - 0.75f) / 0.25f);
            transform.rotation = Quaternion.Euler(0, initialRotationY + rotationAngle, 0);
        }

        CheckForPlayer();
    }

    private void TryStartScan()
    {
        if (isPausedAfterScan)
        {
            pauseTimer += Time.deltaTime;
            if (pauseTimer >= pauseDuration)
            {
                isPausedAfterScan = false;
                pauseTimer = 0.0f;
                SetNextScanTime();
            }
            return;
        }

        if (timeSinceLastScan >= nextScanTime)
        {
            isScanning = true;
            isPausedBeforeScan = true; // Start the pause before scanning
            timeSinceLastScan = 0.0f;
        }
    }

    private void SetNextScanTime()
    {
        nextScanTime = Random.Range(15.0f, 65.0f);
    }

    private void MaintainHoverHeight()
    {
        RaycastHit hit;
        float checkDistance = 10.0f; // Distance to check if the drone is floating between buildings

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
                    transform.position += Vector3.up * ascendSpeed * Time.deltaTime;
                    // Tilt up
                    transform.rotation = Quaternion.Slerp(transform.rotation, Quaternion.Euler(-15, transform.rotation.eulerAngles.y, transform.rotation.eulerAngles.z), Time.deltaTime);
                }
                else if (heightDifference < 0)
                {
                    // Descend to hover height
                    transform.position += Vector3.down * descendSpeed * Time.deltaTime;
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
                transform.position += Vector3.up * ascendSpeed * Time.deltaTime;
                // Tilt up
                transform.rotation = Quaternion.Slerp(transform.rotation, Quaternion.Euler(-15, transform.rotation.eulerAngles.y, transform.rotation.eulerAngles.z), Time.deltaTime);
            }
            else if (!obstacleHits[transform.right])
            {
                // Move left
                transform.position += -transform.right * moveSpeed * Time.deltaTime;
                // Tilt left
                transform.rotation = Quaternion.Slerp(transform.rotation, Quaternion.Euler(0, transform.rotation.eulerAngles.y - 15, transform.rotation.eulerAngles.z), Time.deltaTime);
            }
            else if (!obstacleHits[-transform.right])
            {
                // Move right
                transform.position += transform.right * moveSpeed * Time.deltaTime;
                // Tilt right
                transform.rotation = Quaternion.Slerp(transform.rotation, Quaternion.Euler(0, transform.rotation.eulerAngles.y + 15, transform.rotation.eulerAngles.z), Time.deltaTime);
            }
            else if (!obstacleHits[Vector3.down])
            {
                // Descend
                transform.position += Vector3.down * descendSpeed * Time.deltaTime;
                // Tilt down
                transform.rotation = Quaternion.Slerp(transform.rotation, Quaternion.Euler(15, transform.rotation.eulerAngles.y, transform.rotation.eulerAngles.z), Time.deltaTime);
            }
            else if (!obstacleHits[Quaternion.AngleAxis(45, transform.right) * transform.forward])
            {
                // Move backward to avoid obstacles in front
                transform.position += -transform.forward * moveSpeed * Time.deltaTime;
            }
        }
        else
        {
            avoidingObstacle = false;
        }
    }

    public void Stun(float stunDuration)
    {
        droneState = DroneState.Stunned;
        stunEndTime = Time.time + stunDuration;
        stunParticles.SetActive(true);
        stunSound.Play();
        Debug.Log("Drone is stunned for " + stunDuration + " seconds.");
    }
}
