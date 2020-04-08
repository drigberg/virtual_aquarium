// Some stupid rigidbody based movement by Dani

using System;
using UnityEngine;

public class DiverMovement : MonoBehaviour {

    public Transform playerCam;
    public Transform orientation;
    public float moveSpeed = 4500;
    public float maxSpeed = 20;
    public float sprintMultiplier = 2.0f;

    private Rigidbody rb;
    private Vector3 playerScale;
    private float xRotation;
    private float sensitivity = 50f;
    private float sensMultiplier = 1f;
    private bool sprint;


    // Input
    float x, y, z;
    bool ascend, descend;

    void Awake() {
        rb = GetComponent<Rigidbody>();
    }
    
    void Start() {
        playerScale =  transform.localScale;
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
    }

    
    private void FixedUpdate() {
        Movement();
    }

    private void Update() {
        MyInput();
        Look();
    }

    /// <summary>
    /// Find user input. Should put this in its own class but im lazy
    /// </summary>
    private void MyInput() {
        x = Input.GetAxisRaw("Horizontal");
        y = Input.GetAxisRaw("Vertical");
        ascend = Input.GetKey(KeyCode.E);
        descend = Input.GetKey(KeyCode.Q);
        sprint = Input.GetKey(KeyCode.Space);
    }

    private void Movement() {
        //Find actual velocity relative to where player is looking
        Vector2 mag = FindVelRelativeToLook();
        float xMag = mag.x, yMag = mag.y, zMag = rb.velocity.y;

        //Set max speed
        float currentMaxSpeed = maxSpeed;
        float currentMoveSpeed = moveSpeed;
        if (sprint) {
            currentMaxSpeed *= sprintMultiplier;
            currentMoveSpeed *= sprintMultiplier;
        }
        
        if (ascend) z = 1;
        if (descend) z = -1;

        //If speed is larger than maxspeed, cancel out the input so you don't go over max speed
        if (x > 0 && xMag > currentMaxSpeed) x = 0;
        if (x < 0 && xMag < -currentMaxSpeed) x = 0;
        if (y > 0 && yMag > currentMaxSpeed) y = 0;
        if (y < 0 && yMag < -currentMaxSpeed) y = 0;
        if (z > 0 && zMag > currentMaxSpeed) z = 0;
        if (z < 0 && zMag < -currentMaxSpeed) z = 0;
        if (!ascend && !descend) z = zMag * -0.1f;

        //Apply forces to move player
        rb.AddForce(orientation.transform.forward * y * currentMoveSpeed * Time.deltaTime);
        rb.AddForce(orientation.transform.right * x * currentMoveSpeed * Time.deltaTime);
        rb.AddForce(orientation.transform.up * z * currentMoveSpeed * Time.deltaTime);
    }

    
    private float desiredX;
    private void Look() {
        float mouseX = Input.GetAxis("Mouse X") * sensitivity * Time.fixedDeltaTime * sensMultiplier;
        float mouseY = Input.GetAxis("Mouse Y") * sensitivity * Time.fixedDeltaTime * sensMultiplier;

        //Find current look rotation
        Vector3 rot = playerCam.transform.localRotation.eulerAngles;
        desiredX = rot.y + mouseX;
        
        //Rotate, and also make sure we dont over- or under-rotate.
        xRotation -= mouseY;
        xRotation = Mathf.Clamp(xRotation, -90f, 90f);

        //Perform the rotations
        playerCam.transform.localRotation = Quaternion.Euler(xRotation, desiredX, 0);
        orientation.transform.localRotation = Quaternion.Euler(0, desiredX, 0);
    }

    /// <summary>
    /// Find the velocity relative to where the player is looking
    /// Useful for vectors calculations regarding movement and limiting movement
    /// </summary>
    /// <returns></returns>
    public Vector2 FindVelRelativeToLook() {
        float lookAngle = orientation.transform.eulerAngles.y;
        float moveAngle = Mathf.Atan2(rb.velocity.x, rb.velocity.z) * Mathf.Rad2Deg;

        float u = Mathf.DeltaAngle(lookAngle, moveAngle);
        float v = 90 - u;

        float magnitude = rb.velocity.magnitude;
        float yMag = magnitude * Mathf.Cos(u * Mathf.Deg2Rad);
        float xMag = magnitude * Mathf.Cos(v * Mathf.Deg2Rad);
        
        return new Vector2(xMag, yMag);
    }
}