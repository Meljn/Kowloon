using UnityEngine;
using System;
using System.Runtime.CompilerServices;
using Unity.VisualScripting;

public class Controller : MonoBehaviour
{
    private Animator animator;
    private CharacterController controller;
    public float walkSpeed;
    public float sprintSpeed;
    public float smoothTime;
    float smoothVelocity;
    public Transform firstCamera;


    private float g = -9.8f;
    public float jumpHeight = 2.0f;
    private bool isGrounded;
    private Vector3 velocity;

    public Transform groundCheck;
    public float groundDistance = 0.4f;
    public LayerMask groundMask;

    void Start()
    {
        //groundCheck = GetComponent<Transform>();
        controller = GetComponent<CharacterController>();
        animator = GetComponent<Animator>();
        
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
    }


    void Update()
    {
        Movement();
        Jump();

    }

    private void Movement()
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


}






