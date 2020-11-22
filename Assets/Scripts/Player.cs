using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Cinemachine;

public class Player : MonoBehaviour
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
        // Set up third person camera
        playerCam = Camera.main;
        cmFreeLook.Follow = this.transform;
        cmFreeLook.LookAt = this.transform;

        distToGround = col.bounds.extents.y;
    }

    // FixedUpdate isn't frame dependent
    void FixedUpdate()
    {
        jumpTimer = Mathf.Max(0f, jumpTimer - Time.deltaTime);

        if (isGrounded())
        {
            //childModel.transform.rotation = Quaternion.FromToRotation(Vector3.up, rcHitGround.normal);

            // WASD
            float horizontal = Input.GetAxisRaw("Horizontal");
            float vertical = Input.GetAxisRaw("Vertical");
            Vector3 direction = new Vector3(horizontal, 0f, vertical).normalized;

            if (isMoving(direction))
            {
                moveTowards(direction);
            }

            // Space to jump
            if (Input.GetKey(KeyCode.Space) && jumpTimer <= 0f)
            {
                Vector3 moveDir = rcHitGround.normal;
                rb.AddForce(moveDir.normalized * JumpSpeed);
                jumpTimer = 0.1f;
            }
        }
    }

    // Explained in Brackeys: https://www.youtube.com/watch?v=4HpC--2iowE
    // which moves on a 2D plane and uses kinematics instead of rigidbody.
    // Can be changed, this just simulates Super Monkey Ball.
    void moveTowards(Vector3 direction)
    {
        float moveSpeed = rb.velocity.magnitude < GroundSpeedLimit ? MoveSpeed : 0f;
        float targetAngle = Mathf.Atan2(direction.x, direction.z) * Mathf.Rad2Deg + playerCam.transform.eulerAngles.y;
        float angle = Mathf.SmoothDampAngle(transform.eulerAngles.y, targetAngle, ref turnSmoothVelocity, TurnSmoothTime);
        transform.rotation = Quaternion.Euler(0f, angle, 0f);

        Vector3 moveDir = Quaternion.Euler(0f, targetAngle, 0f) * Vector3.forward;
        rb.AddForce(moveDir.normalized * moveSpeed);
    }

    bool isGrounded()
    {
        if (Physics.Raycast(transform.position, -Vector3.up, out rcHitGround, 50f))
        {
            return rcHitGround.distance < (distToGround + 0.2f);
        }

        return false;
    }

    bool isMoving(Vector3 direction)
    {
        return direction.magnitude >= 0.1f;
    }
}
