using UnityEngine;

public class Bullet : MonoBehaviour
{
    [SerializeField] private float speed = 20f;
    [SerializeField] private float lifetime = 5f;
    [SerializeField] private int damage = 10;

    private Rigidbody rb;

    private void Awake()
    {
        rb = GetComponent<Rigidbody>();
    }

    public void Fire(Vector3 direction)
    {
        rb.velocity = direction.normalized * speed;

        Destroy(gameObject, lifetime);
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.CompareTag("Player"))
        {
            PlayerHealthManager playerHealthManager = collision.gameObject.GetComponent<PlayerHealthManager>();

            if (playerHealthManager != null)
            {
                playerHealthManager.TakeDamage(damage);
            }
        }

        Destroy(gameObject);
    }
}
