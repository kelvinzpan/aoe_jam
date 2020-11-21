using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Cinemachine;

public class Player : Mirror.NetworkBehaviour
{
    public float MoveSpeed;
    public float GroundSpeedLimit;
    public float TurnSmoothTime;
    public float JumpSpeed;

    public GameObject childModel;

    private Camera playerCam;
    private CinemachineFreeLook cmFreeLook;
    private Rigidbody rb;
    private Collider col;
    private RaycastHit rcHitGround;
    private float turnSmoothVelocity;
    private float distToGround;

    // Called once at initialization
    void Awake()
    {
        cmFreeLook = GameObject.FindGameObjectWithTag("CameraController").GetComponent<CinemachineFreeLook>();
        rb = GetComponent<Rigidbody>();
        col = GetComponent<Collider>();
    }

    // Start is called before the first frame update
    void Start()
    {
        if (isLocalPlayer)
        {
            // Set up third person camera
            playerCam = Camera.main;
            cmFreeLook.Follow = this.transform;
            cmFreeLook.LookAt = this.transform;

            distToGround = col.bounds.extents.y;
        }
    }

    // Update is called once per frame
    void Update()
    {
        handleMovement();
    }

    void handleMovement()
    {
        if (isGrounded())
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
                rb.AddForce(moveDir.normalized * MoveSpeed * Time.deltaTime);
            }

            if (Input.GetKey(KeyCode.Space))
            {
                Vector3 moveDir = rcHitGround.normal;
                rb.AddForce(moveDir.normalized * JumpSpeed);
            }
        }

    }

    bool isGrounded()
    {
        if (Physics.Raycast(transform.position, -Vector3.up, out rcHitGround, 50f))
        {
            return rcHitGround.distance < (distToGround + 0.1f);
        }

        return false;
    }
}
