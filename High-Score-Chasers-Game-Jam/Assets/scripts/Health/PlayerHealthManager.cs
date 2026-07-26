using UnityEngine;
using UnityEngine.UI;

public class PlayerHealthManager : MonoBehaviour
{
    [SerializeField] private float maxHealth = 100;
    [SerializeField] private TireDecouple[] decoupleTireObjects;
    [SerializeField] private Slider healthBar;

    private float currentHealth;

    void Start()
    {
        currentHealth = maxHealth;
        UIManager.Instance.UpdateHealthSlider(currentHealth, maxHealth);
    }

    public float GetHealth()
    {
        return currentHealth;
    }

    public void SetHealth(float health)
    {
        currentHealth = health;
    }

    public void TakeDamage(float relativeSpeed)
    {
        if (currentHealth <= 0) return;
        float damage = Mathf.Abs(relativeSpeed);

        currentHealth -= damage;
        UIManager.Instance.UpdateHealthSlider(currentHealth);

        if (currentHealth <= 0)
        {
            // Car totaled logic
            TotallCar();
            GameManager.Instance.SetGameOver();
        }
    }

    private void TotallCar()
    {
        foreach (TireDecouple tire in decoupleTireObjects)
        {
            tire.Decouple(true);
        }
    }

    private void OnCollisionEnter(Collision collision)
    {
        if(collision.gameObject.CompareTag("Enemy"))
        {
            Vector3 relativeVelocity = collision.relativeVelocity;

            float relativeForwardSpeed = Vector3.Dot(relativeVelocity, transform.forward);

            if (relativeForwardSpeed > 0)
            {
                // Damage the Player
                TakeDamage(relativeForwardSpeed);
            }
            else
            {
                // Damage the Enemy
                collision.gameObject.GetComponent<EnemyHealthManager>().TakeDamage(relativeForwardSpeed);
            }
        }
    }

}
