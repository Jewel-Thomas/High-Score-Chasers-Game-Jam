using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CarHealth : MonoBehaviour
{
    public float maxHealth = 100;
    private float currentHealth;
    [SerializeField] private Slider HealthBar;

    void Start()
    {
        currentHealth = maxHealth;
        HealthBar.maxValue = maxHealth;
    }

    public void TakeDamage(float damage)
    {
        
        currentHealth -= damage;
        HealthBar.value = currentHealth;
        Debug.Log("Current Health is at: " + currentHealth);
        if(currentHealth <= 0)
        {
            Die();
        }
    }

    void Die()
    {
        Debug.Log("Game Over");
        Destroy(gameObject);

        // Disable controls
        // Spawn explosion
        // Load game over screen
    }

}
