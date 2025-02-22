using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using Unity.Hierarchy;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;

public class PlayerMovement : Sounds
{

    float playerHeight = 2f;


    [SerializeField] Transform orientation;
    [SerializeField] private Camera cam;

    public Climbing climbingScript;

    [Header("Movement")]
    public float moveSpeed = 0f;
    public float climbSpeed;
    [SerializeField] float airMultiplier = 0.4f;
    float movementMultiplier = 10f;
    public float crouchYscale = 0.5f;
    public float startYScale;
    public float sprintfov;
    public float walkfov;
    bool isCrouching = false;

    [Header("Sprinting")]
    public float walkSpeed = 4f;
    public float sprintSpeed = 6f;
    [SerializeField] float crouchSpeed = 2f;
    [SerializeField] float acceleration = 10f;

    [Header("Jumping")]
    public float jumpForce = 5f;

    [Header("Keybinds")]
    [SerializeField] KeyCode jumpKey = KeyCode.Space;
    [SerializeField] KeyCode sprintKey = KeyCode.LeftShift;
    [SerializeField] KeyCode crouchKey = KeyCode.LeftControl;

    [Header("Drag")]
    [SerializeField] float groundDrag = 6f;
    [SerializeField] float airDrag = 2f;

    float horizontalMovement;
    float verticalMovement;

    [Header("Ground Detection")]
    [SerializeField] Transform groundCheck;
    [SerializeField] LayerMask groundMask;
    [SerializeField] float groundDistance = 0.2f;
    public bool isGrounded { get; private set; }

    public Vector3 moveDirection;
    public float maxSlopeAngle;
    public float jumpCooldown = 0.5f;
    bool exitingSlope;
    bool readyToJump;

    public bool freeze;
    public bool unlimited;
    public bool climbing;

    public bool restricted;

    Rigidbody rb;

    RaycastHit slopeHit;

    public MovementState state;

    public enum MovementState
    {
        freeze,
        unlimited,
        climbing,
        walk,
        sprint,
        jump,
        idle
    }

    private bool OnSlope()
    {
        if (Physics.Raycast(transform.position, Vector3.down, out slopeHit, playerHeight / 2 + 0.3f))
        {
            float angle = Vector3.Angle(Vector3.up, slopeHit.normal);
            return angle < maxSlopeAngle && angle != 0;
        }
        return false;
    }

    private Vector3 GetSlopeMoveDirection()
    {
        return Vector3.ProjectOnPlane(moveDirection, slopeHit.normal).normalized;
    }

    private void Start()
    {
        rb = GetComponent<Rigidbody>();
        rb.freezeRotation = true;
        startYScale = transform.localScale.y;
        readyToJump = true;
    }

    private void Update()
    {
        isGrounded = Physics.CheckSphere(groundCheck.position, groundDistance, groundMask);

        MyInput();
        ControlDrag();
        ControlSpeed();
        Crouching();
        StateHandler();


        Debug.Log(moveSpeed + " " +  OnSlope());
    }

    void MyInput()
    {
        horizontalMovement = Input.GetAxisRaw("Horizontal");
        verticalMovement = Input.GetAxisRaw("Vertical");

        moveDirection = orientation.forward * verticalMovement + orientation.right * horizontalMovement;
        if (Input.GetKey(jumpKey) && readyToJump && isGrounded)
        {
            state = MovementState.jump;
            readyToJump = false;

            Jump();

            Invoke(nameof(ResetJump), jumpCooldown);
        }
    }

    private void Jump()
    {
        exitingSlope = true;

        // reset y velocity
        rb.linearVelocity = new Vector3(rb.linearVelocity.x, 0f, rb.linearVelocity.z);

        rb.AddForce(transform.up * jumpForce, ForceMode.Impulse);

        PlaySound(sounds[2]);

    }
    private void ResetJump()
    {
        readyToJump = true;

        exitingSlope = false;
    }


    void ControlSpeed()
    {

        if (OnSlope() && !exitingSlope)
        {
            if (Input.GetKey(sprintKey))
            {
                moveSpeed = Mathf.Lerp(moveSpeed, sprintSpeed, acceleration * Time.deltaTime);
            }

            else
            {
                moveSpeed = Mathf.Lerp(moveSpeed, walkSpeed, acceleration * Time.deltaTime);
            }

            if (rb.linearVelocity.magnitude > moveSpeed)
                rb.linearVelocity = rb.linearVelocity.normalized * moveSpeed;
        }

        else if (Input.GetKey(sprintKey) && isGrounded)
        {
            moveSpeed = Mathf.Lerp(moveSpeed, sprintSpeed, acceleration * Time.deltaTime);
        }
        else if (isCrouching && isGrounded)
        {
            moveSpeed = Mathf.Lerp(moveSpeed, crouchSpeed, acceleration * Time.deltaTime);
        }
        else if (climbing)
        {
            moveSpeed = Mathf.Lerp(moveSpeed, climbSpeed, acceleration * Time.deltaTime);
        }
        else if (isGrounded)
        {
            moveSpeed = Mathf.Lerp(moveSpeed, walkSpeed, acceleration * Time.deltaTime);
        }

        else if (!isGrounded && Input.GetKey(sprintKey))
        {
            moveSpeed = Mathf.Lerp(moveSpeed, sprintSpeed, acceleration * Time.deltaTime);
        }

        else
        {
            moveSpeed = Mathf.Lerp(moveSpeed, walkSpeed, acceleration * Time.deltaTime);
        }


        rb.useGravity = !OnSlope();

       
    }

    void ControlDrag()
    {
        if (isGrounded)
        {
            rb.linearDamping = groundDrag;
        }
        else
        {
            rb.linearDamping = airDrag;
        }
    }

    private void FixedUpdate()
    {
        MovePlayer();
    }

    void MovePlayer()
    {

        if (restricted) return;

        if (moveSpeed > walkSpeed && isGrounded && rb.linearVelocity.magnitude > 1) state = MovementState.sprint;
        else if ((isGrounded && rb.linearVelocity.magnitude < 1) || (moveSpeed == walkSpeed)) state = MovementState.walk;

        if (climbingScript.exitingWall)
        {
            return;
        }

        if (isGrounded && !OnSlope())
        {
            rb.AddForce(moveDirection.normalized * moveSpeed * movementMultiplier, ForceMode.Acceleration);
        }
        else if (OnSlope() && !exitingSlope)
        {
            rb.AddForce(GetSlopeMoveDirection() * moveSpeed * movementMultiplier, ForceMode.Acceleration);
            if (rb.linearVelocity.y > 0)
                rb.AddForce(Vector3.down * 20f, ForceMode.Acceleration);
        }
        else if (!isGrounded)
        {
            rb.AddForce(moveDirection.normalized * moveSpeed * movementMultiplier * airMultiplier, ForceMode.Acceleration);
        }



    }

    void Crouching()
    {
        if (Input.GetKeyDown(crouchKey)) {
            transform.localScale = new Vector3(transform.localScale.x, crouchYscale, transform.localScale.z);
            rb.AddForce(Vector3.down * 5f, ForceMode.Impulse);
            isCrouching = true;
        }

        if (Input.GetKeyUp(crouchKey))
        {
            transform.localScale = new Vector3(transform.localScale.x, startYScale, transform.localScale.z);
            isCrouching = false;
        }

    }

    private void StateHandler()
    {
        /*if (freeze)
        {
            state = MovementState.freeze;
            rb.linearVelocity = Vector3.zero;
            moveSpeed = 0;
        }

        else if (unlimited)
        {
            state = MovementState.unlimited;
            moveSpeed = 999f;
            return;
        }*/

        if (state == MovementState.sprint)
        {
            cam.fieldOfView = Mathf.Lerp(cam.fieldOfView, sprintfov, 5f * Time.deltaTime);
        }

        else if (state == MovementState.walk)
        {
            cam.fieldOfView = Mathf.Lerp(cam.fieldOfView, walkfov, 5f * Time.deltaTime);
        }

        else if (state == MovementState.jump && (Mathf.Abs(rb.linearVelocity.z) > 0f))
        {
            cam.fieldOfView = Mathf.Lerp(cam.fieldOfView, sprintfov, 5f * Time.deltaTime);

        }

        else if (state == MovementState.idle)
        {
            
        }
    }

}