using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FloatingArrow : MonoBehaviour
{
    [Header("Movement Settings")]
    public float height = 0.5f;      // How far it moves up/down
    public float speed = 2f;         // How fast it moves

    private Vector3 startPosition;


    void Start()
    {
        startPosition = transform.position;
    }


    void Update()
    {
        float newY = startPosition.y + Mathf.Sin(Time.time * speed) * height;

        transform.position = new Vector3(
            startPosition.x,
            newY,
            startPosition.z
        );
    }
}