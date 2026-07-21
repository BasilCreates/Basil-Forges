using System.Collections;
using System.Collections.Generic;
using UnityEngine;
public class PlayerMovement : MonoBehaviour
{
    //The basic player movement parameters
    [Header("Player Attributes")]
    public int movementSpeed = 10;
    public int lookSpeed = 20;
    public Rigidbody rb;
    [Header("Camera")]
    public Transform cameraTransform;
    private Vector3 move;
    private float xRotation;
    private float yRotation;

    // Grabs the rigidbody and locks the cursor on start
    public void Start()
    {
        rb = GetComponent<Rigidbody>();
    }

    // Calls move and look every frame
    public void Update()
    {
        Move();
        Look();
    }

    // Handles player movement input from WASD or arrow keys
    public void Move()
    {
        if (Time.timeScale == 0f) return;
        float x = Input.GetAxisRaw("Horizontal");
        float z = Input.GetAxisRaw("Vertical");
        move = new Vector3(x, 0f, z).normalized;
    }

    // Handles mouse look rotation for both player and camera
    public void Look()
    {
        if (Time.timeScale == 0f) return;
        float mouseX = Input.GetAxis("Mouse X") * lookSpeed;
        float mouseY = Input.GetAxis("Mouse Y") * lookSpeed;
        yRotation += mouseX;
        xRotation -= mouseY;
        xRotation = Mathf.Clamp(xRotation, -85f, 85f);
        cameraTransform.localRotation = Quaternion.Euler(xRotation, 0f, 0f);
        transform.rotation = Quaternion.Euler(0f, yRotation, 0f);
    }

    // Applies the movement velocity to the rigidbody on a fixed timestep
    public void FixedUpdate()
    {
        rb.velocity = MoveDirection() * movementSpeed;
    }

    // Calculates camera relative movement direction from player input
    private Vector3 MoveDirection()
    {
        Vector3 forward = cameraTransform.forward;
        Vector3 right = cameraTransform.right;
        forward.y = 0f;
        right.y = 0f;
        forward.Normalize();
        right.Normalize();
        Vector3 direction =
            forward * move.z +
            right * move.x;
        return direction;
    }
}