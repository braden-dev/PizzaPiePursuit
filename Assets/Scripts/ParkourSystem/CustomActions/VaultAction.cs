using System.Collections;
using System.Collections.Generic;
using UnityEngine;


[CreateAssetMenu(menuName = "Parkour System/Custom Actions/New Vault Action")]

public class VaultAction : ParkourAction
{
    public override bool CheckIfPossible(EnvironmentScanner.ObstacleHitData hitData, Transform player)
    {
        if (!base.CheckIfPossible(hitData, player))
            return false;
        // Check if approaching obstacle from left or right side.
        var hitPoint = hitData.forwardHit.transform.InverseTransformPoint(hitData.forwardHit.point);

        if (hitPoint.z < 0 && hitPoint.x < 0 || hitPoint.z > 0 && hitPoint.x > 0) // Approaching back and left or front and right.
        {
            // Mirror animation
            Mirror = true;
            matchBodyPart = AvatarTarget.RightHand;
        }
        else
        {
            // Don't mirror animation.
            Mirror = false;
            matchBodyPart = AvatarTarget.LeftHand;
        }

        return true;
    }
}
