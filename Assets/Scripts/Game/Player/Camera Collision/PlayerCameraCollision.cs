using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerCameraCollision : MonoBehaviour
{
    [Header("Camera Settings")]
    public Transform player;
    public float cameraDistance = 5f;
    public float minimumDistance = 1f;
    public float collisionRadius = 0.2f;

    [Header("Layers")]
    public LayerMask collisionLayers;

    private Vector3 cameraDirection;


    void LateUpdate()
    {
        if (player == null)
            return;

        cameraDirection = (transform.position - player.position).normalized;

        RaycastHit hit;

        if (Physics.SphereCast(
            player.position,
            collisionRadius,
            cameraDirection,
            out hit,
            cameraDistance,
            collisionLayers))
        {
            float distance = hit.distance;

            transform.position =
                player.position +
                cameraDirection * Mathf.Max(distance, minimumDistance);
        }
        else
        {
            transform.position =
                player.position +
                cameraDirection * cameraDistance;
        }
    }
}
