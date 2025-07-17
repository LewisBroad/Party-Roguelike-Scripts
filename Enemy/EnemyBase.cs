using UnityEngine;
using UnityEngine.AI;
using UnityEngine.UI;
using System.Collections;

public class EnemyBase : MonoBehaviour, IDamageable
{
    public Stat Health;
    public Stat maxHealth;
    public Stat armour;

    [SerializeField] private GameObject healthBarUI;
    private Image healthBarFill;
    private Transform healthBarTransform;
    private Camera mainCam;
    [SerializeField] private GameObject[] possibleDrops;



    public enum enemyState { idle, Angered }
    public enemyState currentState;
    public Transform playerTarget;
    public float aggroRange = 10f;
    private float originalSpeed;

    public Transform overrideTarget;
    private float tauntTimer = 0f;


    public bool IsSpawning { get; protected set; } = false;
    public float spawnDuration = 1.5f; // How long they stay in spawn state
    private float spawnTimer = 0f;

    public event System.Action OnHealthChange;
    protected NavMeshAgent agent;
    public Rigidbody rb;
    private Coroutine knockbackRoutine;
    private Coroutine slowCoroutine;
    public event System.Action OnDeath;


    protected virtual void Start()
    {
        /*        Health.BaseValue = maxHealth.BaseValue;
                agent = GetComponent<NavMeshAgent>();
                rb = GetComponent<Rigidbody>();
                originalSpeed = agent.speed;
                mainCam = Camera.main;
                if(healthBarUI != null)
                {
                    Transform fillTransform = healthBarUI.transform.Find("Background/Fill");
                    if (fillTransform != null)
                    healthBarFill = fillTransform.GetComponent<Image>();
                    else Debug.LogWarning("Fill image not found in health bar UI!");


                    healthBarTransform = healthBarUI.transform;
                    // healthBarUI.SetActive(false); // Hidden until damaged
                }*/

        InitialiseEnemyBase();

    }
    public virtual void BeginSpawning()
    {
        IsSpawning = true;
        spawnTimer = spawnDuration;

        // Trigger spawn animation here
        Animator anim = GetComponent<Animator>();
        if (anim != null)
        {
            anim.SetTrigger("Spawn");
        }

        // Optional: disable movement/attack temporarily
        if (agent != null) agent.isStopped = true;
    }
    protected virtual void FinishSpawning()
    {
        IsSpawning = false;

        // Re-enable movement
        if (agent != null) agent.isStopped = false;

        // Ready to act
    }

    protected virtual void InitialiseEnemyBase()
    {
        InitialiseHealth();
        InitialiseNavMesh();
        InitialiseHealthBar();
    }
    protected void InitialiseHealth()
    {
        Health.BaseValue = maxHealth.BaseValue;
        mainCam = Camera.main;
    }
    protected void InitialiseNavMesh()
    {
        agent = GetComponent<NavMeshAgent>();
        rb = GetComponent<Rigidbody>();
        originalSpeed = agent.speed;
        agent.Warp(transform.position); // if it's already set
    }

    protected void InitialiseHealthBar()
    {
        if (healthBarUI != null)
        {
            Transform fillTransform = healthBarUI.transform.Find("Background/Fill");
            if (fillTransform != null)
                healthBarFill = fillTransform.GetComponent<Image>();
            else Debug.LogWarning("Fill image not found in health bar UI!");


            healthBarTransform = healthBarUI.transform;
            // healthBarUI.SetActive(false); // Hidden until damaged
        }
    }


    protected virtual void Update()
    {
        if (IsSpawning)
        {
            spawnTimer -= Time.deltaTime;
            if (spawnTimer <= 0f)
            {
                FinishSpawning();
            }
            return; // Don't do anything else while spawning
        }

        if (currentState == enemyState.idle && playerTarget != null)
        {
            float dist = Vector3.Distance(transform.position, playerTarget.position);
            if (dist < aggroRange)
            {
                BecomeAngered();
            }
        }
        if (healthBarUI.activeSelf && mainCam != null)
        {
            healthBarTransform.position = transform.position + Vector3.up * 2.2f;
            healthBarTransform.rotation = Quaternion.LookRotation(healthBarTransform.position - mainCam.transform.position);

        }
        if (currentState == enemyState.Angered)
        {
            Transform target = GetCurrentTarget();
          
        }
        if (tauntTimer > 0f)
        {
            tauntTimer -= Time.deltaTime;
            if (tauntTimer <= 0f)
            {
                overrideTarget = null;
            }
        }

        if (transform.position.y < -10f) // Check if the enemy has fallen off the map
        {
            Debug.Log($"{gameObject.name} fell off the map and will be destroyed.");
            Die();
        }
        if (Health.BaseValue <= 0)
        {
            Die();
        }

    }
    protected virtual void BecomeAngered()
    {
        currentState = enemyState.Angered;
        rb.linearVelocity = Vector3.zero;
        Debug.Log($"{gameObject.name} is now angry!");
    }

    public virtual void TakeDamage(double amount, GameObject source)
    {
        if (IsSpawning) return;
        Debug.Log($"{gameObject.name} took {amount} damage from {source.name}");
        healthBarUI.SetActive(true);
        if (enemyState.idle == currentState)
        {
            BecomeAngered();
        }
        if (!healthBarUI.activeSelf)
        {
            Debug.Log($"{gameObject.name} health bar activated.");
            healthBarUI.SetActive(true);
        }
        float damage = Mathf.Max(0, (float)amount - armour.BaseValue);
        Health.BaseValue -= damage;
        OnHealthChange?.Invoke();

        Debug.Log($"{gameObject.name} took {damage} damage. Health: {Health.BaseValue}");

        if (healthBarFill != null)
        {
            float fill = Mathf.Clamp01((float)Health.BaseValue / maxHealth.BaseValue);
            healthBarFill.fillAmount = fill;
            Debug.Log($"Health: {Health.BaseValue}/{maxHealth.BaseValue} -> Fill: {fill}");
        }

        /*        Vector3 knockback = (transform.position - source.transform.position).normalized; // Adjust knockback force as needed
                StartCoroutine(ApplyKnockback(knockback * 10f, 0.5f)); // Adjust force and duration as needed*/


        if (Health.BaseValue <= 0)
        {
            Die();
        }
    }
    public virtual void Attack()
    {
        if (playerTarget != null)
        {
            // Add attack logic here
            Debug.Log($"{gameObject.name} attacks {playerTarget.name}");
        }
    }
    protected virtual void Die()
    {
        //play death effect
        Debug.Log($"{gameObject.name} died!");
        DropLoot();

        healthBarUI.SetActive(false); // Hide health bar on death
        OnDeath?.Invoke(); // Notify SpawnManager

        gameObject.SetActive(false);
        //Destroy(gameObject);
    }
    private void DropLoot()
    {
        foreach (var item in possibleDrops)
        {
            if (UnityEngine.Random.value < 0.5f) // 50% drop rate
            {
                Instantiate(item, transform.position, Quaternion.identity);
            }
        }
    }

    public void ApplySlow(float slowAmount, float duration)
    {
        
        if (slowCoroutine != null)
        {
            StopCoroutine(slowCoroutine);
        }
        slowCoroutine = StartCoroutine(SlowEffect(slowAmount, duration));
    }

    private IEnumerator SlowEffect(float slowAmount, float duration)
    {
        if(agent != null) agent.speed = originalSpeed * (1f - slowAmount); // e.g., slowAmount = 0.5f for 50% slow

        yield return new WaitForSeconds(duration);
        if(agent != null) agent.speed = originalSpeed;
        slowCoroutine = null;
    }




    IEnumerator ApplyKnockback(Vector3 force, float duration)
    {
        agent.enabled = false;                   // Stop NavMesh control
        rb.isKinematic = false;                 // Allow physics
        rb.AddForce(force, ForceMode.Impulse);  // Apply knockback

        yield return new WaitForSeconds(duration);

        rb.linearVelocity = Vector3.zero;
        rb.isKinematic = true;                  // Stop physics interference
        agent.enabled = true;                   // Resume NavMeshAgent control
    }

    private IEnumerator KnockbackRoutine(Vector3 direction, float force, float duration)
    {
        agent.enabled = false;
        rb.isKinematic = false;
        rb.useGravity = true;

        rb.AddForce(direction.normalized * force, ForceMode.Impulse);

        yield return new WaitForSeconds(duration);

        rb.linearVelocity = Vector3.zero;
        rb.angularVelocity = Vector3.zero;

        rb.isKinematic = true;
        rb.useGravity = false;
        agent.enabled = true;
    }

    public virtual void SetTemporaryTarget(Transform distraction, float duration)
    {
        StartCoroutine(TemporarilyRedirect(distraction, duration));
    }

    private IEnumerator TemporarilyRedirect(Transform distraction, float duration)
    {
        NavMeshAgent agent = GetComponent<NavMeshAgent>();
        if (agent == null || distraction == null) yield break;

        Vector3 originalDestination = agent.destination;
        agent.SetDestination(distraction.position);

        float elapsed = 0f;
        while (elapsed < duration)
        {
            elapsed += Time.deltaTime;
            agent.SetDestination(distraction.position); // Update to follow moving decoy
            yield return null;
        }

        agent.SetDestination(originalDestination);
    }

    public void TauntTo(Transform newTarget, float duration)
    {
        overrideTarget = newTarget;
        tauntTimer = Time.time + duration;
    }

    protected virtual Transform GetCurrentTarget()
    {
        if (overrideTarget != null && Time.time < tauntTimer)
        {
            // Decoy is still active — return it
            if (overrideTarget.gameObject.activeInHierarchy)
            {
                return overrideTarget;
            }
            else
            {
                // Decoy was destroyed or disabled — clear taunt
                overrideTarget = null;
                tauntTimer = 0f;
            }
        }

        return playerTarget;
    }
    public virtual void ClearTaunt()
    {
        overrideTarget = null;
        tauntTimer = 0f;
    }
}
