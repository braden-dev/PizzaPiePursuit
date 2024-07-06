using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BarrelCollisionSound : MonoBehaviour
{
    public AudioClip collisionSound;
    private AudioSource audioSource;

    void Start()
    {
        audioSource = gameObject.AddComponent<AudioSource>();
    }

    void OnCollisionEnter(Collision collision)
    {
        if (audioSource != null && collisionSound != null)
        {
            float volume = Mathf.Clamp01(collision.relativeVelocity.magnitude / 10.0f);
            audioSource.PlayOneShot(collisionSound, volume);
        }
    }
}
