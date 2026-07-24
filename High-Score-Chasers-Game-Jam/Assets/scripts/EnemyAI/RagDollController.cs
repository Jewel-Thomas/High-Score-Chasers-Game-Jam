using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RagDollController : MonoBehaviour
{
    [SerializeField] private Rigidbody hipsRb;
    [SerializeField] private Rigidbody MainRb;
    private Rigidbody[] bodies;
    private Collider[] colliders;

    [SerializeField] private Animator animator;
    [SerializeField] private Collider MainCol;


    private void Awake()
    {
        bodies = GetComponentsInChildren<Rigidbody>();
        colliders = GetComponentsInChildren<Collider>();

        DisableRagdoll();
    }

    public bool CheckAnimator()
    {
        return animator.enabled;
    }

    public void EnableRagdoll(Vector3 impactVelocity)
    {
        animator.enabled = false;
        MainRb.isKinematic = true;
        MainCol.enabled = false;
        

        foreach (Rigidbody rb in bodies)
        {
            rb.isKinematic = false;
        }

        foreach (Collider col in colliders)
        {
            if (col.gameObject != gameObject) 
                col.enabled = true;
        }

        float force = Mathf.Clamp(impactVelocity.magnitude * 3f,
            15f,
            100f
        );

        Vector3 direction = impactVelocity.normalized;
        direction.y += 0.25f;
        direction.Normalize();
        hipsRb.AddForce(direction * force, ForceMode.Impulse);
    }

    public void DisableRagdoll()
    {
        animator.enabled = true;

        foreach (Rigidbody rb in bodies)
        {
            rb.isKinematic = true;
        }

        foreach (Collider col in colliders)
        {
            if (col.gameObject != gameObject)
                col.enabled = false;
        }
    }
}
