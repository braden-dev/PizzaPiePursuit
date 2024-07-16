using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EMPCollectableController : MonoBehaviour
{
    public PlayerEMPManager playerEMPManager;

    void Start()
    {
        GameObject player = GameObject.FindWithTag("Player");
        if (player != null)
        {
            // Get the PlayerHealthManager component from the player object
            playerEMPManager = player.GetComponent<PlayerEMPManager>();
            if (playerEMPManager == null)
            {
                Debug.LogError("PlayerHealthManager component not found on Player.");
            }
        }
        else
        {
            Debug.LogError("Player object not found.");
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if(other.CompareTag("Player"))
        {
            if (playerEMPManager.GetEMPCharges() < 3)
            {
                playerEMPManager.AddEMPCharge();
                //Debug.Log("Emp Charges: " + playerEMPManager.GetEMPCharges());

                Destroy(gameObject);
            }
        }
    }
}
