using System.Collections;
using UnityEngine;

public class GuardRotation : MonoBehaviour
{
    [Header("Rotation Settings")]
    public float rotationAngle = 90f;    // How far they rotate
    public float rotationSpeed = 45f;    // How fast they rotate
    public float waitTime = 2f;          // How long they wait before rotating back

    private Quaternion startRotation;
    private Quaternion targetRotation;

    //setting our target rotation and then calling our coroutine
    void Start()
    {
        startRotation = transform.rotation;

        // Rotate only around Z axis
        targetRotation = Quaternion.Euler(
            transform.eulerAngles.x,
            transform.eulerAngles.y,
            transform.eulerAngles.z + rotationAngle
        );

        StartCoroutine(RotationPattern());
    }

    //delaying before beginning the rotation
    IEnumerator RotationPattern()
    {
        while (true)
        {
            yield return StartCoroutine(RotateTo(targetRotation));

            yield return new WaitForSeconds(waitTime);

            yield return StartCoroutine(RotateTo(startRotation));

            yield return new WaitForSeconds(waitTime);
        }
    }

    //starts and sets our ratation
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