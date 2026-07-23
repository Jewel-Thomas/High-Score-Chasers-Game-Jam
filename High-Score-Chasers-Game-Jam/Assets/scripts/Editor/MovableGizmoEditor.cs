using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(SpawnArea))]
public class MovableGizmoEditor : Editor
{
    private LayerMask GroundMask => LayerMask.GetMask("Ground");

    private void OnSceneGUI()
    {
        SpawnArea demo = (SpawnArea)target;
        Event currentEvent = Event.current;

        bool isModifierDown = currentEvent.control || currentEvent.command;

        if (isModifierDown && currentEvent.type == EventType.MouseDown && currentEvent.button == 0)
        {
            Ray ray = HandleUtility.GUIPointToWorldRay(currentEvent.mousePosition);

            if (Physics.Raycast(ray, out RaycastHit hit, Mathf.Infinity, GroundMask))
            {
                Undo.RecordObject(demo, "Add Gizmo Sphere");

                Vector3 groundedWorldPos = PositionShereToGround(hit.point);
                Vector3 localHitPoint = demo.transform.InverseTransformPoint(groundedWorldPos);
                demo.gizmoLocalPositions.Add(localHitPoint);

                currentEvent.Use();
            }
        }

        if (demo.gizmoLocalPositions.Count > 1)
        {
            Handles.color = Color.yellow;

            for (int i = 0; i < demo.gizmoLocalPositions.Count - 1; i++)
            {
                Vector3 startWorld = demo.transform.TransformPoint(demo.gizmoLocalPositions[i]);
                Vector3 endWorld = demo.transform.TransformPoint(demo.gizmoLocalPositions[i + 1]);

                // Draw connecting line in 3D scene space
                Handles.DrawLine(startWorld, endWorld, 2.0f);
            }

            Vector3 start = demo.transform.TransformPoint(demo.gizmoLocalPositions[demo.gizmoLocalPositions.Count - 1]);
            Vector3 end = demo.transform.TransformPoint(demo.gizmoLocalPositions[0]);
            Handles.DrawLine(start, end, 2.0f);
        }

        Handles.color = Color.cyan; // Sphere color

        for (int i = 0; i < demo.gizmoLocalPositions.Count; i++)
        {
            Vector3 worldPos = demo.transform.TransformPoint(demo.gizmoLocalPositions[i]);
            EditorGUI.BeginChangeCheck();

            Vector3 newWorldPos = Handles.FreeMoveHandle(
                worldPos,
                demo.sphereRadius,
                Vector3.zero,
                Handles.SphereHandleCap
            );

            if (EditorGUI.EndChangeCheck())
            {
                Undo.RecordObject(demo, "Move Gizmo Sphere");
                Vector3 groundedWorldPos = PositionShereToGround(newWorldPos);
                demo.gizmoLocalPositions[i] = demo.transform.InverseTransformPoint(groundedWorldPos);
            }
        }
    }

    private Vector3 PositionShereToGround(Vector3 rayStartPos)
    {
        Vector3 highStart = new Vector3(rayStartPos.x, rayStartPos.y + 50f, rayStartPos.z);

        if (Physics.Raycast(highStart, Vector3.down, out RaycastHit hit, Mathf.Infinity, GroundMask))
        {
            return new Vector3(rayStartPos.x, hit.point.y, rayStartPos.z);
        }

        return rayStartPos;
    }
}