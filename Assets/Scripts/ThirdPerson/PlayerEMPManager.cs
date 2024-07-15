using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PlayerEMPManager : MonoBehaviour
{
    private int empCharges = 0;
    public AudioSource collectableSound;
    public GameObject empChargeContainer;
    public GameObject empChargeCirclePrefab; // Prefab of the circle image

    // Start is called before the first frame update
    void Start()
    {
        if (collectableSound == null)
        {
            collectableSound = GetComponent<AudioSource>();
        }
        UpdateEMPChargeUI();
    }

    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.E))
        {
            UseEMPCharge();
        }
    }

    public void AddEMPCharge()
    {
        collectableSound.Play();
        empCharges += 1;
        UpdateEMPChargeUI();
    }

    public int GetEMPCharges()
    {
        return empCharges;
    }

    private void UpdateEMPChargeUI()
    {
        foreach (Transform child in empChargeContainer.transform)
        {
            Destroy(child.gameObject);
        }

        for (int i = 0; i < empCharges; i++)
        {
            Instantiate(empChargeCirclePrefab, empChargeContainer.transform);
        }
    }

    private void UseEMPCharge()
    {
        if (empCharges > 0)
        {
            empCharges -= 1;
            UpdateEMPChargeUI();
        }
    }
}