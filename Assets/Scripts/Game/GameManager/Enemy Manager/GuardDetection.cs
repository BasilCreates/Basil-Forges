using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GuardDetection : MonoBehaviour
{
    [Header("Detection Settings")]
    public float detectionRadius = 5f;
    public float detectionAngle = 60f;
    public LayerMask playerLayer;

    [Header("Detection Direction")]
    [Tooltip("Adjusts where the guard is looking without rotating the object")]
    public Vector3 detectionRotation = Vector3.zero;

    [Header("Loss Settings")]
    public string lossScene = "Vote Reveal";

    private Transform player;


    //finds the officer
    void Start()
    {
        GameObject playerObject = GameObject.FindWithTag("Player");

        if (playerObject != null)
        {
            player = playerObject.transform;
        }
    }


    //updates the axis detection logic 
    void Update()
    {
        if (player == null) return;

        float distanceToPlayer = Vector3.Distance(transform.position, player.position);

        if (distanceToPlayer <= detectionRadius)
        {
            Vector3 detectionDirection = GetDetectionDirection();

            Vector3 directionToPlayer =
                (player.position - transform.position).normalized;

            float angle = Vector3.Angle(detectionDirection, directionToPlayer);

            if (angle <= detectionAngle / 2f)
            {
                Debug.Log("Player caught by guard!");
                PlayerCaught();
            }
        }
    }


    //determines the direction of the detection
    Vector3 GetDetectionDirection()
    {
        return transform.rotation *
               (Quaternion.Euler(detectionRotation) * Vector3.forward);
    }


    //caught sequence
    void PlayerCaught()
    {
        GameManager.Instance.suspicionLoss = true;
        SceneManager.LoadScene(lossScene);
    }

    //draws the lines so we can see in the level when adjusting
    void OnDrawGizmos()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, detectionRadius);


        Vector3 detectionDirection = GetDetectionDirection();


        Vector3 leftBoundary =
            Quaternion.Euler(0, -detectionAngle / 2f, 0) *
            detectionDirection;

        Vector3 rightBoundary =
            Quaternion.Euler(0, detectionAngle / 2f, 0) *
            detectionDirection;


        Gizmos.color = Color.yellow;

        Gizmos.DrawRay(
            transform.position,
            leftBoundary * detectionRadius
        );

        Gizmos.DrawRay(
            transform.position,
            rightBoundary * detectionRadius
        );


        Gizmos.color = Color.green;

        Gizmos.DrawRay(
            transform.position,
            detectionDirection * detectionRadius
        );
    }
}