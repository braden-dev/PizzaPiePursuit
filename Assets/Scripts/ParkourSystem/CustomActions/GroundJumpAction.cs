using UnityEngine;

[CreateAssetMenu(menuName = "Parkour System/Custom Actions/New Ground Jump Action")]
public class GroundJumpAction : ParkourAction
{
    [SerializeField] float jumpForce = 5f;
    [SerializeField] float jumpDuration = 0.5f;

    public override bool CheckIfPossible(EnvironmentScanner.ObstacleHitData hitData, Transform player)
    {
        PlayerController playerController = player.GetComponent<PlayerController>();
        return playerController.IsGrounded;
    }

    public float JumpForce => jumpForce;
    public float JumpDuration => jumpDuration;
}