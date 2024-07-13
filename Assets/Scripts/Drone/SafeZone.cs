using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class SafeZone : MonoBehaviour
{
    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            PlayerHealthManager playerHealth = other.GetComponent<PlayerHealthManager>();
            if (playerHealth != null)
            {
                playerHealth.SetInvulnerability(true);
                NotifyDronesToResetFireTime();
            }
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            PlayerHealthManager playerHealth = other.GetComponent<PlayerHealthManager>();
            if (playerHealth != null)
            {
                playerHealth.SetInvulnerability(false);
                NotifyDronesToResetFireTime();
            }
        }
    }

    private void NotifyDronesToResetFireTime()
    {
        DroneShooting[] drones = FindObjectsOfType<DroneShooting>();
        foreach (DroneShooting drone in drones)
        {
            drone.ResetFireTime();
        }
    }
}


