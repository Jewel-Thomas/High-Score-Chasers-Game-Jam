using UnityEngine;

public class EnemyHealthManager : MonoBehaviour
{
    [SerializeField] private float health = 100;
    [SerializeField] private TireDecouple[] decoupleTireObjects;

    public float GetHealth()
    {
        return health;
    }

    public void SetHealth(float _health)
    {
        health = _health;
    }

    public void TakeDamage(float relativeSpeed)
    {
        if (health < 0) return;
        float damage = Mathf.Abs(relativeSpeed);

        health -= damage;

        Debug.Log("Enemy Health : " + health);

        if (health < 0)
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
    }
}
