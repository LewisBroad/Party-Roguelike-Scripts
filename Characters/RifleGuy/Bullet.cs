using Unity.Burst.CompilerServices;
using UnityEngine;

public class Bullet : MonoBehaviour
{
    private int damage;
    public float lifeTime = 5f;
    public GameObject hitEffect;
    public LayerMask hitMask;
    private Rigidbody rb;


    public void SetDamage(int dmg)
    {
        damage = dmg;
    }

    private void Start()
    {
        rb = GetComponent<Rigidbody>();
        if (rb != null)
        {
            rb.collisionDetectionMode = CollisionDetectionMode.Continuous;
        }
        Destroy(transform.root.gameObject, lifeTime);
    }
    private void FixedUpdate()
    {
        float distance = rb.linearVelocity.magnitude * Time.fixedDeltaTime;
        if (Physics.Raycast(transform.position, rb.linearVelocity.normalized, out RaycastHit hit, distance, hitMask))
        {
            HandleHit(hit.collider, hit.point, hit.normal);
        }
    }

    private void OnCollisionEnter(Collision collision)
    {
        ContactPoint contact = collision.contacts[0];
        HandleHit(collision.collider, contact.point, contact.normal);

        /*        if (collision.gameObject.CompareTag("Player"))
                    return;
                //if (((1 << collision.gameObject.layer) & hitMask) != 0)
                //{
                //Debug.Log("Bullet Damage = " + damage);
                *//*if (collision.collider.TryGetComponent(out IDamageable dmg))
                        dmg.TakeDamage(damage, gameObject); // Pass the bullet as the source of damage
                    else if (collision.collider.TryGetComponent(out Rigidbody rb))
                        rb.AddForce(-collision.contacts[0].normal * 10f, ForceMode.Impulse); // Apply force to rigidbody
                    *//*  else if (collision.collider.TryGetComponent(out HealthComponent health))
                          health.TakeDamage(damage); // Apply damage to health component*//*
                    else
                        Debug.Log("Hit something that is not damageable: " + collision.gameObject.name);*//*

                    if (hitEffect != null)
                    {
                        //Debug.Log("Hit effect instantiated");
                        ContactPoint contact = collision.contacts[0];
                        Quaternion rot = Quaternion.LookRotation(contact.normal);
                        GameObject impact = Instantiate(hitEffect, contact.point, rot);
                        impact.transform.localScale = Vector3.one / 4;
                        Destroy(impact, 1f);
                    }
                    if(collision.transform.TryGetComponent(out EnemyBase enemy))
                {
                    //Debug.Log("Hit enemy: " + collision.gameObject.name);
                    // Apply knockback or any other effect here
                    enemy.TakeDamage(damage, gameObject);
                    enemy.ApplySlow(0.2f, 0.5f);
                }
                //}

                Destroy(transform.root.gameObject);*/


        // Deal damage if the hit object has health
        //var health = collision.gameObject.GetComponent<HealthComponent>();
        /*   if (health)
           {
               health.TakeDamage(damage);
           }*/

    }
    private void HandleHit(Collider other, Vector3 point, Vector3 normal)
    {
        if (other.CompareTag("Player"))
            return;

        if (hitEffect != null)
        {
            Quaternion rot = Quaternion.LookRotation(normal);
            GameObject impact = Instantiate(hitEffect, point, rot);
            impact.transform.localScale = Vector3.one / 4;
            Destroy(impact, 1f);
        }

        if (other.TryGetComponent(out EnemyBase enemy))
        {
            enemy.TakeDamage(damage, gameObject);
            enemy.ApplySlow(0.2f, 0.5f);
        }

        Destroy(transform.root.gameObject);
    }

    public void SetHitMask(LayerMask mask)
    {
        hitMask = mask;
    }
}
