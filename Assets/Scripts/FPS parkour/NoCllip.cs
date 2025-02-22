using System;
using UnityEngine;

public class NoCllip : MonoBehaviour
{
    private bool isNoClip = false;
    public PlayerMovement pm;
    public WallRun wr;
    public LedgeGrabbing rg;

    [SerializeField] float noClipSpeed = 10f;
    private Rigidbody rb;
    [SerializeField] Transform orientation;

    private void Start()
    {
        rb = GetComponent<Rigidbody>();
    }

    private void Update()
    {
        if (isNoClip)
        {
            NoClipMovement();
        }

        if (Input.GetKeyDown(KeyCode.N)) ToggleNoClip();
    }

    private void ToggleNoClip()
    {
        pm.enabled = !pm.enabled;
        rg.enabled = !rg.enabled;
        wr.enabled = !wr.enabled;
        isNoClip = !isNoClip;
        rb.isKinematic = isNoClip;
    }

    private void NoClipMovement()
    {
        float x = Input.GetAxis("Horizontal") * noClipSpeed * Time.deltaTime;
        float y = Input.GetAxis("Vertical") * noClipSpeed * Time.deltaTime;
        float z = 0;

        if (Input.GetKey(KeyCode.Space)) z += noClipSpeed * Time.deltaTime;
        if (Input.GetKey(KeyCode.LeftControl)) z -= noClipSpeed * Time.deltaTime;

        if (Input.GetKey(KeyCode.LeftShift))
            transform.Translate(orientation.right * x * 2f + orientation.forward * y * 2f + Vector3.up * z * 2f);

        else
            transform.Translate(orientation.right * x + orientation.forward * y + Vector3.up * z);
    }
}
