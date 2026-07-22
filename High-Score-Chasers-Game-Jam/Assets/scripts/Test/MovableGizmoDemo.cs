using System.Collections.Generic;
using UnityEngine;

public class MovableGizmoDemo : MonoBehaviour
{
    [Header("Gizmo Settings")]
    public float sphereRadius = 0.5f;

    // List to store multiple local positions
    public List<Vector3> gizmoLocalPositions = new List<Vector3>();


}
