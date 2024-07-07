using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    private float moveSpeed = 7f;
    private float rotationSpeed = 500f;

    //[Header("Ground Check Settings")]
    private float groundCheckRadius = 0.2f;
    private Vector3 groundCheckOffset = new Vector3(0f, 0.1f, 0.07f);
    private LayerMask groundLayer;
    private string groundLayerName = "Obstacles";

    private bool isGrounded;
	public bool IsGrounded => isGrounded;
    private bool hasControl = true;

    private Vector3 desiredMoveDir;
    private Vector3 moveDir;
    private Vector3 velocity;

    public bool IsOnLedge { get; set; }
    public LedgeData LedgeData { get; set; }

    private float ySpeed;
    private Quaternion targetRotation;

    private CameraController cameraController;
    private Animator animator;
    private CharacterController characterController;
    private EnvironmentScanner environmentScanner;

    private void Awake()
    {
        cameraController =  Camera.main.GetComponent<CameraController>();
        animator = GetComponent<Animator>();
        characterController = GetComponent<CharacterController>();
        environmentScanner = GetComponent<EnvironmentScanner>();
        groundLayer = LayerMask.GetMask(groundLayerName);
    }

    private void Update()
    {
        float h = Input.GetAxis("Horizontal");
        float v = Input.GetAxis("Vertical");

        float moveAmount = Mathf.Clamp01(Mathf.Abs(h) + Mathf.Abs(v));

        var moveInput = (new Vector3(h, 0, v)).normalized;

        desiredMoveDir = cameraController.PlanarRotation * moveInput;
        moveDir = desiredMoveDir;

        if (!hasControl)
            return;

        velocity = Vector3.zero;

        GroundCheck();
        animator.SetBool("isGrounded", isGrounded);
        if (isGrounded)
        {
            ySpeed = -0.5f;
            velocity = desiredMoveDir * moveSpeed;

            IsOnLedge = environmentScanner.LedgeCheck(desiredMoveDir, out LedgeData ledgeData);
            if (IsOnLedge)
            {
                LedgeData = ledgeData;
                LedgeMovement();
            }

            animator.SetFloat("moveAmount", velocity.magnitude / moveSpeed, 0.2f, Time.deltaTime);
        }
        else
        {
            ySpeed += Physics.gravity.y * Time.deltaTime;

            velocity = transform.forward * moveSpeed / 2;
        }

        
        velocity.y = ySpeed;

        characterController.Move(velocity * Time.deltaTime);

        if (moveAmount > 0 && moveDir.magnitude > 0.2f)
        {
            targetRotation = Quaternion.LookRotation(moveDir);
        }

        transform.rotation = Quaternion.RotateTowards(transform.rotation, targetRotation, 
            rotationSpeed * Time.deltaTime);

        
    }

    void GroundCheck()
    {
        isGrounded = Physics.CheckSphere(transform.TransformPoint(groundCheckOffset), groundCheckRadius, groundLayer);
    }

    void LedgeMovement()
    {
        float signedAngle = Vector3.SignedAngle(LedgeData.surfaceHit.normal, desiredMoveDir, Vector3.up);
        float angle = Mathf.Abs(signedAngle);

        if (Vector3.Angle(desiredMoveDir, transform.forward) >= 80)
        {
            // Don't move, but rotate
            velocity = Vector3.zero;
            return;
        }

        if (angle < 60)
        {
            velocity = Vector3.zero;
            moveDir = Vector3.zero;
        }
        else if (angle < 90)
        {
            // Angle is b/w 60 and 90, so limit the velocity to horizontal direction

            var left = Vector3.Cross(Vector3.up, LedgeData.surfaceHit.normal);
            var dir = left * Mathf.Sign(signedAngle);

            velocity = velocity.magnitude * dir;
            moveDir = dir;
        }
    }

	public void Jump(float force) 
    {
		if (isGrounded)
        {
        	ySpeed = force;
			isGrounded = false;
			animator.SetTrigger("jump");
		}
	}


    public void SetControl(bool hasControl)
    {
        this.hasControl = hasControl;
        characterController.enabled = hasControl;

        if (!hasControl)
        {
            animator.SetFloat("moveAmount", 0f);
            targetRotation = transform.rotation;
        }
    }

    public bool HasControl {
        get => hasControl;
        set => hasControl = value;
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = new Color(0, 1, 0, 0.5f);
        Gizmos.DrawSphere(transform.TransformPoint(groundCheckOffset), groundCheckRadius);
    }

    public float RotationSpeed => rotationSpeed;
}
