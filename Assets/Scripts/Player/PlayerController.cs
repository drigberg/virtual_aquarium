// Some stupid rigidbody based movement by Dani

using System;
using UnityEngine;

public class PlayerController : MonoBehaviour {

    public Transform playerCam;
    public Transform orientation;
    public Rigidbody rb;
    public GUI gui;
    public float moveSpeed = 4500;
    public float maxSpeed = 20;

    private Vector3 playerScale;
    private float xRotation;
    private float sensitivity = 100;
    private float sensMultiplier = 1f;


    // Input
    float x, y, z;
    bool jump;

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
        if (!gui.paused) {
            Look();
        }
    }

    /// <summary>
    /// Find user input. Should put this in its own class but im lazy
    /// </summary>
    private void MyInput() {
        x = Input.GetAxisRaw("Horizontal");
        y = Input.GetAxisRaw("Vertical");
        jump = Input.GetKey(KeyCode.Space);
    }

    private void Movement() {
        // Save some effort if no buttons are pressed
        if (x == 0 && y == 0 && !jump) {
            return;
        }

        Vector2 mag = FindVelRelativeToLook();
        float xMag = mag.x, yMag = mag.y, zMag = rb.velocity.y;

        if (jump) {
            z = 2;
        } else {
            z = 0;
        }

        // If speed is larger than maxspeed, cancel out the input so you don't go over max speed
        if (x > 0 && xMag > maxSpeed) x = 0;
        if (x < 0 && xMag < -maxSpeed) x = 0;
        if (y > 0 && yMag > maxSpeed) y = 0;
        if (y < 0 && yMag < -maxSpeed) y = 0;
        if (z > 0 && zMag > maxSpeed) z = 0;
        if (z < 0 && zMag < -maxSpeed) z = 0;

        // Apply forces to move player
        rb.AddForce(orientation.transform.forward * y * moveSpeed * Time.deltaTime);
        rb.AddForce(orientation.transform.right * x * moveSpeed * Time.deltaTime);
        rb.AddForce(orientation.transform.up * z * moveSpeed * Time.deltaTime);
    }


    private float desiredX;
    private void Look() {
        float mouseX = Input.GetAxis("Mouse X") * sensitivity * Time.fixedDeltaTime * sensMultiplier;
        float mouseY = Input.GetAxis("Mouse Y") * sensitivity * Time.fixedDeltaTime * sensMultiplier;

        // Find current look rotation
        Vector3 rot = playerCam.transform.localRotation.eulerAngles;
        desiredX = rot.y + mouseX;

        // Rotate, and also make sure we dont over- or under-rotate.
        xRotation -= mouseY;
        xRotation = Mathf.Clamp(xRotation, -90f, 90f);

        // Perform the rotations
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