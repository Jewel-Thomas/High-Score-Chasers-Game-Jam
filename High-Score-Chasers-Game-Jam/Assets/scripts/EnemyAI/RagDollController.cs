using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RagDollController : MonoBehaviour
{
    private Rigidbody[] bodies;
    private Collider[] colliders;

    [SerializeField] private Animator animator;

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

    public void EnableRagdoll()
    {
        animator.enabled = false;

        foreach (Rigidbody rb in bodies)
        {
            rb.isKinematic = false;
        }

        foreach (Collider col in colliders)
        {
            if (col.gameObject != gameObject) 
                col.enabled = true;
        }
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
