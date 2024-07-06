using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ParkourController : MonoBehaviour
{
    [SerializeField] List<ParkourAction> parkourActions;
    [SerializeField] ParkourAction jumpDownAction;
    [SerializeField] float autoDropHeightLimit = 1f;

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

        if (Input.GetKeyUp(KeyCode.Space) && !inAction)
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
                        StartCoroutine(DoParkourAction(action));
                        break;
                    }
                }
            }
        }
        
        if (playerController.IsOnLedge && !inAction && !hitData.forwardHitFound)
        {
            bool shouldJump = true;
            if (playerController.LedgeData.height > autoDropHeightLimit && !Input.GetKeyUp(KeyCode.Space))
                shouldJump = false;

            if (shouldJump && playerController.LedgeData.angle <= 50)
            {
                playerController.IsOnLedge = false;
                StartCoroutine(DoParkourAction(jumpDownAction));
            }
        }

        if (environmentScanner.ClearPathSlideCheck() && Input.GetKeyUp(KeyCode.LeftShift) && !inAction && playerController.IsGrounded)
        {
            StartCoroutine(DoParkourAction(parkourActions[4]));
        }

        if (environmentScanner.ClearPathJumpCheck() && !hitData.forwardHitFound && playerController.IsGrounded && Input.GetKeyUp(KeyCode.Space) && !inAction)
        {
            StartCoroutine(DoParkourAction(parkourActions[5]));
        }

    }

    IEnumerator DoParkourAction(ParkourAction action)
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

            if (animator.IsInTransition(0) && timer > 0.5f)
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
