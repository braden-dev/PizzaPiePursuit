using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DeliveryLocation : MonoBehaviour
{
    private GameControls gameControls;

    public GameObject emissiveLightObj;
    public MissionController missionController;

    public AudioSource winSound;

    void Start()
    {
        gameControls = FindObjectOfType<GameControls>();
        emissiveLightObj.SetActive(false);

        if (gameControls == null)
        {
            Debug.LogError("GameControls not found in the scene.");
        }
        if (emissiveLightObj == null)
        {
            Debug.LogError("EmissiveLight object not set for delivery location.");
        }
        if (missionController == null)
        {
            Debug.LogError("Mission Controller script not set for delivery location.");
        }
        if(winSound == null)
        {
            winSound = GetComponent<AudioSource>();
        }
    }

    private void Update()
    {
        //Debug.Log(missionController.GetIsInMission());
        if(missionController.GetIsInMission())
            emissiveLightObj.SetActive(true);
        else
            emissiveLightObj.SetActive(false);
    }

    void OnTriggerEnter(Collider other)
    {
        if (missionController.GetIsInMission())
        {
            if (other.CompareTag("Player"))
            {
                if(winSound != null)
                {
                    winSound.Play();
                }
                gameControls.ShowWinMenu();
            }
        }
    }
}
