using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LineRendererVisualizer : MonoBehaviour
{
    public RayDirection rayDirectionEnum = RayDirection.Forward;
    public Color color = Color.blue;
    public float rayLength = 1.0f;

    private Vector3 rayDirection;
    private LineRenderer lineRenderer;

    public enum RayDirection
    {
        Forward,
        Up,
        Right
    }

    void Start()
    {
        // Initialize the LineRenderer
        lineRenderer = gameObject.AddComponent<LineRenderer>();
        lineRenderer.startWidth = 0.01f;
        lineRenderer.endWidth = 0.01f;
        lineRenderer.positionCount = 2;
        lineRenderer.material = new Material(Shader.Find("Sprites/Default"));
    }

    void Update()
    {
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

        Vector3 rayOrigin = transform.position;
        lineRenderer.startColor = color;
        lineRenderer.endColor = color;

        if (Physics.Raycast(rayOrigin, rayDirection, out RaycastHit hit, rayLength))
        {
            lineRenderer.SetPosition(0, rayOrigin);
            lineRenderer.SetPosition(1, hit.point);
        }
        else
        {
            lineRenderer.SetPosition(0, rayOrigin);
            lineRenderer.SetPosition(1, rayOrigin + rayDirection * rayLength);
        }
    }
}
