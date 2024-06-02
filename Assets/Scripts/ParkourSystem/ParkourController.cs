using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ParkourController : MonoBehaviour
{
    bool inAction = false;

    [SerializeField] float stepUpTime = 0.2f;

    EnvironmentScanner environmentScanner;
    Animator animator;
    PlayerController playerController;

    private void Awake()
    {
        environmentScanner = GetComponent<EnvironmentScanner>();
        animator = GetComponent<Animator>();
        playerController = GetComponent<PlayerController>();
    }

    private void Update()
    {
        var hitData = environmentScanner.ObstacleCheck();

        if (Input.GetButton("Jump") && !inAction)
        {
            //var hitData = environmentScanner.ObstacleCheck();
            if (hitData.forwardHitFound)
            {
                //Debug.Log("Obstacle Found " + hitData.forwardHit.transform.name);
                StartCoroutine(DoParkourAction());
                
            }
        }
    }

    IEnumerator DoParkourAction()
    {
        inAction = true;
        playerController.SetControl(false);
        animator.CrossFade("StepUp", stepUpTime);

        yield return null;

        var animState = animator.GetNextAnimatorStateInfo(0);

        yield return new WaitForSeconds(animState.length);

        playerController.SetControl(true);
        inAction = false;
    }
}
