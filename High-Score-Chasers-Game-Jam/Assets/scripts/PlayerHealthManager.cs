using UnityEngine;

public class PlayerHealthManager : MonoBehaviour
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

        Debug.Log("Player Health : " + health);

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
