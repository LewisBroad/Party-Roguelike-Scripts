using UnityEngine;
using System.Collections;

public class LandMine : MonoBehaviour
{
    public float triggerRadius = 2f;
    public float damage = 50f;
    public GameObject source;
    public GameObject explosionEffect;
    public float armingDelay = 0.5f;
    public LayerMask groundMask;

    private bool armed = false;
    private bool stuck = false;
    private bool exploded = false;
    private Rigidbody rb;

    private void Start()
    {
        rb = GetComponent<Rigidbody>();
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (stuck) return;

        if (((1 << collision.gameObject.layer) & groundMask) != 0)
        {
            ContactPoint contact = collision.contacts[0];
            StickToSurface(contact.point, contact.normal);
        }
    }

    private void StickToSurface(Vector3 position, Vector3 normal)
    {
        stuck = true;

        if (rb != null)
        {
            rb.linearVelocity = Vector3.zero;
            rb.angularVelocity = Vector3.zero;
            rb.isKinematic = true;
        }

        transform.position = position + normal * 0.01f;
        transform.rotation = Quaternion.LookRotation(Vector3.ProjectOnPlane(transform.forward, normal), normal);

        StartCoroutine(ArmAfterDelay());
    }

    private IEnumerator ArmAfterDelay()
    {
        yield return new WaitForSeconds(armingDelay);
        armed = true;
    }

    private void Update()
    {
        if (!armed || exploded) return;

        Collider[] hits = Physics.OverlapSphere(transform.position, triggerRadius);
        bool anyHit = false;

        foreach (Collider hit in hits)
        {
            EnemyBase enemy = hit.GetComponent<EnemyBase>();
            if (enemy != null)
            {
                enemy.TakeDamage(damage, source);
                anyHit = true;
            }
        }

        if (anyHit)
        {
            exploded = true;

            if (explosionEffect)
                Instantiate(explosionEffect, transform.position, Quaternion.identity);

            Destroy(gameObject);
        }
    }
}
