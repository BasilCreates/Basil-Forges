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

    [Header("Chase Settings")]
    public float chaseSpeed = 3.5f;
    public float rotationSpeed = 5f;
    public float loseRadius = 8f; // Distance at which the guard gives up the chase
    public AudioSource heySound;

    [Header("Animator")]
    public Animator guardAnimator;

    [Header("Loss Settings")]
    public string lossScene = "Vote Reveal";

    private Transform player;
    private bool isChasing = false;
    private CharacterController characterController; // Reference to Character Controller

    // Finds the player and references
    void Start()
    {
        GameObject playerObject = GameObject.FindWithTag("Player");

        if (playerObject != null)
        {
            player = playerObject.transform;
        }

        // Grab the Character Controller component automatically
        characterController = GetComponent<CharacterController>();
        if (characterController == null)
        {
            Debug.LogWarning("No Character Controller found on the Guard! Please add one so they don't walk through walls.");
        }
    }

    // Updates detection and chase logic
    void Update()
    {
        if (player == null) return;

        float distanceToPlayer = Vector3.Distance(transform.position, player.position);

        if (!isChasing)
        {
            // Searching/Scanning logic
            if (distanceToPlayer <= detectionRadius)
            {
                Vector3 detectionDirection = GetDetectionDirection();
                Vector3 directionToPlayer = (player.position - transform.position).normalized;
                float angle = Vector3.Angle(detectionDirection, directionToPlayer);

                if (angle <= detectionAngle / 2f)
                {
                    Debug.Log("Player spotted! Starting chase.");
                    heySound.Play();
                    StartChase();
                }
            }
        }
        else
        {
            // Check if the player has escaped the chase range
            if (distanceToPlayer > loseRadius)
            {
                Debug.Log("Player escaped! Returning to scan.");
                StopChase();
                return;
            }

            // Chasing logic: Turn Z forward to the player
            Vector3 targetDirection = player.position - transform.position;
            targetDirection.y = 0f; // Keep rotation level on the ground plane

            if (targetDirection != Vector3.zero)
            {
                Quaternion targetRotation = Quaternion.LookRotation(targetDirection);
                transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, rotationSpeed * Time.deltaTime);
            }

            // Move guard using Character Controller so they collide with walls/obstacles properly!
            if (characterController != null)
            {
                Vector3 moveDir = transform.forward * chaseSpeed;
                // Add a small gravity pull down so they stick to the floor
                moveDir.y = -9.81f;
                characterController.Move(moveDir * Time.deltaTime);
            }
            else
            {
                // Fallback if component is missing
                transform.position += transform.forward * chaseSpeed * Time.deltaTime;
            }

            // Check if guard actually catches the player
            if (distanceToPlayer <= 1.2f)
            {
                Debug.Log("Player caught by guard!");
                PlayerCaught();
            }
        }
    }

    // Switches state to chasing and updates the Animator
    void StartChase()
    {
        isChasing = true;

        if (guardAnimator != null)
        {
            guardAnimator.SetBool("SeePlayer", true);
        }
    }

    // Switches state back to scanning when player escapes
    void StopChase()
    {
        isChasing = false;

        if (guardAnimator != null)
        {
            guardAnimator.SetBool("SeePlayer", false);
        }
    }

    // Determines the direction of the detection
    Vector3 GetDetectionDirection()
    {
        return transform.rotation *
               (Quaternion.Euler(detectionRotation) * Vector3.forward);
    }

    // Caught sequence
    void PlayerCaught()
    {
        GameManager.Instance.suspicionLoss = true;
        SceneManager.LoadScene(lossScene);
    }

    // Draws the lines so we can see in the level when adjusting
    void OnDrawGizmos()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, detectionRadius);

        // Draw lose radius circle in orange
        Gizmos.color = new Color(1f, 0.5f, 0f);
        Gizmos.DrawWireSphere(transform.position, loseRadius);

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