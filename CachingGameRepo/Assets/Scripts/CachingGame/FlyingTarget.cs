using System.Collections;
using System.Collections.Generic;
using UnityEngine;


/// <summary>
/// FlyingTarget class moves object based on the set speed during its lifeSpan, and projects a fly path.
/// </summary>
public class FlyingTarget : MonoBehaviour
{

    public float maxFlySpeed = 5f;
    private float flySpeed;

    [Tooltip("Life span of the GameObject this script is attached to.")]
    private float lifeSpan = 25f;
    private Vector3 startingPoint;

    [Header("Visualization of fly path projection.")]
    public Texture2D dashedTexture;
    private LineRenderer lineRenderer;

    [Tooltip("Length of fly path projection.")]
    public float rayDistance = 10f;
    public Color rayColor;
    public float rayOpacity = 0.5f;


    /// <summary>
    /// Start method positions the object, updates the renderer with color for the ray visualization and sets the fly speed.
    /// </summary>
    void Start()
    {
        startingPoint = this.transform.position;

        Renderer renderer = GetComponent<Renderer>();
        if (renderer != null && renderer.material != null)
        {
            rayColor = renderer.material.color;
        }
        else
        {
            rayColor = Color.green;
        }

        // Set up the LineRenderer component
        if (lineRenderer == null)
        {
            lineRenderer = GetComponent<LineRenderer>();
        }

        SetUpLineRenderer();

        flySpeed = maxFlySpeed;
    }


    /// <summary>
    /// Update refreshs the position and distance every frame to move the object along its forward vector, as well as the fly path visualization.
    /// Additionally, it sets after a lifespan the ObjectInterationState of the attached gameObject to EXPIRED and destroys it.
    /// </summary>
    void Update()
    {
        transform.position += Time.deltaTime * transform.forward * flySpeed;

        // Calculate the distance between the object and the reference point
        float distance = Vector3.Distance(transform.position, startingPoint);

        // If the distance exceeds the threshold, destroy the object
        if (distance >= lifeSpan)
        {
            //ScoreManager.instance.MissedBall();
            string uuidReference = Helper.TryGetUuid(gameObject);
            if (uuidReference == string.Empty) UnityEngine.Debug.LogError("TryGetUuid in Colision Collectible NOT WORKING: " + uuidReference + ".");
            SpawnedObjectTracker.instance.SetStateByUuid(uuidReference, ObjectInteractionState.EXPIRED);
            UnityEngine.Debug.Log("Changed interaction State Expired based on uuidReference " + uuidReference + " to " + SpawnedObjectTracker.instance.GetStateByUuid(uuidReference));
            Destroy(gameObject);
        }

        VisualizeRay(transform.position, transform.forward, rayDistance, rayColor, rayOpacity);
    }


    /// <summary>
    /// VisualizeRay determines start and end point, and the according color with opacity.
    /// </summary>
    /// <param name="startPosition">The current position of the object.</param>
    /// <param name="direction">The direction of the extending ray.</param>
    /// <param name="distance">The intended length of the ray.</param>
    /// <param name="color">The color of the ray.</param>
    /// <param name="opacity">The opacity of the ray.</param>
    void VisualizeRay(Vector3 startPosition, Vector3 direction, float distance, Color color, float opacity)
    {
        // Set the positions of the LineRenderer: start and end based on the distance
        lineRenderer.positionCount = 2;
        Vector3 endPosition = startPosition + direction.normalized * distance;

        // Set the positions of the LineRenderer
        lineRenderer.SetPosition(0, startPosition); // Start point
        lineRenderer.SetPosition(1, endPosition); // End point (calculated from distance)

        Color colorWithOpacity = new Color(color.r, color.g, color.b, opacity);

        // Set the line color (optional)
        lineRenderer.startColor = colorWithOpacity;
        lineRenderer.endColor = colorWithOpacity;
    }


    /// <summary>
    /// Setup of LineRenderer with static properties including width, material and texture, and color gradient.
    /// </summary>
    private void SetUpLineRenderer()
    {
        lineRenderer.startWidth = 0.15f; // Line width at the start
        lineRenderer.endWidth = 0.00f; // Line width at the end

        //lineRenderer.material = new Material(Shader.Find("Sprites/Default")); // Simple material
        Material dashedLineMat = new Material(Shader.Find("Sprites/Default"));
        dashedLineMat.mainTexture = dashedTexture;
        lineRenderer.material = dashedLineMat;
        lineRenderer.textureMode = LineTextureMode.Tile;
        lineRenderer.alignment = LineAlignment.View;

        Gradient gradient = new Gradient();
        gradient.SetKeys(
            new GradientColorKey[] {
                new GradientColorKey(Color.white, 0.0f),
                new GradientColorKey(Color.white, 1.0f)
            },
            new GradientAlphaKey[] {
                new GradientAlphaKey(0.4f, 0.0f), // more visible at start
                new GradientAlphaKey(0.0f, 1.0f)  // fade to transparent
            }
        );
        lineRenderer.colorGradient = gradient;
    }
}