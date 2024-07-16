using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EMPStunController : MonoBehaviour
{
    public float stunDuration;

    private void Start()
    {
        stunDuration = 1.5f;
    }

    // This function is called when the collider enters the trigger
    private void OnTriggerEnter(Collider other)
    {
        //Debug.Log("Collided with: " + other.gameObject.name);
        DroneMovement drone = other.GetComponentInChildren<DroneMovement>();
        if (drone != null)
        {
            //Debug.Log("Stunning drone!");
            drone.Stun(stunDuration);
        }
    }
}
