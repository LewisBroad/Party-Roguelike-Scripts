using UnityEngine;

public class EnemyProjectile : MonoBehaviour
{
    public float lifetime = 5f;
    public float damage = 10f;
    public GameObject splatterPrefab;
    public LayerMask hitLayers;

    private void Start()
    {
        Destroy(gameObject, lifetime);
    }

    private void OnCollisionEnter(Collision collision)
    {
        // Check if the object can be damaged
        IDamageable damageable = collision.gameObject.GetComponent<IDamageable>();
        if (damageable != null)
        {
            damageable.TakeDamage(damage, gameObject);
        }
        else
        {
            // Spawn splatter if it's not a damageable target
            if (splatterPrefab != null)
            {
                ContactPoint contact = collision.contacts[0];
                Instantiate(splatterPrefab, contact.point, Quaternion.identity);
            }
        }

        Destroy(gameObject);
    }
}
