using UnityEngine;

[RequireComponent(typeof(PolygonCollider2D))]
public class PolygonTriangleSpawner : MonoBehaviour
{
    [Header("Materials")]
    [SerializeField] private Material triangleMaterial;
    [SerializeField] private Material fullPolygonMaterial;

    [Header("Offsets")]
    [Tooltip("Base height offset for individual triangle meshes")]
    [SerializeField] private float heightOffset = 0.05f;

    [Tooltip("Small additional offset to render the full polygon slightly above the triangles")]
    [SerializeField] private float polygonMeshOffset = 0.02f;

    [ContextMenu("1. Set Hexagon Points")]
    public void SetHexagonPointsMenu()
    {
        PolygonCollider2D poly = GetComponent<PolygonCollider2D>();
        if (poly == null) return;

        // Calculate 6 regular hexagon vertices
        Vector2[] hexPoints = new Vector2[6];
        float radius = 5f;
        for (int i = 0; i < 6; i++)
        {
            float angle = i * 60f * Mathf.Deg2Rad;
            hexPoints[i] = new Vector2(Mathf.Cos(angle) * radius, Mathf.Sin(angle) * radius);
        }

        poly.points = hexPoints;
        poly.offset = Vector2.zero; // Reset collider offset to zero
        Debug.Log("PolygonCollider2D points set to a regular Hexagon!");
    }

    [ContextMenu("2. Generate Children")]
    public void GenerateChildren()
    {
        ClearGeneratedChildren();

        PolygonCollider2D poly = GetComponent<PolygonCollider2D>();
        if (poly == null || poly.points == null || poly.points.Length < 3) return;

        // 1. Ask Unity to triangulate the 2D polygon
        Mesh autoMesh = poly.CreateMesh(false, false);
        if (autoMesh == null || autoMesh.triangles.Length == 0) return;

        Vector3[] meshVerts2D = autoMesh.vertices; // Actual vertices returned by Unity's triangulator
        int[] triangles = autoMesh.triangles;
        Vector2 offset = poly.offset;

        // Default materials fallback
        if (triangleMaterial == null) triangleMaterial = new Material(Shader.Find("Sprites/Default"));
        if (fullPolygonMaterial == null) fullPolygonMaterial = new Material(Shader.Find("Sprites/Default"));

        // -------------------------------------------------------------
        // 1. CREATE INDIVIDUAL TRIANGLE CHILD GAMEOBJECTS
        // -------------------------------------------------------------
        int triangleCount = triangles.Length / 3;
        for (int i = 0; i < triangleCount; i++)
        {
            // Direct index lookup from Unity's triangulated mesh vertices
            Vector3 vA = meshVerts2D[triangles[i * 3]];
            Vector3 vB = meshVerts2D[triangles[i * 3 + 1]];
            Vector3 vC = meshVerts2D[triangles[i * 3 + 2]];

            Vector3[] triVertices = new Vector3[]
            {
                new Vector3(vA.x + offset.x, heightOffset, vA.y + offset.y),
                new Vector3(vB.x + offset.x, heightOffset, vB.y + offset.y),
                new Vector3(vC.x + offset.x, heightOffset, vC.y + offset.y)
            };

            int[] triIndices = new int[] { 0, 1, 2, 2, 1, 0 }; // Double-sided

            Mesh triMesh = new Mesh
            {
                name = $"TriangleMesh_{i}",
                vertices = triVertices,
                triangles = triIndices
            };
            triMesh.RecalculateNormals();
            triMesh.RecalculateBounds();

            GameObject childTri = new GameObject($"Triangle_{i}");
            childTri.transform.SetParent(transform, false);

            childTri.AddComponent<MeshFilter>().sharedMesh = triMesh;
            childTri.AddComponent<MeshRenderer>().sharedMaterial = triangleMaterial;
        }

        // -------------------------------------------------------------
        // 2. CREATE FULL COMBINED POLYGON CHILD GAMEOBJECT
        // -------------------------------------------------------------
        Vector3[] fullVertices = new Vector3[meshVerts2D.Length];
        float fullPolyHeight = heightOffset + polygonMeshOffset;

        for (int i = 0; i < meshVerts2D.Length; i++)
        {
            fullVertices[i] = new Vector3(meshVerts2D[i].x + offset.x, fullPolyHeight, meshVerts2D[i].y + offset.y);
        }

        // Double-sided triangle face mapping
        int[] doubleSidedTriangles = new int[triangles.Length * 2];
        for (int i = 0; i < triangles.Length; i += 3)
        {
            // Top face
            doubleSidedTriangles[i] = triangles[i];
            doubleSidedTriangles[i + 1] = triangles[i + 1];
            doubleSidedTriangles[i + 2] = triangles[i + 2];

            // Bottom face
            doubleSidedTriangles[triangles.Length + i] = triangles[i + 2];
            doubleSidedTriangles[triangles.Length + i + 1] = triangles[i + 1];
            doubleSidedTriangles[triangles.Length + i + 2] = triangles[i];
        }

        Mesh fullMesh = new Mesh
        {
            name = "FullPolygonMesh",
            vertices = fullVertices,
            triangles = doubleSidedTriangles
        };
        fullMesh.RecalculateNormals();
        fullMesh.RecalculateBounds();

        GameObject fullPolyObj = new GameObject("Full_Polygon");
        fullPolyObj.transform.SetParent(transform, false);

        fullPolyObj.AddComponent<MeshFilter>().sharedMesh = fullMesh;
        fullPolyObj.AddComponent<MeshRenderer>().sharedMaterial = fullPolygonMaterial;

        Debug.Log($"Generated {triangleCount} triangles and 1 Full Polygon child under {gameObject.name}.");
    }

    [ContextMenu("Clear Children")]
    public void ClearGeneratedChildren()
    {
        int childCount = transform.childCount;
        for (int i = childCount - 1; i >= 0; i--)
        {
            Transform child = transform.GetChild(i);
            if (child.name.StartsWith("Triangle_") || child.name == "Full_Polygon")
            {
                if (Application.isPlaying) Destroy(child.gameObject);
                else DestroyImmediate(child.gameObject);
            }
        }
    }
}