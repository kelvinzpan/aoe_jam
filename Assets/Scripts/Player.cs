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
    private float jumpTimer;

    // Called once at initialization
    void Awake()
    {
        cmFreeLook = GameObject.FindGameObjectWithTag("CameraController").GetComponent<CinemachineFreeLook>();
        rb = GetComponent<Rigidbody>();
        col = GetComponent<Collider>();
        jumpTimer = 0f;
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
        jumpTimer = Mathf.Max(0f, jumpTimer - Time.deltaTime);

        if (isGrounded())
        {
            // WASD
            float horizontal = Input.GetAxisRaw("Horizontal");
            float vertical = Input.GetAxisRaw("Vertical");
            Vector3 direction = new Vector3(horizontal, 0f, vertical).normalized;

            turnTowards(direction);

            // Space to jump
            if (Input.GetKey(KeyCode.Space) && jumpTimer <= 0f)
            {
                Vector3 moveDir = rcHitGround.normal;
                rb.AddForce(moveDir.normalized * JumpSpeed);
                jumpTimer = 0.1f;
            }

            childModel.transform.rotation = Quaternion.FromToRotation(Vector3.up, rcHitGround.normal);
        }
    }

    // Explained in Brackeys: https://www.youtube.com/watch?v=4HpC--2iowE
    // which moves on a 2D plane and uses kinematics instead of rigidbody.
    // Can be changed, this just simulates Super Monkey Ball.
    void turnTowards(Vector3 direction)
    {
        if (direction.magnitude >= 0.1f && rb.velocity.magnitude < GroundSpeedLimit)
        {
            float targetAngle = Mathf.Atan2(direction.x, direction.z) * Mathf.Rad2Deg + playerCam.transform.eulerAngles.y;
            float angle = Mathf.SmoothDampAngle(transform.eulerAngles.y, targetAngle, ref turnSmoothVelocity, TurnSmoothTime);
            transform.rotation = Quaternion.Euler(0f, angle, 0f);

            Vector3 moveDir = Quaternion.Euler(0f, targetAngle, 0f) * Vector3.forward;
            rb.AddForce(moveDir.normalized * MoveSpeed * Time.deltaTime);
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
