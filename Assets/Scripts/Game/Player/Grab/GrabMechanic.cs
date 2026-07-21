using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GrabMechanic : MonoBehaviour
{
    [Header("Camera Mechanics")]
    public PlayerMovement playerMovement;
    public Transform cameraTransform;

    [Header("Grab Mechanics")]
    public Transform grabPoint;
    public int grabRange = 5;
    public LayerMask grabbableLayer;

    private Transform heldObject;

    public void Start()
    {
        cameraTransform = playerMovement.cameraTransform;
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.E))
        {
            if (heldObject == null)
                Grab();
            else
                Drop();
        }

        HoldObject(); // IMPORTANT: keeps object attached every frame
    }

    public void Grab()
    {
        Ray ray = new Ray(cameraTransform.position, cameraTransform.forward);
        RaycastHit hit;

        if (Physics.Raycast(ray, out hit, grabRange, grabbableLayer))
        {
            heldObject = hit.transform;

            // Snap immediately into hand
            heldObject.position = grabPoint.position;
            heldObject.rotation = grabPoint.rotation;

            Debug.Log("Hit the object! Grabbing it...");
        }
    }

    public void HoldObject()
    {
        if (heldObject != null)
        {
            heldObject.position = grabPoint.position;
            heldObject.rotation = grabPoint.rotation;
        }
    }

    public void Drop()
    {
        heldObject = null;
    }
}