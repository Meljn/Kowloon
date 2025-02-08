using UnityEngine;
using System;
using System.Runtime.CompilerServices;
using Unity.VisualScripting;
using UnityEditor.ShaderKeywordFilter;

public class FPSController : MonoBehaviour
{
    CharacterController characterController;
    CameraController cameraController;

    [Header("Movement Settings")]
    [SerializeField] float walkSpeed;
    [SerializeField] float sprintSpeed;
    float currentSpeed;
    [SerializeField] float jumpHeight;

    [Header("Ground Check Settings")]
    [SerializeField] float groundCheckRadius = 0.2f;
    [SerializeField] Vector3 groundCheckOffset;
    [SerializeField] LayerMask groundLayer;
    bool isGrounded;
    float ySpeed;

    [SerializeField] float fovSmooth;
    float defaultCameraFOV;
    float currentFOV;
    float targetFOV;
    float sprintCameraFOV;


    void Start()
    {
        defaultCameraFOV = Camera.main.fieldOfView;
        currentFOV = Camera.main.fieldOfView;
        sprintCameraFOV = Camera.main.fieldOfView + 15;
        targetFOV = currentFOV;

        characterController = GetComponent<CharacterController>();
        cameraController = Camera.main.GetComponent<CameraController>();
    }

    
    void Update()
    {
        Movement();
    }

    void Movement()
    {
        float h = Input.GetAxisRaw("Horizontal");
        float v = Input.GetAxisRaw("Vertical");


        //Debug.Log(currentFOV + " " + sprintCameraFOV);

        if (Input.GetKey(KeyCode.LeftShift) && Input.GetKey(KeyCode.W)) currentSpeed = sprintSpeed;
        else currentSpeed = walkSpeed;

        targetFOV = (currentSpeed == sprintSpeed) ? sprintCameraFOV : defaultCameraFOV;
        currentFOV = Mathf.Lerp(currentFOV, targetFOV, Time.deltaTime * fovSmooth);
        Debug.Log(currentFOV);
        Camera.main.fieldOfView = currentFOV;

        var moveInput = new Vector3(h, 0, v).normalized;
        var moveDir = cameraController.PlanarRotation * moveInput;


        GroundCheck();

        if (isGrounded)
        {
            ySpeed = -0.5f;
        }

        if (Input.GetButtonDown("Jump") && isGrounded)
        {
            ySpeed = Mathf.Sqrt(jumpHeight * -2f * Physics.gravity.y);
        }

        if (!isGrounded)
        {
            ySpeed += Physics.gravity.y * Time.deltaTime;
        }


        var velocity =  moveDir * currentSpeed;
        velocity.y = ySpeed;

        Debug.Log(isGrounded);

        characterController.Move(velocity * Time.deltaTime);
    }

    void GroundCheck()
    {
        isGrounded = Physics.CheckSphere(transform.TransformPoint(groundCheckOffset), groundCheckRadius, groundLayer);
    }


    private void OnDrawGizmosSelected()
    {
        Gizmos.color = new Color(0, 1, 0, 0.5f);
        Gizmos.DrawSphere(transform.TransformPoint(groundCheckOffset), groundCheckRadius);
    }
}
