using UnityEngine;

[RequireComponent(typeof(MeshFilter), typeof(MeshRenderer))]
public class AngleColorizer : MonoBehaviour
{
    public float startAngle = 0f; // Startwinkel in Grad
    public float endAngle = 90f; // Endwinkel in Grad
    public float radius = 0.2f; // Radius des Kreises
    public Color color = new Color(164f / 255f, 39f / 255f, 39f / 255f, 0.5f); // Farbe des Bereichs
    public Material material; // Material mit deaktiviertem Backface Culling
    public UpperLimbAngleCalculator angleCalculator;

    void Start()
    {
        // CreateMesh(startAngle, endAngle);
    }

    void Update()
    {
        CreateShoulderVerticalMesh();
    }

    private void CreateMesh(float startAngle, float endAngle)
    {
        MeshFilter meshFilter = GetComponent<MeshFilter>();
        MeshRenderer meshRenderer = GetComponent<MeshRenderer>();

        // Erstellen des Meshes
        Mesh mesh = new Mesh();
        mesh.name = "AngleVisualizationMesh";

        // Berechnen der Anzahl der Segmente
        int segments = Mathf.CeilToInt(endAngle - startAngle);
        // print("Segments: " + segments);
        int verticesCount = segments + 2;
        // print("Vertices: " + verticesCount);
        Vector3[] vertices = new Vector3[verticesCount];
        int[] triangles = new int[segments * 3];
        // print("Triangles: " + triangles.Length);

        // Ursprungspunkt
        vertices[0] = Vector3.zero;

        // Berechnen der Eckpunkte des Bereichs
        for (int i = 0; i <= segments; i++)
        {
            float angle = Mathf.Lerp(startAngle, endAngle, (float)i / segments) * Mathf.Deg2Rad;
            vertices[i + 1] = new Vector3(Mathf.Cos(angle) * radius, Mathf.Sin(angle) * radius, 0); // Definition of Angle Direction
            // print("Vertices[" + (i + 1) + "]: " + vertices[i + 1]);
        }

        // Erstellen der Dreiecke
        for (int i = 0; i < segments; i++)
        {
            triangles[i * 3] = 0;
            triangles[i * 3 + 1] = i + 1;
            triangles[i * 3 + 2] = i + 2;
        }

        // Handle the last triangle correctly if the segment is closed -TESTESTEST
        if (endAngle - startAngle == 360 || endAngle - startAngle == -360)
        {
            triangles[segments * 3 - 1] = 1; // Last vertex wraps around to the first segment vertex
        }

        mesh.vertices = vertices;
        mesh.triangles = triangles;
        mesh.RecalculateNormals();

        meshFilter.mesh = mesh;

        // Material und Farbe festlegen
        if (material == null)
        {
            material = new Material(Shader.Find("Standard"));
        }
        material.color = color;
        meshRenderer.material = material;
    }

    private void CreateShoulderVerticalMesh()
    {
        // Berechnen des Winkels
        //float angle = angleCalculator.angleShoulderVerticalRight(angleCalculator.trackerChest, angleCalculator.trackerElbowRight);

        // Anpassen des Winkels
        // if (angle < 0)
        // {
        //     angle = 0;
        // }
        // else if (angle > 180)
        // {
        //     angle = 180;
        // }

        // Anpassen des Winkels
        // startAngle = 0;
        // endAngle = angle;

        // Erstellen des Meshes
        // CreateMesh(startAngle, endAngle);
    }
}
