using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class EnemyTEST : MonoBehaviour
{
    int scoreValue = 100;    
    
    [SerializeField] private Transform player;

    private NavMeshAgent agent;

    [SerializeField] private RagDollController R1;

    private void Awake()
    {
        agent = GetComponent<NavMeshAgent>();
    }
    
    private void Update()
    {
        if(player != null)
        {
            agent.SetDestination(player.position);
        }
    }

    private void OnCollisionEnter(Collision collision)
    {
        if(collision.gameObject.CompareTag("Player") && R1.CheckAnimator())
        {
            ScoreManager.Instance.AddScore(
                scoreValue,
                ScoreType.EnemyHit
            );

            R1.EnableRagdoll();
            //Debug.Log("Collison successfully detected");


            Destroy(gameObject,5f);
        }
    }
}
