using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PlayerEMPManager : MonoBehaviour
{
    private int empCharges = 0;
    public AudioSource collectableSound;
    public GameObject empChargeContainer;
    public GameObject empChargeCirclePrefab;
    public GameObject aoeSpherePrefab;
    public AudioSource empBlastSound;

    // Start is called before the first frame update
    void Start()
    {
        if (collectableSound == null)
        {
            collectableSound = GetComponent<AudioSource>();
        }
        if (empBlastSound == null)
        {
            empBlastSound = GetComponent<AudioSource>();
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
            empBlastSound.Play();
            TriggerAOEEffect();
        }
    }

    private void TriggerAOEEffect()
    {
        if (aoeSpherePrefab != null)
        {
            GameObject aoeSphere = Instantiate(aoeSpherePrefab, transform.position, Quaternion.identity);
            aoeSphere.SetActive(true);
            StartCoroutine(ScaleOverTime(aoeSphere, 0.1f, 15f, 0.3f));
            StartCoroutine(DeactivateAfterDelay(aoeSphere, 1f)); 
        }
    }

    private IEnumerator ScaleOverTime(GameObject aoeSphere, float startScale, float endScale, float duration)
    {
        float currentTime = 0.0f;
        Vector3 startScaleVector = new Vector3(startScale, startScale, startScale);
        Vector3 endScaleVector = new Vector3(endScale, endScale, endScale);

        while (currentTime <= duration)
        {
            aoeSphere.transform.localScale = Vector3.Lerp(startScaleVector, endScaleVector, currentTime / duration);
            currentTime += Time.deltaTime;
            yield return null;
        }
        aoeSphere.transform.localScale = endScaleVector;
    }

    private IEnumerator DeactivateAfterDelay(GameObject aoeSphere, float delay)
    {
        yield return new WaitForSeconds(delay);
        aoeSphere.SetActive(false);
        Destroy(aoeSphere);
    }
}