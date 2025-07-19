using System.Collections;
using UnityEngine;

[RequireComponent(typeof(SphereCollider))]
public class HealthStation : MonoBehaviour
{
    [Header("Rise Settings (Configured at runtime")]
    private float targetRiseHeight;
    private float riseSpeed;
    private float lifetime;

    [Header("Healing Settings (Configured at runtime")]
    private float healPerSecond;
    private float healRadius;


    public GameObject healingAura;
    private SphereCollider auraCollider;
    private bool hasRisen = false;
    private bool hasActivated = false;
    public LayerMask groundMask;

    [SerializeField] private GameObject healingAuraEffect;
    private GameObject auraInstance;

    private void Awake()
    {
       /* auraCollider = GetComponent<SphereCollider>();
        auraCollider.isTrigger = true;*/
    }

    /// <summary>
    /// Called by the ability script immediately after instantiation.
    /// </summary>

    public void Initialize(float riseHeight, float riseSpeed, float stationLifetime, float healPerSecond, float radius)
    {
        targetRiseHeight = riseHeight;
        this.riseSpeed = riseSpeed;
        lifetime = stationLifetime;
        this.healPerSecond = healPerSecond;
        healRadius = radius;


//        auraCollider.radius = healRadius;

        healingAura.SetActive(false); // Deactivate the healing aura at start


    }

    private IEnumerator RiseAndExpire()
    {
        if(auraInstance != null)
        {
            auraInstance.transform.SetParent(null);
        }
        Quaternion targetRotation = Quaternion.Euler(0f, transform.rotation.eulerAngles.y, 0f);
        float rotationDuration = 0.5f;
        float elapsedTime = 0f;
        Quaternion startRotation = transform.rotation;
        while(elapsedTime < rotationDuration)
        {
            transform.rotation = Quaternion.Slerp(startRotation, targetRotation, elapsedTime / rotationDuration);
            elapsedTime += Time.deltaTime;
            yield return null;
        }
        transform.rotation = targetRotation;

        Vector3 groundPos = transform.root.position;
        Vector3 finalPos = new Vector3(groundPos.x, groundPos.y + targetRiseHeight, groundPos.z);


        while (Vector3.Distance(transform.root.position, finalPos) > 0.1f)
        {
            transform.root.position = Vector3.MoveTowards(transform.root.position, finalPos, riseSpeed * Time.deltaTime);
            yield return null;
        }
        hasRisen = true;

        if(auraInstance != null)
        {
            auraInstance.transform.SetParent(transform);
        }

        healingAura.SetActive(true);

        float elapsed = 0f;
        while (elapsed < lifetime)
        {
            elapsed += Time.deltaTime;
            yield return null;
        }

        Destroy(transform.root.gameObject);
    }

    private void OnCollisionEnter(Collision collision)
    {
        Debug.Log("Health Station collided with: " + collision.gameObject.name);
        if (hasActivated) return;

        if((groundMask.value & (1 << collision.gameObject.layer)) == 0)
        {
            Debug.Log("Health Station collided with non-ground object: " + collision.gameObject.name);
            return;
        }


        Rigidbody rb = GetComponent<Rigidbody>();
        if(rb) rb.isKinematic = true; // Disable physics on collision

        StartCoroutine(RiseAndExpire());
        SpawnAuraEffect(); // Spawn the healing aura effect
        hasActivated = true;


    }

    /*    private void OnTriggerStay(Collider other)
        {
           if(!hasRisen) return;

            if (other.TryGetComponent(out IDamageable dmg))
            {
                float healAmount = healPerSecond * Time.deltaTime;
                dmg.TakeDamage(-healAmount, gameObject); // Negative damage to heal
            }
        }
    */
    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.green;
        Gizmos.DrawWireSphere(transform.root.position, healRadius);
    }
    private void SpawnAuraEffect()
    {
        healingAura.SetActive(false);
        RaycastHit hit;
        if(Physics.Raycast(transform.position, Vector3.down, out hit, Mathf.Infinity, groundMask))
        {
            Vector3 spawnPos = hit.point;
            spawnPos.y += 0.1f; // Adjust the height slightly above the ground
            auraInstance = Instantiate(healingAuraEffect, spawnPos, Quaternion.identity);
         //   Destroy(healingAura.gameObject, lifetime); // Destroy the healing aura after the station expires

        }
        else
        {
            Debug.LogWarning("No ground detected below the health station.");
            Vector3 auraPos = transform.position - new Vector3(0, targetRiseHeight, 0);
            auraInstance = Instantiate(healingAuraEffect, auraPos, Quaternion.Euler(0,0,0));
          //  Destroy(healingAura.gameObject, lifetime); // Destroy the healing aura after the station expires

        }
    }
    public bool IsInside(Vector3 position)
    {
        return Vector3.Distance(transform.position, position) <= healRadius;
    }
}




