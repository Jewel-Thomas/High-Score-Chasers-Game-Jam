using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class SpeedTicket : MonoBehaviour
{
    int scoreValue = 200; 
    [SerializeField] private float speedThreshold = 11f;
    [SerializeField] private TMP_Text SpeedWarning;

    void Awake(){

        SpeedWarning.text = speedThreshold + "m/s";
    }

    private void OnTriggerEnter(Collider other)
    {
        if (!other.CompareTag("Player"))
        {
            return;
        }
            
        Rigidbody rb = other.attachedRigidbody;

        if (rb == null)
        {
            return;
        }
            
        float speed = rb.velocity.magnitude; 

        if (speed >= speedThreshold)
        {
            ScoreManager.Instance.AddScore(
                scoreValue,
                ScoreType.SpeedTicket
            );

            Destroy(transform.parent.gameObject);
        }
    }
}
