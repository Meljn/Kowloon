using UnityEngine;
using System;
using System.Runtime.CompilerServices;
using Unity.VisualScripting;
using UnityEditor.ShaderKeywordFilter;

public class Controller : MonoBehaviour
{
    [SerializeField] float walkSpeed = 5f;
    [SerializeField] float sprintSpeed = 7f;
    [SerializeField] float rotationSpeed = 500f;
    [SerializeField] float jumpHeight = 2f;

    [Header("Ground Check Settings")]
    [SerializeField] float groundCheckRadius = 0.2f;
    [SerializeField] Vector3 groundCheckOffset;
    [SerializeField] LayerMask groundLayer;

    bool isGrounded;
    bool hasControl = true;

    float ySpeed;

    Quaternion targetRotation;

    CameraController cameraController;
    Animator animator;
    CharacterController characterController;

    void Awake()
    {
        cameraController = Camera.main.GetComponent<CameraController>();
        //animator = GetComponent<Animator>();
        characterController = GetComponent<CharacterController>();
    }


    void Update()
    {
        Movement();
    }



    private void Movement()
    {
        float h = Input.GetAxis("Horizontal");
        float v = Input.GetAxis("Vertical");

        float currentSpeed = (Input.GetKey(KeyCode.LeftShift) ? sprintSpeed : walkSpeed);
        float moveAmount = Mathf.Clamp01(Math.Abs(h) + Math.Abs(v));

        var moveInput = (new Vector3(h, 0, v)).normalized;

        var moveDir = cameraController.PlanarRotation * moveInput;

        if (!hasControl) return;

        GroundCheck();
        //animator.SetBool("isGrounded", isGrounded);
        //animator.SetBool("isSprint", Input.GetKey(KeyCode.LeftShift));

        if (isGrounded)
        {
            ySpeed = -0.5f;
        }

        /*if (Input.GetButtonDown("Jump") && isGrounded)
        {
            ySpeed = Mathf.Sqrt(jumpHeight * -2f * Physics.gravity.y);
        }*/

        if (!isGrounded) 
        {
            ySpeed += Physics.gravity.y * Time.deltaTime;
        }

        var velocity = moveDir * currentSpeed;
        velocity.y = ySpeed;

        characterController.Move(velocity * Time.deltaTime);

        if (moveAmount > 0)
        {
            targetRotation = Quaternion.LookRotation(moveDir);
        }

        transform.rotation = Quaternion.RotateTowards(transform.rotation, targetRotation, rotationSpeed * Time.deltaTime);

        moveAmount = (Input.GetKey(KeyCode.LeftShift) && moveAmount > 0) ? moveAmount + 1 : moveAmount;
        //sanimator.SetFloat("Speed", moveAmount, 0.15f, Time.deltaTime);

    }

    void GroundCheck()
    {
        isGrounded = Physics.CheckSphere(transform.TransformPoint(groundCheckOffset), groundCheckRadius, groundLayer);
    }

    public void SetControl(bool hasControl)
    {
        this.hasControl = hasControl;
        characterController.enabled = hasControl;

        if (!hasControl)
        {            
            targetRotation = transform.rotation;
        }
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = new Color(0, 1, 0, 0.5f);
        Gizmos.DrawSphere(transform.TransformPoint(groundCheckOffset), groundCheckRadius);
    }

    public float RotationSpeed => rotationSpeed;

}















    /*private void Movement()
    {
        float horizontal = Input.GetAxisRaw("Horizontal");
        float vertical = Input.GetAxisRaw("Vertical");
        Vector3 direction = new Vector3(horizontal, 0f, vertical).normalized;

        if (direction.magnitude >= 0.1f)
        {
            float rotationAngle = Mathf.Atan2(direction.x, direction.z) * Mathf.Rad2Deg + firstCamera.eulerAngles.y;
            float angle = Mathf.SmoothDampAngle(transform.eulerAngles.y, rotationAngle, ref smoothVelocity, smoothTime);
            transform.rotation = Quaternion.Euler(0f, angle, 0f);

            float currentSpeed = Input.GetKey(KeyCode.LeftShift) ? sprintSpeed : walkSpeed;
            Vector3 move = Quaternion.Euler(0f, rotationAngle, 0f) * Vector3.forward;
            controller.Move(move.normalized * currentSpeed *  Time.deltaTime);
            

            //animator.SetBool("Sprint", Input.GetKey(KeyCode.LeftShift));
            //animator.SetBool("Slide", Input.GetKey (KeyCode.LeftControl));
        }

        float h = Input.GetAxis("Horizontal");
        float v = Input.GetAxis("Vertical");


        float currentMovementSpeed = Mathf.Clamp01(Mathf.Abs(h) + Mathf.Abs(v));


        if (currentMovementSpeed > 0 && Input.GetKey(KeyCode.LeftShift)) animator.SetFloat("Speed", currentMovementSpeed + 1f, .2f, Time.deltaTime);
        else animator.SetFloat("Speed", currentMovementSpeed, .1f, Time.deltaTime);

    }        

    private void Jump()
    {
        isGrounded = Physics.CheckSphere(groundCheck.position, groundDistance, groundMask);

        if (isGrounded && velocity.y < 0)
        {
            velocity.y = -2f;
        }

        if (Input.GetButtonDown("Jump") && isGrounded)
        {
            velocity.y += Mathf.Sqrt(jumpHeight * -2f * g);
            animator.SetBool("Jump", true);
        }

        else animator.SetBool("Jump", false);

        velocity.y += g * Time.deltaTime;
        controller.Move(velocity*Time.deltaTime);
    }


}*/






