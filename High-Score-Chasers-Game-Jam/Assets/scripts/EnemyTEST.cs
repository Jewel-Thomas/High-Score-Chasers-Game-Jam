using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyTEST : MonoBehaviour
{
    int scoreValue = 100;

       private void OnCollisionEnter(Collision collision)
    {
        if(collision.gameObject.CompareTag("Player"))
        {
            ScoreManager.Instance.AddScore(
                scoreValue,
                ScoreType.EnemyHit
            );
            //Debug.Log("Collison successfully detected");


            Destroy(gameObject);
        }
    }
}
