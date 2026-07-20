using UnityEngine;

public class EnemyAI : MonoBehaviour
{
    [SerializeField] private float detectionRadius = 2f;
    [SerializeField] private float maxDetectionDistance = 10f;
    [SerializeField] private LayerMask targetLayer;

    private Transform detectedTarget;

    private void FixedUpdate()
    {
        DetectTarget();
    }

    private void DetectTarget()
    {
        Vector3 sweepDirection = transform.forward;
        Vector3 startOrigin = transform.position + sweepDirection * 1.0f;

        if (Physics.SphereCast(startOrigin, detectionRadius, sweepDirection, out RaycastHit hit, maxDetectionDistance, targetLayer))
        {
            detectedTarget = hit.transform;
            Debug.Log($"Target spotted! {detectedTarget.name} is {hit.distance} meters ahead.");
        }
        else
        {
            detectedTarget = null;
        }
    }

    private void OnDrawGizmos()
    {
        // Draw the capsule volume inside the Unity Scene View for perfect alignment checking
        Gizmos.color = Color.cyan;
        Vector3 sweepDirection = transform.forward;
        Vector3 startCenter = transform.position + (sweepDirection * 1.0f);
        Vector3 endCenter = startCenter + (sweepDirection * maxDetectionDistance);

        // Visualizes the start sphere, end sphere, and connecting capsule hull
        Gizmos.DrawWireSphere(startCenter, detectionRadius);
        Gizmos.DrawWireSphere(endCenter, detectionRadius);
        Gizmos.DrawLine(startCenter + transform.right * detectionRadius, endCenter + transform.right * detectionRadius);
        Gizmos.DrawLine(startCenter - transform.right * detectionRadius, endCenter - transform.right * detectionRadius);
    }
}
