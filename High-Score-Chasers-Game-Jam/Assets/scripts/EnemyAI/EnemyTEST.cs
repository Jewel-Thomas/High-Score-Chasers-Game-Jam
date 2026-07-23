using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class EnemyTEST : MonoBehaviour
{
    int scoreValue = 100; 
    
    private Coroutine AICoroutine;

    public enum EnemyState
    {
        Idle,
        Chase,
        Shoot,
        Retreat
    }  

    public enum AnimationState
    {
        Idle = 0,
        Run = 1,
        Shoot = 2
    }
    
    EnemyState CurrentState = EnemyState.Idle;

    [SerializeField] private Transform player;
    [SerializeField] private Animator animator;
    
    private NavMeshAgent agent;

    [SerializeField] private RagDollController R1;

    [SerializeField] float fireRate = 1.5f;

    private float nextFireTime;
    [SerializeField] private CowboyGun Gun;

    [Header("Ranges")]

    [SerializeField] private float detectionRange = 25f;

    [SerializeField] private float shootingRange = 12f;

    [SerializeField] private float retreatRange = 5f;

    private void Awake()
    {
        agent = GetComponent<NavMeshAgent>();
    }
    
    void Start()
    {
        //agent.updateRotation = false;
    }

    void RunStateMachine()
    {
        float distance = Vector3.Distance(transform.position, player.position);
        switch (CurrentState)
        {
            case EnemyState.Idle:
                IdleState(distance);
                break;

            case EnemyState.Chase:
                ChaseState(distance);
                break;

            case EnemyState.Shoot:
                ShootState(distance);
                break;

            case EnemyState.Retreat:
                RetreatState(distance);
                break;
        }
    }

    void UpdateAnimation(EnemyState state)
    {
        AnimationState animState;

        switch(state)
        {
            case EnemyState.Idle:
                animState = AnimationState.Idle;
                break;

            case EnemyState.Chase:
            case EnemyState.Retreat:
                animState = AnimationState.Run;
                break;

            case EnemyState.Shoot:
                animState = AnimationState.Shoot;
                break;

            default:
                animState = AnimationState.Idle;
                break;
        }

        animator.SetInteger(
            "AnimationState",
            (int)animState
        );
        Debug.Log("Animation has been updated to :" + animState);
    }

    IEnumerator AIThink()
    {
        while(true)
        {
            RunStateMachine();

            yield return new WaitForSeconds(0.2f);
        }
    }
    
    // Player Entering detection range

    void OnTriggerEnter(Collider other)
    {
        if(!other.CompareTag("Player"))
        {
            return;
        }
        else if(AICoroutine == null)
        {
            AICoroutine = StartCoroutine(AIThink());
        }
    }
    // Player Leaves detection range
    void OnTriggerExit(Collider other)
    {
        if(!other.CompareTag("Player"))
        {
            return;
        }
        else if(AICoroutine != null)
        {
            StopCoroutine(AICoroutine);
            AICoroutine = null;
        }
        ChangeState(EnemyState.Idle);
    }

    //Collision with player

    private void Death()
    {
        if (AICoroutine != null)
        {
            StopCoroutine(AICoroutine);
            AICoroutine = null;
        }

        agent.enabled = false;
       

    }

    private void OnCollisionEnter(Collision collision)
    {
        if(collision.gameObject.CompareTag("Player") && R1.CheckAnimator())
        {
            Vector3 impactV = collision.relativeVelocity;
            //Debug.Log("Impact Speed: " + impactSpeed);

            ScoreManager.Instance.AddScore(
                scoreValue,
                ScoreType.EnemyHit
            );
            Death();
            R1.EnableRagdoll(impactV);
            //Debug.Log("Collison successfully detected");


            Destroy(gameObject,5f);
        }
    }

    //States Code

    void IdleState(float distance)
    {
        if (!agent.enabled)
        {
            return;
        }
        agent.isStopped = true;

        if (distance <= detectionRange)
        {
            ChangeState(EnemyState.Chase);
        }
    }

    void ChaseState(float distance)
    {
        if (!agent.enabled)
        {
            return;
        }

        agent.isStopped = false;
        agent.SetDestination(player.position);

        if (distance <= retreatRange)
        {
            ChangeState(EnemyState.Retreat);
            return;
        }

        if (distance <= shootingRange)
        {
            ChangeState(EnemyState.Shoot);
            return;
        }

        if (distance > detectionRange)
        {
            ChangeState(EnemyState.Idle);
        }
    }

    void ShootState(float distance)
    {
        if (!agent.enabled)
        {
            return;
        }
        agent.isStopped = true;

        // Face the player
        Vector3 lookPos = player.position - transform.position;
        lookPos.y = 0;

        transform.rotation = Quaternion.Slerp(
            transform.rotation,
            Quaternion.LookRotation(lookPos),
            Time.deltaTime * 6f);

        if (Time.time >= nextFireTime)
        {
            Debug.Log("Gun Fired");
            Gun.Fire(player.position);

            nextFireTime = Time.time + fireRate;
        }

        if (distance <= retreatRange)
        {
            ChangeState(EnemyState.Retreat);
            return;
        }

        if (distance > shootingRange)
        {
            ChangeState(EnemyState.Chase);
            return;
        }

        if (distance > detectionRange)
        {
            ChangeState(EnemyState.Idle);
        }
    }


    void RetreatState(float distance)
    {
        if (!agent.enabled)
        {
            return;
        }

        agent.isStopped = false;

        Vector3 direction = (transform.position - player.position).normalized;

        Vector3 retreatPosition =
            transform.position + direction * 5f;

        agent.SetDestination(retreatPosition);

        if (distance >= shootingRange)
        {
            ChangeState(EnemyState.Shoot);
            return;
        }

        if (distance > detectionRange)
        {
            ChangeState(EnemyState.Idle);
        }
    }

    void ChangeState( EnemyState newState)
    {
        if (CurrentState == newState)
            return;

        CurrentState = newState;
        UpdateAnimation(CurrentState);
        //Debug.Log("State Changed To: " + CurrentState);
    }

}
