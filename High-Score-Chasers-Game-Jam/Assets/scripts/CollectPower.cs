using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CollectPower : MonoBehaviour
{
    int scoreValue = 150; 
    private void OnTriggerEnter(Collider other)
    {
        if(other.CompareTag("Player"))
        {
            ScoreManager.Instance.AddScore(
                scoreValue,
                ScoreType.CollectPower
            );
            Destroy(transform.parent.gameObject);
        }
    }
}
