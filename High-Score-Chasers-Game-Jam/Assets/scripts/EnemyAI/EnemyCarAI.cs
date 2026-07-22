using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(CarController))]
public class EnemyCarAI : MonoBehaviour
{
    private CarController carController;

    [SerializeField] private int directionCount = 8;
    [SerializeField] private float searchRadius = 15f;
    [SerializeField] private LayerMask targetLayer;
    [SerializeField] private LayerMask dangerLayer;

    private Vector3[] rayDirections;
    private Vector3 optimalDirection;
    private float[] interestWeights;
    private float[] dangerWeights;
    private Transform detectedTarget;
    private List<Transform> detectedDangerTargets = new List<Transform>();

    [SerializeField] private float raySphereRadius = 0.5f;
    [SerializeField] private float dangerWeightMultiplier = 2.0f;
    [SerializeField] private float dangerRayDistance = 8f;

    [SerializeField] private Color gizmoColor;
    [SerializeField] private float gizmoScale = 1.5f;

    [SerializeField] private float minSpeedThreshold = 0.5f;
    [SerializeField] private float timeBeforeReverse = 1.2f;
    [SerializeField] private float reverseDuration = 1.0f;

    // State variables
    private float stuckTimer = 0f;
    private float recoveryTimer = 0f;
    private bool isRecovering = false;
    private float chosenReverseSteer = 1f;

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
        FindDanger();
        CalculateInterestWeights();
        CalculateDangerWeights();
        optimalDirection = ChooseBestDirection();

        if (carController != null && carController.drivator == Drivator.AI)
        {
            float currentSpeed = carController.GetComponent<Rigidbody>().velocity.magnitude;

            if (!isRecovering)
            {
                if (detectedTarget != null && currentSpeed < minSpeedThreshold)
                {
                    stuckTimer += Time.fixedDeltaTime;
                    if (stuckTimer >= timeBeforeReverse)
                    {
                        isRecovering = true;
                        recoveryTimer = reverseDuration;
                        stuckTimer = 0f;

                        chosenReverseSteer = GetBestReverseSteerDirection();
                    }
                }
                else
                {
                    stuckTimer = 0f;
                }
            }

            if (isRecovering)
            {
                recoveryTimer -= Time.fixedDeltaTime;
                if (recoveryTimer <= 0f)
                {
                    isRecovering = false;
                }

                carController.HandBrake(false);
                carController.GetInput(-1f, chosenReverseSteer);
                return;
            }

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
                gas = 0.5f;
                steering = localDirection.x >= 0 ? 1f : -1f;
            }
            else
            {
                gas = localDirection.z;
                steering = localDirection.x;
            }

            carController.HandBrake(false);
            carController.GetInput(gas, steering);
        }
    }

    // Initializes {directionCount} number of rays from the enemy to detect the player.
    private void InitializeRays()
    {
        rayDirections = new Vector3[directionCount];
        interestWeights = new float[directionCount];
        dangerWeights = new float[directionCount];

        for(int i = 0; i < directionCount; i++)
        {
            float angle = i * 2 * Mathf.PI / directionCount;
            rayDirections[i] = new Vector3(Mathf.Sin(angle), 0, Mathf.Cos(angle)).normalized;
        }
    }

    private float GetBestReverseSteerDirection()
    {
        float leftDanger = 0f;
        float rightDanger = 0f;

        for (int i = 0; i < directionCount; i++)
        {
            if (rayDirections[i].x < 0) leftDanger += dangerWeights[i];
            else if (rayDirections[i].x > 0) rightDanger += dangerWeights[i];
        }

        return leftDanger > rightDanger ? 1f : -1f;
    }

    private void FindTarget()
    {
        Collider[] targets = Physics.OverlapSphere(transform.position, searchRadius, targetLayer);
        if (targets.Length > 0) detectedTarget = targets[0].transform;
        else detectedTarget = null;
    }

    private void FindDanger()
    {
        detectedDangerTargets.Clear();
        Collider[] dangers = Physics.OverlapSphere(transform.position, searchRadius, dangerLayer);
        if (dangers.Length > 0)
        {
            foreach (Collider danger in dangers)
            {
                detectedDangerTargets.Add(danger.transform);
            }
        }
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

    private void CalculateDangerWeights()
    {
        Array.Clear(dangerWeights, 0, dangerWeights.Length);

        for (int i = 0; i < directionCount; i++)
        {
            Vector3 worldSpaceRayDirection = transform.TransformDirection(rayDirections[i]);
            Vector3 rayOrigin = transform.position + Vector3.up * 0.5f;

            if (Physics.SphereCast(rayOrigin, raySphereRadius, worldSpaceRayDirection, out RaycastHit hit, dangerRayDistance, dangerLayer))
            {
                if (hit.transform == transform || hit.transform.IsChildOf(transform)) continue;

                float distanceRatio = hit.distance / dangerRayDistance;
                float dangerValue = (1f - distanceRatio) * dangerWeightMultiplier;

                dangerWeights[i] = Mathf.Clamp01(dangerValue);
            }
        }
    }

    private Vector3 ChooseBestDirection()
    {
        if (!detectedTarget) return Vector3.zero;

        Vector3 outputDirection = Vector3.zero;

        for(int i = 0; i < directionCount; i++)
        {
            Vector3 worldSpaceRayDirection = transform.TransformDirection(rayDirections[i]);
            float resultantWeight = Mathf.Max(0, interestWeights[i] - dangerWeights[i]);
            outputDirection += worldSpaceRayDirection * resultantWeight;
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
