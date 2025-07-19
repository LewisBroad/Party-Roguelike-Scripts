using UnityEngine;
using System.Collections;

public class AirstrikeBeacon : MonoBehaviour
{
    public GameObject explosionPrefab;
    public GameObject projectilePrefab;
    public GameObject strikeDecalPrefab;

    public float delayBeforeStrike = 1.5f;
    public float strikeRadius = 8f;
    public double damage = 100f;
    public int numberOfStrikes = 5;
    public float timeBetweenStrikes = 0.3f;
    public float projectileHeight = 30f;

    private GameObject player;
    private bool strikeStarted = false;
    private Rigidbody rb;

    public float minVelocityToTrigger = 0.2f;
    public float maxWaitBeforeStrike = 5f;

    public void Initialize(GameObject owner)
    {
        player = owner;
        rb = GetComponent<Rigidbody>();
        StartCoroutine(CheckForRest());
        StartCoroutine(TimeoutFallback());
    }

    private IEnumerator CheckForRest()
    {
        while (!strikeStarted)
        {
            if (rb != null && rb.linearVelocity.magnitude < minVelocityToTrigger)
            {
                StartCoroutine(DelayedStrike());
                break;
            }
            yield return new WaitForSeconds(0.1f);
        }
    }

    private IEnumerator TimeoutFallback()
    {
        yield return new WaitForSeconds(maxWaitBeforeStrike);
        if (!strikeStarted)
        {
            Debug.Log("Airstrike beacon timeout fallback triggered.");
            StartCoroutine(DelayedStrike());
        }
    }

    private IEnumerator DelayedStrike()
    {
        strikeStarted = true;

        if (rb != null)
        {
            rb.linearVelocity = Vector3.zero;
            rb.isKinematic = true;
        }

        Vector3 groundPos = GetGroundPosition(transform.position);

        GameObject decal = null;
        if (Physics.Raycast(transform.position + Vector3.up * 10f, Vector3.down, out RaycastHit hit, 50f))
        {
            decal = Instantiate(strikeDecalPrefab, hit.point + hit.normal * 0.01f, Quaternion.LookRotation(hit.normal));
            decal.transform.localScale = Vector3.one * strikeRadius * 2f;
        }

        yield return new WaitForSeconds(delayBeforeStrike);

        for (int i = 0; i < numberOfStrikes; i++)
        {
            Vector2 offset = Random.insideUnitCircle * strikeRadius;
            Vector3 strikePos = groundPos + new Vector3(offset.x, 0, offset.y);

            // Spawn projectile
            if (projectilePrefab)
            {
                Vector3 startPos = strikePos + Vector3.up * projectileHeight;
                GameObject proj = Instantiate(projectilePrefab, startPos, Quaternion.identity);
                Rigidbody projRb = proj.GetComponent<Rigidbody>();
                if (projRb) projRb.linearVelocity = Vector3.down * 30f;
                Destroy(proj, 2f);
            }

            yield return new WaitForSeconds(0.2f);

            // Explosion VFX
            GameObject explosion = Instantiate(explosionPrefab, strikePos, Quaternion.identity);
            explosion.transform.localScale = Vector3.one * strikeRadius * 0.4f;

            // Damage all in radius
            Collider[] hits = Physics.OverlapSphere(strikePos, strikeRadius);
            foreach (Collider hitc in hits)
            {
                EnemyBase enemy = hitc.GetComponent<EnemyBase>();
                if (enemy != null)
                    enemy.TakeDamage(damage, player);
            }

            // Camera shake
            CameraShake.Instance?.ShakeCamera(0.5f, 0.3f);
            yield return new WaitForSeconds(timeBetweenStrikes);
        }

        if (decal != null)
            Destroy(decal);

        Destroy(gameObject);
    }

    private Vector3 GetGroundPosition(Vector3 pos)
    {
        if (Physics.Raycast(pos + Vector3.up * 10f, Vector3.down, out RaycastHit hit, 50f))
            return hit.point;
        return pos;
    }
}
