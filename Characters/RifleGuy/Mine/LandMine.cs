using UnityEngine;

public class LandMine : MonoBehaviour
{
    public float triggerRadius = 2f;
    public float damage = 50f;
    public GameObject source;

    private void Update()
    {
        Collider[] hits = Physics.OverlapSphere(transform.position, triggerRadius);
        foreach (Collider hit in hits)
        {
            EnemyBase enemy = hit.GetComponent<EnemyBase>();
            if (enemy != null)
            {
                enemy.TakeDamage(damage, source);
                Destroy(gameObject);
                break;
            }
        }
    }
}

