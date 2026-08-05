using System.Collections;
using UnityEngine;

public class GuardRotation : MonoBehaviour
{
    public enum RotationAxis { X, Y, Z }

    [Header("Axis Settings")]
    [Tooltip("Choose which axis the guard should rotate around.")]
    public RotationAxis rotationAxis = RotationAxis.Y;

    [Header("Angle Settings")]
    public float startAngle = 0f;    // The starting angle of the sweep
    public float endAngle = 90f;     // The ending angle of the sweep

    [Header("Speed & Pause")]
    public float rotationSpeed = 45f;    // How fast they rotate
    public float waitTime = 2f;          // How long they wait at each endpoint

    private Quaternion rotationA;
    private Quaternion rotationB;

    void Start()
    {
        // Capture the base orientation of the guard for X, Y, and Z
        Vector3 baseEuler = transform.eulerAngles;

        // Build the two target rotations based on the chosen axis and angles
        Vector3 eulerA = baseEuler;
        Vector3 eulerB = baseEuler;

        switch (rotationAxis)
        {
            case RotationAxis.X:
                eulerA.x = startAngle;
                eulerB.x = endAngle;
                break;
            case RotationAxis.Y:
                eulerA.y = startAngle;
                eulerB.y = endAngle;
                break;
            case RotationAxis.Z:
                eulerA.z = startAngle;
                eulerB.z = endAngle;
                break;
        }

        rotationA = Quaternion.Euler(eulerA);
        rotationB = Quaternion.Euler(eulerB);

        // Snap to the start position initially
        transform.rotation = rotationA;

        StartCoroutine(RotationPattern());
    }

    IEnumerator RotationPattern()
    {
        while (true)
        {
            // Rotate from Start Angle to End Angle
            yield return StartCoroutine(RotateTo(rotationB));

            yield return new WaitForSeconds(waitTime);

            // Rotate back from End Angle to Start Angle
            yield return StartCoroutine(RotateTo(rotationA));

            yield return new WaitForSeconds(waitTime);
        }
    }

    IEnumerator RotateTo(Quaternion target)
    {
        while (Quaternion.Angle(transform.rotation, target) > 0.1f)
        {
            transform.rotation = Quaternion.RotateTowards(
                transform.rotation,
                target,
                rotationSpeed * Time.deltaTime
            );

            yield return null;
        }

        transform.rotation = target;
    }
}