using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraLook : MonoBehaviour
{
    [Header("Player Transition Parameter")]
    public GameObject player;

    [Header("Camera Parameters")]
    public Transform cameraTransform; //our camera transform
    private float xRotation; //X rotation speed

    // We replaced yRotation with an offset and a baseline to fix the snapping
    private float currentYOffset; //Tracks our local Y movement
    private float baseYRotation; //start with the player's scene rotation (formerly yRotation)

    public int lookSpeed = 20; //Look speed, how fast it looks

    [Header("Camera Look Limits")]
    public float maxLookUp = 85f; //Maximum angle the player can look up
    public float maxLookDown = -85f; //Maximum angle the player can look down
    public float maxLookLeft = -45f; //Maximum angle the player can look left
    public float maxLookRight = 45f; //Maximum angle the player can look right

    public void Start()
    {
        Cursor.visible = false;
        Cursor.lockState = CursorLockMode.Locked;

        //start with the player's scene rotation
        // Store the initial rotation (180) to add our clamped movement to later
        baseYRotation = transform.eulerAngles.y;
        currentYOffset = 0f;

        xRotation = cameraTransform.localEulerAngles.x; //start with the camera's scene rotation
    }

    public void Update()
    {
        Look();
    }

    public void Look()
    {
        if (Time.timeScale == 0f) return;

        float mouseX = Input.GetAxis("Mouse X") * lookSpeed;
        float mouseY = Input.GetAxis("Mouse Y") * lookSpeed;

        // Add mouse movement to the offset, not the global rotation
        currentYOffset += mouseX;

        // Clamp the offset between -45 and 45
        currentYOffset = Mathf.Clamp(currentYOffset, maxLookLeft, maxLookRight);

        xRotation -= mouseY;
        xRotation = Mathf.Clamp(xRotation, maxLookDown, maxLookUp);

        // Apply local rotation to the camera (Up/Down)
        cameraTransform.localRotation = Quaternion.Euler(xRotation, 0f, 0f);

        // Apply base rotation + clamped offset to the player (Left/Right)
        transform.rotation = Quaternion.Euler(0f, baseYRotation + currentYOffset, 0f);
    }
}