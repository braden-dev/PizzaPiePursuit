using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DeliveryLocation : MonoBehaviour
{
    private GameControls gameControls;

    public GameObject emissiveLightObjMission1;
    public GameObject emissiveLightObjMission2;
    public GameObject emissiveLightObjMission3;
    public GameObject emissiveLightObjMission4;
    public GameObject pizzaShopLight;
    public MissionController missionController;

    private int missionId;

    public AudioSource winSound;

    void Start()
    {
        gameControls = FindObjectOfType<GameControls>();

        pizzaShopLight.SetActive(true);
        emissiveLightObjMission1.SetActive(false);
        emissiveLightObjMission2.SetActive(false);
        emissiveLightObjMission3.SetActive(false);
        emissiveLightObjMission4.SetActive(false);

        if (gameControls == null)
        {
            Debug.LogError("GameControls not found in the scene.");
        }
        if (missionController == null)
        {
            Debug.LogError("Mission Controller script not set for delivery location.");
        }
        if(winSound == null)
        {
            winSound = GetComponent<AudioSource>();
        }
        if(emissiveLightObjMission1 == null)
        {
            Debug.LogError("Emmisive Light A not found in the scene.");
        }
        if (emissiveLightObjMission2 == null)
        {
            Debug.LogError("Emmisive Light B not found in the scene.");
        }
        if (emissiveLightObjMission3 == null)
        {
            Debug.LogError("Emmisive Light C not found in the scene.");
        }
        if (emissiveLightObjMission4 == null)
        {
            Debug.LogError("Emmisive Light D not found in the scene.");
        }
        if (pizzaShopLight == null)
        {
            Debug.LogError("Pizza Shop Light not found in the scene.");
        }
    }

    private void Update()
    {
        //Debug.Log(missionController.GetIsInMission());
        if (missionController.GetIsInMission())
        { 
            missionId = missionController.GetMissionId();
            switch (missionId)
            {
                case 1:
                    emissiveLightObjMission1.SetActive(true);
                    pizzaShopLight.SetActive(false);
                    break;
                case 2:
                    emissiveLightObjMission2.SetActive(true);
                    pizzaShopLight.SetActive(false);
                    break;
                case 3:
                    emissiveLightObjMission3.SetActive(true);
                    pizzaShopLight.SetActive(false);
                    break;
                case 4:
                    emissiveLightObjMission4.SetActive(true);
                    pizzaShopLight.SetActive(false);
                    break;
                default:
                    break;
            }
        }
        else
        {
            emissiveLightObjMission1.SetActive(false);
            emissiveLightObjMission2.SetActive(false);
            emissiveLightObjMission3.SetActive(false);
            emissiveLightObjMission4.SetActive(false);
            pizzaShopLight.SetActive(true);
        }
    }

    void OnTriggerEnter(Collider other)
    {
        if (missionController.GetIsInMission())
        {
            if (gameObject.name == "DeliveryHouseA" && missionController.GetMissionId() == 1)
            {
                if (other.CompareTag("Player"))
                {
                    if (winSound != null)
                    {
                        winSound.Play();
                    }
                    //Debug.Log("Time Left: " + gameControls.timer);
                    missionController.SetMissionComplete(1, gameControls.maxTimer - gameControls.timer);
                    gameControls.ShowWinMenu();
                }
            }
            else if(gameObject.name == "DeliveryHouseB" && missionController.GetMissionId() == 2)
            {
                if (other.CompareTag("Player"))
                {
                    if (winSound != null)
                    {
                        winSound.Play();
                    }
                    missionController.SetMissionComplete(2, gameControls.maxTimer - gameControls.timer);
                    gameControls.ShowWinMenu();
                }
            }
            else if(gameObject.name == "DeliveryHouseC" && missionController.GetMissionId() == 3)
            {
                if (other.CompareTag("Player"))
                {
                    if (winSound != null)
                    {
                        winSound.Play();
                    }
                    missionController.SetMissionComplete(3, gameControls.maxTimer - gameControls.timer);
                    gameControls.ShowWinMenu();
                }
            }
            else if (gameObject.name == "DeliveryFlyingCarA" && missionController.GetMissionId() == 4)
            {
                if (other.CompareTag("Player"))
                {
                    if (winSound != null)
                    {
                        winSound.Play();
                    }
                    missionController.SetMissionComplete(4, gameControls.maxTimer - gameControls.timer);
                    gameControls.ShowWinMenu();
                }
            }
        }
    }
}
