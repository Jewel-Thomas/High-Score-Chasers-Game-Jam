using UnityEngine;

[RequireComponent(typeof(CarController))]
public class EnemyAI : MonoBehaviour
{
    private CarController carController;

    [SerializeField] private int directionCount = 8;
    [SerializeField] private float searchRadius = 15f;
    [SerializeField] private LayerMask targetLayer;

    private Vector3[] rayDirections;
    private float[] interestWeights;
    private Transform detectedTarget;
    private Vector3 optimalDirection;

    [SerializeField] private Color gizmoColor;
    [SerializeField] private float gizmoScale = 1.5f;

    private void Awake()
    {
        InitializeRays();
    }

    private void Start()
    {
        carController = GetComponent<CarController>();
    }

    private void FixedUpdate()
    {
        FindTarget();
        CalculateInterestWeights();
        optimalDirection = ChooseBestDirection();

        if (carController != null && carController.drivator == Drivator.AI)
        {
            if (optimalDirection == Vector3.zero)
            {
                carController.GetInput(0f, 0f);
                carController.HandBrake(true);
                return;
            }

            Vector3 localDirection = transform.InverseTransformDirection(optimalDirection);

            float gas;
            float steering;

            if (localDirection.z < 0)
            {
                gas = 1f;
                steering = localDirection.x >= 0 ? 1f : -1f;
            }
            else
            {
                // Normal forward driving behavior
                gas = localDirection.z;
                steering = localDirection.x;
            }

            // Push the corrected steering values to the drivetrain
            carController.HandBrake(false);
            carController.GetInput(gas, steering);
        }
    }

    // Initializes {directionCount} number of rays from the enemy to detect the player.
    private void InitializeRays()
    {
        rayDirections = new Vector3[directionCount];
        interestWeights = new float[directionCount];

        for(int i = 0; i < directionCount; i++)
        {
            float angle = i * 2 * Mathf.PI / directionCount;
            rayDirections[i] = new Vector3(Mathf.Sin(angle), 0, Mathf.Cos(angle)).normalized;
        }
    }

    private void FindTarget()
    {
        Collider[] targets = Physics.OverlapSphere(transform.position, searchRadius, targetLayer);
        if (targets.Length > 0) detectedTarget = targets[0].transform;
        else detectedTarget = null;
    }

    private void CalculateInterestWeights()
    {
        if (!detectedTarget) return;
        Vector3 enemyToTargetDirection = (detectedTarget.position - transform.position).normalized;

        for(int i = 0; i < directionCount; i++)
        {
            // Convert Local raydirection to world space direction
            Vector3 worldSpaceRayDirection = transform.TransformDirection(rayDirections[i]);
            float targetDot = Vector3.Dot(worldSpaceRayDirection, enemyToTargetDirection);

            interestWeights[i] = Mathf.Max(0, targetDot);
        }
    }

    private Vector3 ChooseBestDirection()
    {
        if (!detectedTarget) return Vector3.zero;

        Vector3 outputDirection = Vector3.zero;

        for(int i = 0; i < directionCount; i++)
        {
            Vector3 worldSpaceRayDirection = transform.TransformDirection(rayDirections[i]);
            outputDirection += worldSpaceRayDirection * interestWeights[i];
        }

        return outputDirection.normalized;
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = gizmoColor;

        if (rayDirections == null || rayDirections.Length != directionCount)
        {
            return;
        }

        for (int i = 0; i < directionCount; i++)
        {
            Vector3 worldSpaceRayDirection = transform.TransformDirection(rayDirections[i]);
            Gizmos.DrawRay(transform.position + Vector3.up * 1.1f, worldSpaceRayDirection * gizmoScale);
        }
    }
}
