using UnityEngine;
using UnityEngine.UI;

public class EnemyHealthManager : MonoBehaviour
{
    [SerializeField] private float maxHealth = 100;
    [SerializeField] private TireDecouple[] decoupleTireObjects;
    [SerializeField] private Slider healthBar;

    private float currentHealth;

    void Start()
    {
        currentHealth = maxHealth;
        healthBar.maxValue = maxHealth;
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
        if (currentHealth < 0) return;
        float damage = Mathf.Abs(relativeSpeed);

        currentHealth -= damage;
        healthBar.value = currentHealth;

        if (currentHealth < 0)
        {
            // Car totaled logic
            TotallCar();
        }
    }

    private void TotallCar()
    {
        foreach (TireDecouple tire in decoupleTireObjects)
        {
            tire.Decouple(true);
            Destroy(tire.GetDecoupleTire(), 10f);
        }

        Destroy(gameObject, 15);
    }
}
