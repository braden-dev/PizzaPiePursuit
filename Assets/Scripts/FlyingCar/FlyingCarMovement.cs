using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FlyingCarMovement : MonoBehaviour
{

    [Header("Waypoint Settings")]
    public GameObject[] waypoints; // Array of waypoints
    private GameObject[] randomWaypoints;
    public float waypointThreshold = 1.0f; // Distance to waypoint to consider it reached

    [Header("Movement Settings")]
    public float moveSpeed = 3.0f; // Speed of the drone's forward movement 
    public float rotationSpeed = 2.0f; // Speed of rotation towards the target 
    private List<Vector3> smoothedWaypoints; // List of smoothed waypoints
    private int currentSmoothedIndex = 0; // Index of the current smoothed waypoint

    public GameObject flyingCarLandingSpot;
    public float goToLandingSpotChance = 0.05f;
    private bool isLanding = false;

    // Start is called before the first frame update
    void Start()
    {
        RandomizeWaypoints();
        smoothedWaypoints = GenerateSmoothPath();
    }

    // Update is called once per frame
    void Update()
    {
        if (waypoints.Length == 0)
        {
            Debug.LogError("No waypoints set for the drone.");
            return;
        }

        if(!isLanding)
            MoveAlongSmoothedPath();
    }

    private List<Vector3> GenerateSmoothPath()
    {
        List<Vector3> pathPoints = new List<Vector3>();
        for (int i = 0; i < randomWaypoints.Length; i++)
        {
            Vector3 p0 = randomWaypoints[(i - 1 + randomWaypoints.Length) % randomWaypoints.Length].transform.position;
            Vector3 p1 = randomWaypoints[i].transform.position;
            Vector3 p2 = randomWaypoints[(i + 1) % randomWaypoints.Length].transform.position;
            Vector3 p3 = randomWaypoints[(i + 2) % randomWaypoints.Length].transform.position;

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
            RandomizeWaypoints();
            smoothedWaypoints = GenerateSmoothPath();
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
            float randomLandValue = Random.value;
            StartCoroutine(CheckAndLand(randomLandValue));
        }
    }

    private void RandomizeWaypoints()
    {
        randomWaypoints = new GameObject[waypoints.Length];

        List<GameObject> waypointsList = new List<GameObject>(waypoints);

        for (int i = 0; i < waypointsList.Count; i++)
        {
            GameObject temp = waypointsList[i];
            int randomIndex = Random.Range(i, waypointsList.Count);
            waypointsList[i] = waypointsList[randomIndex];
            waypointsList[randomIndex] = temp;
        }

        randomWaypoints = waypointsList.ToArray();
    }

    private IEnumerator CheckAndLand(float randomLandValue)
    {
        //Debug.Log("Random Land Value: " + randomLandValue);
        if (randomLandValue <= goToLandingSpotChance)
        {
            isLanding = true;

            Vector3 landingPosition = flyingCarLandingSpot.transform.position;

            while (Vector3.Distance(new Vector3(transform.position.x, 0, transform.position.z), new Vector3(landingPosition.x, 0, landingPosition.z)) > waypointThreshold)
            {
                Vector3 direction = (new Vector3(landingPosition.x, transform.position.y, landingPosition.z) - transform.position).normalized;
                Quaternion toRotation = Quaternion.LookRotation(direction, Vector3.up);
                transform.rotation = Quaternion.Slerp(transform.rotation, toRotation, rotationSpeed * Time.deltaTime);
                transform.position += direction * moveSpeed * Time.deltaTime;
                yield return null;
            }

            Quaternion targetRotation = Quaternion.Euler(0, -90, 0);
            while (Quaternion.Angle(transform.rotation, targetRotation) > 0.1f)
            {
                transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, rotationSpeed * Time.deltaTime);
                yield return null;
            }

            while (transform.position.y > landingPosition.y)
            {
                transform.position -= new Vector3(0, moveSpeed / 2 * Time.deltaTime, 0);
                yield return null;
            }

            float stayDuration = Random.Range(5f, 30f);
            yield return new WaitForSeconds(stayDuration);

            while (transform.position.y < landingPosition.y + 10f) 
            {
                transform.position += new Vector3(0, moveSpeed / 2 * Time.deltaTime, 0);
                yield return null;
            }

            isLanding = false;
        }
    }
}
