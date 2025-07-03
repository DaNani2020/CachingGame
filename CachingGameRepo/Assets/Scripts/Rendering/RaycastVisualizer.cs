using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RaycastVisualizer : MonoBehaviour
{
    public RayDirection rayDirectionEnum = RayDirection.Forward;

    // Color of the Raycast
    public Color color = Color.blue;

    // Length of the ray
    public float rayLength = 1.0f;

    // Direction of the ray (e.g., forward direction of the object)
    private Vector3 rayDirection;

    public enum RayDirection
    {
        Forward,
        Up,
        Right
    }

    void Update()
    {

        // Set rayDirection based on the selected enum value
        switch (rayDirectionEnum)
        {
            case RayDirection.Forward:
                rayDirection = transform.forward;
                color = Color.blue;
                break;
            case RayDirection.Up:
                rayDirection = transform.up;
                color = Color.green;
                break;
            case RayDirection.Right:
                rayDirection = transform.right;
                color = Color.red;
                break;
            default:
                rayDirection = transform.forward;
                color = Color.blue;
                break;
        }

        // Origin of the ray (starting point) - This Object
        Vector3 rayOrigin = transform.position;

        // Perform the raycast
        if (Physics.Raycast(rayOrigin, rayDirection, out RaycastHit hit, rayLength))
        {
            // If the ray hits something, draw a green ray to the hit point
            Debug.DrawRay(rayOrigin, rayDirection * hit.distance, color);
        }
        else
        {
            // If the ray doesn't hit anything, draw a red ray of the specified length
            Debug.DrawRay(rayOrigin, rayDirection * rayLength, color);
        }

    }
}
