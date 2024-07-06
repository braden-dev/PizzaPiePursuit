using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerCollisionSound : MonoBehaviour
{
    public AudioClip barrelCollisionSound;
    private AudioSource audioSource;

    private void Start()
    {
        audioSource = gameObject.GetComponent<AudioSource>();
    }

    private void OnCollisionEnter(Collision collision)
    {
        //Debug.Log("Collided with another object");
        if (collision.collider.CompareTag("Barrel"))
        {
            //Debug.Log("Collided with a BARREL");
            float volume = Mathf.Clamp01(collision.relativeVelocity.magnitude / 10.0f);
            audioSource.PlayOneShot(barrelCollisionSound, volume);
        }
    }
}
