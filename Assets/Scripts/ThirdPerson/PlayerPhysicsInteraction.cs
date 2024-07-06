using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerPhysicsInteraction : MonoBehaviour
{
    private CharacterController characterController;
    private Rigidbody playerRigidbody;

    void Start()
    {
        characterController = GetComponent<CharacterController>();
        playerRigidbody = GetComponent<Rigidbody>();

        if (playerRigidbody != null)
        {
            playerRigidbody.isKinematic = true;
        }
    }

    void OnControllerColliderHit(ControllerColliderHit hit)
    {
        Rigidbody hitRigidbody = hit.collider.attachedRigidbody;

        // Ensure the collided object has a Rigidbody and is not kinematic
        if (hitRigidbody != null && !hitRigidbody.isKinematic)
        {
            Vector3 forceDirection = hit.point - transform.position;
            forceDirection.y = 0;
            forceDirection.Normalize();

            hitRigidbody.AddForce(forceDirection * characterController.velocity.magnitude, ForceMode.Impulse);
        }
    }
}
