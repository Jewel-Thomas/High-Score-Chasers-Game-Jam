using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(PolygonCollider2D))]
public class SpawnArea : MonoBehaviour
{
    [Header("Gizmo Settings")]
    public float sphereRadius = 0.5f;

    // List to store multiple local positions
    public List<Vector3> gizmoLocalPositions = new List<Vector3>();

    [SerializeField] private PolygonCollider2D polygonCollider2D;

    [Header("Spawn Setting")]
    [SerializeField] private GameObject spawnObject;

    // Area Settings
    private Vector3[] verts;
    private int[] triangleIndices;
    private int triangleCount;
    private float[] prefixSumAreas;
    private float totalArea;

    private void Awake()
    {
        polygonCollider2D = GetComponent<PolygonCollider2D>();
    }

    private void Start()
    {
        InitializeSpawnArea();
        InvokeRepeating(nameof(SpawnObject), 3f, 5f);
    }

    private void InitializeSpawnArea()
    {
        Mesh automesh = TriangulatePolygon();

        verts = gizmoLocalPositions.ToArray();
        triangleIndices = automesh.triangles;
        triangleCount = triangleIndices.Length / 3;

        float[] areas = CalculateTriangleAreas(verts, triangleIndices, triangleCount);
        totalArea = 0f;

        foreach (float area in areas)
        {
            totalArea += area;
        }

        prefixSumAreas = ConvertToPrefixSum(areas);
        totalArea = prefixSumAreas.Length > 0 ? prefixSumAreas[prefixSumAreas.Length - 1] : 0f;
        
#if UNITY_EDITOR
        DebugTriangles(verts, triangleIndices, triangleCount);
#endif
    }


    private Mesh TriangulatePolygon()
    {
        if (polygonCollider2D == null || gizmoLocalPositions == null || gizmoLocalPositions.Count < 3) return null;

        // polygonCollider2D stores points in an array, converting list objects to arrays ...
        Vector2[] points = new Vector2[gizmoLocalPositions.Count];

        for (int i = 0; i < gizmoLocalPositions.Count; i++)
        {
            points[i] = new Vector2(gizmoLocalPositions[i].x, gizmoLocalPositions[i].z);
        }

        polygonCollider2D.points = points;
        Mesh automesh = polygonCollider2D.CreateMesh(false, false);

        if(automesh == null || automesh.triangles.Length == 0)
        {
            Debug.LogWarning("Triangulation failed or produced no triangles.");
            return null;
        }

        return automesh;
    }

    private Vector3 GenerateRandomPointInArea()
    {
        Vector3[] chosenTriangleVerts = ChooseSpawnTriangle(totalArea, prefixSumAreas, verts, triangleIndices);
        Vector2 chosenPointFromTriangle = GetPointFromChosenTriangle(chosenTriangleVerts);

        Vector3 localRandomPoint = new Vector3(chosenPointFromTriangle.x, 0, chosenPointFromTriangle.y);
        Vector3 worldRandomPoint = transform.TransformPoint(localRandomPoint);
        Vector3 randomPointGrounded = MeshHeightChecker.Instance.PositionObjectsToGround(worldRandomPoint);

        return randomPointGrounded;
    }

    private void SpawnObject()
    {
        Vector3 spawnPosition = GenerateRandomPointInArea();
        SpawnManager.Instance.SpawnObject(spawnObject, spawnPosition);
    }

    private Vector3[] ChooseSpawnTriangle(float totalArea, float[] prefixSumAreas, Vector3[] verts, int[] triangleIndices)
    {
        float weightedRoll = Random.Range(0f, totalArea);

        int chosenTriangleAreaIndex = 0;
        float areaWeightRollDiff = Mathf.Infinity;

        for (int i = 0; i < prefixSumAreas.Length; i++)
        {
            float currentDiff = prefixSumAreas[i] - weightedRoll;
            if (currentDiff < 0) continue;

            if (areaWeightRollDiff > currentDiff)
            {
                areaWeightRollDiff = currentDiff;
                chosenTriangleAreaIndex = i;
            }
        }

        int triangleVertOffset = chosenTriangleAreaIndex * 3;

        int indexA = triangleIndices[triangleVertOffset];
        int indexB = triangleIndices[triangleVertOffset + 1];
        int indexC = triangleIndices[triangleVertOffset + 2];

        Vector3[] triangleVerts = { verts[indexA], verts[indexB], verts[indexC] };

        return triangleVerts;
    }

    private Vector2 GetPointFromChosenTriangle(Vector3[] chosenTriangleVerts)
    {
        // Raw Random roles for weighted positioning of the point
        float u1 = Random.Range(0f, 1f);
        float u2 = Random.Range(0f, 1f);

        // Square-root transformation used to ensure uniform distribution across the triangle
        float s = Mathf.Sqrt(u1);

        // Performing Barycentric weight Calculation
        float wA = 1 - s;
        float wB = s * (1 - u2);
        float wC = s * u2;

        float xRand = (wA * chosenTriangleVerts[0].x) + (wB * chosenTriangleVerts[1].x) + (wC * chosenTriangleVerts[2].x);
        float yRand = (wA * chosenTriangleVerts[0].z) + (wB * chosenTriangleVerts[1].z) + (wC * chosenTriangleVerts[2].z);

        Vector2 pointFromChosenTriangle = new Vector2(xRand, yRand);

        return pointFromChosenTriangle;
    }

    private float[] ConvertToPrefixSum(float[] array)
    {
        float[] prefixSumArray = new float[array.Length];

        float sum = 0;
        for (int i = 0; i < array.Length; i++)
        {
            sum += array[i];
            prefixSumArray[i] = sum;
        }

        return prefixSumArray;
    }

    private float[] CalculateTriangleAreas(Vector3[] verts, int[] triangleIndices, int triangleCount)
    {
        float[] areas = new float[triangleCount];

        for(int i = 0; i < triangleCount; i++)
        {
            int indexA = triangleIndices[i * 3];
            int indexB = triangleIndices[i * 3 + 1];
            int indexC = triangleIndices[i * 3 + 2];

            Vector3 vertexA = verts[indexA];
            Vector3 vertexB = verts[indexB];
            Vector3 vertexC = verts[indexC];

            float triangleArea = 0.5f * Mathf.Abs(
                                 vertexA.x * (vertexB.z - vertexC.z) +
                                 vertexB.x * (vertexC.z - vertexA.z) +
                                 vertexC.x * (vertexA.z - vertexB.z));

            areas[i] = triangleArea;
        }

        return areas;
    }

    private void DebugTriangles(Vector3[] verts, int[] triangleIndices, int triangleCount)
    {
        for(int i = 0; i < triangleCount; i++)
        {
            int indexA = triangleIndices[i * 3];
            int indexB = triangleIndices[i * 3 + 1];
            int indexC = triangleIndices[i * 3 + 2];

            Vector3 vertexA = verts[indexA];
            Vector3 vertexB = verts[indexB];
            Vector3 vertexC = verts[indexC];

            Debug.Log($"Triangle {i}: [Indices: {indexA}, {indexB}, {indexC}] -> Vertices: A({vertexA.x:F2}, {vertexA.y:F2}), B({vertexB.x:F2}, {vertexB.y:F2}), C({vertexC.x:F2}, {vertexC.y:F2})");
        }
    }

}
