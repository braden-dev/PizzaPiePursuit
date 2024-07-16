using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EMPStunController : MonoBehaviour
{
    public float stunDuration = 3.0f;

    // This function is called when the collider enters the trigger
    private void OnTriggerEnter(Collider other)
    {
        Debug.Log("Collided with: " + other.gameObject.name);
        DroneMovement drone = other.GetComponentInChildren<DroneMovement>();
        if (drone != null)
        {
            Debug.Log("Stunning drone!");
            drone.Stun(stunDuration);
        }
    }

    private void OnTriggerStay(Collider other)
    {
        Debug.Log("Object staying inside sphere: " + other.gameObject.name);
        DroneMovement drone = other.GetComponentInChildren<DroneMovement>();
        if (drone != null)
        {
            Debug.Log("Stunning drone 2!");
            drone.Stun(stunDuration);
        }
    }
}
