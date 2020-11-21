using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Cinemachine;

public class Player : Mirror.NetworkBehaviour
{
    public float MoveSpeed;
    public float GroundSpeedLimit;
    public float TurnSmoothTime;

    private Camera playerCam;
    private CinemachineFreeLook cmFreeLook;
    private Rigidbody rb;
    private float turnSmoothVelocity;

    // Called once at initialization
    void Awake()
    {
        cmFreeLook = GameObject.FindGameObjectWithTag("CameraController").GetComponent<CinemachineFreeLook>();
        rb = GetComponent<Rigidbody>();
    }

    // Start is called before the first frame update
    void Start()
    {
        if (isLocalPlayer)
        {
            playerCam = Camera.main;

            // Set up third person camera
            cmFreeLook.Follow = this.transform;
            cmFreeLook.LookAt = this.transform;
        }
    }

    // Update is called once per frame
    void Update()
    {
        handleMovement();
    }

    private void handleMovement()
    {
        float horizontal = Input.GetAxisRaw("Horizontal");
        float vertical = Input.GetAxisRaw("Vertical");
        Vector3 direction = new Vector3(horizontal, 0f, vertical).normalized;

        // Explained in Brackeys: https://www.youtube.com/watch?v=4HpC--2iowE
        // which moves on a 2D plane and uses kinematics instead of rigidbody.
        // Can be changed, this just simulates Super Monkey Ball
        if (direction.magnitude >= 0.1f && rb.velocity.magnitude < GroundSpeedLimit)
        {
            float targetAngle = Mathf.Atan2(direction.x, direction.z) * Mathf.Rad2Deg + playerCam.transform.eulerAngles.y;
            float angle = Mathf.SmoothDampAngle(transform.eulerAngles.y, targetAngle, ref turnSmoothVelocity, TurnSmoothTime);
            transform.rotation = Quaternion.Euler(0f, angle, 0f);

            Vector3 moveDir = Quaternion.Euler(0f, targetAngle, 0f) * Vector3.forward;
            GetComponent<Rigidbody>().AddForce(moveDir.normalized * MoveSpeed * Time.deltaTime);
        }
    }
}
