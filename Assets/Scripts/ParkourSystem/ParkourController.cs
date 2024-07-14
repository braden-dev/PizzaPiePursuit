using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ParkourController : MonoBehaviour
{
    [SerializeField] List<ParkourAction> parkourActions;
    [SerializeField] ParkourAction jumpDownAction;
    //[SerializeField] float autoDropHeightLimit = 1f;

    bool inAction = false;

    [SerializeField] float stepUpTime = 0.2f;

    EnvironmentScanner environmentScanner;
    Animator animator;
    PlayerController playerController;
    Rigidbody rb;

    public bool requireInputForActions = false;

    //int inAirCounter = 0;

    private void Awake()
    {
        environmentScanner = GetComponent<EnvironmentScanner>();
        animator = GetComponent<Animator>();
        playerController = GetComponent<PlayerController>();
        rb = GetComponent<Rigidbody>();
    }

    private void Update()
    {
        var hitData = environmentScanner.ObstacleCheck();

        if (requireInputForActions)
        {
            if (Input.GetKeyDown(KeyCode.Space))
                PerformParkourActions(hitData);
        }
        else
            PerformParkourActions(hitData);
        
        //if (playerController.IsOnLedge && !inAction && !hitData.forwardHitFound)
        //{
            //if ((playerController.LedgeData.height <= autoDropHeightLimit || Input.GetKeyDown(KeyCode.Q)) && playerController.LedgeData.angle <= 50)
            //{
            //    playerController.IsOnLedge = false;
            //    StartCoroutine(DoParkourAction(jumpDownAction, 0.5f));
            //}

            //bool shouldJump = true;
            //if (playerController.LedgeData.height > autoDropHeightLimit && !Input.GetKeyDown(KeyCode.Q))
            //    shouldJump = false;

            //if (shouldJump && playerController.LedgeData.angle <= 50)
            //{
            //    playerController.IsOnLedge = false;
            //    StartCoroutine(DoParkourAction(jumpDownAction, 0.5f));
            //}
        //}

        if (!playerController.IsOnLedge && environmentScanner.ClearPathSlideCheck() && Input.GetKeyDown(KeyCode.LeftShift) && animator.GetCurrentAnimatorStateInfo(0).IsName("Locomotion") && playerController.IsGrounded)
        {
            StartCoroutine(DoParkourAction(parkourActions[4], 3.0f));
        }

        if (environmentScanner.ClearPathJumpCheck() && !hitData.forwardHitFound && playerController.IsGrounded && Input.GetKeyDown(KeyCode.Space) && animator.GetCurrentAnimatorStateInfo(0).IsName("Locomotion"))
        {
            StartCoroutine(DoParkourAction(parkourActions[5], 3.0f));
        }

        //if (!playerController.IsGrounded && animator.GetCurrentAnimatorStateInfo(0).IsName("Locomotion"))
        //{
            //playerController.SetControl(false);
            //inAirCounter += 1;
            //if (inAirCounter > 100)
            //{
            //    StartCoroutine(DoParkourAction(jumpDownAction, 0.5f));
            //}
        //}
        //else
        //{
            //inAirCounter = 0;
            //playerController.SetControl(true);
        //}
        //Debug.Log(playerController.IsGrounded);
        //Debug.Log(inAction);

    }

    void PerformParkourActions(EnvironmentScanner.ObstacleHitData hitData)
    {
        if (!inAction && animator.GetCurrentAnimatorStateInfo(0).IsName("Locomotion"))
        {
            // // Then check for ledge jump if on a ledge
            // else if (playerController.IsOnLedge && ledgeJumpAction.CheckIfPossible(hitData, transform))
            // {
            //     StartCoroutine(DoParkourAction(ledgeJumpAction));
            // }
            if (hitData.forwardHitFound)
            {
                foreach (var action in parkourActions)
                {
                    if (action.CheckIfPossible(hitData, transform))
                    {
                        StartCoroutine(DoParkourAction(action, 0.5f));
                        break;
                    }
                }
            }
        }
    }

    IEnumerator DoParkourAction(ParkourAction action, float timer_value)
    {
        inAction = true;
        playerController.SetControl(false);
        animator.SetBool("mirrorAction", action.Mirror);

        animator.CrossFade(action.AnimName, stepUpTime);

        yield return null;

        var animState = animator.GetNextAnimatorStateInfo(0);
        if (!animState.IsName(action.AnimName))
            Debug.LogError("The parkour animation is wrong");

        float timer = 0f;
        while (timer <= animState.length)
        {
            timer += Time.deltaTime;

            // Rotate player towards the obstacle
            if (action.RotateToObstacle)
                transform.rotation = Quaternion.RotateTowards(transform.rotation, action.TargetRotation, playerController.RotationSpeed * Time.deltaTime);

            if (action.EnableTargetMatching && !animator.IsInTransition(0))
                MatchTarget(action);

            if (animator.IsInTransition(0) && timer > timer_value)
                break;

            yield return null;
        }

        yield return new WaitForSeconds(action.PostActionDelay);

        playerController.SetControl(true);
        inAction = false;
    }

    void MatchTarget(ParkourAction action)
    {
        if (animator.isMatchingTarget) return;
        animator.MatchTarget(action.MatchPos, transform.rotation, action.MatchBodyPart, new MatchTargetWeightMask(action.MatchPoseWeight, 0), action.MatchStartTime, action.MatchTargetTime);
    }
}
