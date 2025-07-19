using UnityEngine;
using UnityEngine.AI;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;

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
    public Transform[] possibleTargets;
    public float aggroRange = 10f;
    private float originalSpeed;

    public Transform overrideTarget;
    private float tauntTimer = 0f;

    public bool IsSpawning { get; protected set; } = false;
    public float spawnDuration = 1.5f;
    private float spawnTimer = 0f;

    public event System.Action OnHealthChange;
    public event System.Action OnDeath;

    protected NavMeshAgent agent;
    public Rigidbody rb;
    private Coroutine slowCoroutine;

    protected virtual void Start()
    {
        InitialiseEnemyBase();
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
        if (agent != null)
        {
            agent.baseOffset = 0.1f;
            originalSpeed = agent.speed;
            agent.Warp(transform.position);
        }
        rb = GetComponent<Rigidbody>();
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
        }
    }

    public virtual void BeginSpawning()
    {
        IsSpawning = true;
        spawnTimer = spawnDuration;
        if (agent != null) agent.isStopped = true;

        Animator anim = GetComponent<Animator>();
        if (anim) anim.SetTrigger("Spawn");
    }

    protected virtual void FinishSpawning()
    {
        IsSpawning = false;
        if (agent != null) agent.isStopped = false;
    }

    protected virtual void Update()
    {
        if (IsSpawning)
        {
            spawnTimer -= Time.deltaTime;
            if (spawnTimer <= 0f) FinishSpawning();
            return;
        }

        if (currentState == enemyState.idle)
        {
            Transform target = GetCurrentTarget();
            if (target != null && Vector3.Distance(transform.position, target.position) < aggroRange)
            {
                BecomeAngered();
            }
        }

        if (healthBarUI != null && healthBarUI.activeSelf && mainCam != null)
        {
            healthBarTransform.position = transform.position + Vector3.up * 2.2f;
            healthBarTransform.rotation = Quaternion.LookRotation(healthBarTransform.position - mainCam.transform.position);
        }

        if (overrideTarget != null && Time.time > tauntTimer)
            overrideTarget = null;

        if (transform.position.y < -10f || Health.BaseValue <= 0)
            Die();
    }

    protected virtual void BecomeAngered()
    {
        currentState = enemyState.Angered;
        rb.linearVelocity = Vector3.zero;
        Debug.Log($"{gameObject.name} is now angry!");
    }
    public virtual void Attack()
    {
        Transform target = GetCurrentTarget();
        if (target != null)
        {
            // Add attack logic here
            Debug.Log($"{gameObject.name} attacks {target.name}");
        }
    }

    public virtual void TakeDamage(double amount, GameObject source)
    {
        if (IsSpawning) return;

        healthBarUI.SetActive(true);
        if (currentState == enemyState.idle) BecomeAngered();

        float damage = Mathf.Max(0, (float)amount - armour.BaseValue);
        Health.BaseValue -= damage;
        OnHealthChange?.Invoke();

        if (healthBarFill != null)
            healthBarFill.fillAmount = Mathf.Clamp01((float)Health.BaseValue / maxHealth.BaseValue);

        if (Health.BaseValue <= 0)
            Die();
    }

    protected virtual void Die()
    {
        Debug.Log($"{gameObject.name} died!");
        DropLoot();

        healthBarUI.SetActive(false);
        OnDeath?.Invoke();
        gameObject.SetActive(false);
    }

    private void DropLoot()
    {
        foreach (var item in possibleDrops)
        {
            if (Random.value < 0.5f)
                Instantiate(item, transform.position, Quaternion.identity);
        }
    }

    public void ApplySlow(float slowAmount, float duration)
    {
        if (slowCoroutine != null)
            StopCoroutine(slowCoroutine);
        slowCoroutine = StartCoroutine(SlowEffect(slowAmount, duration));
    }

    private IEnumerator SlowEffect(float slowAmount, float duration)
    {
        if (agent != null) agent.speed = originalSpeed * (1f - slowAmount);
        yield return new WaitForSeconds(duration);
        if (agent != null) agent.speed = originalSpeed;
    }

    public void TauntTo(Transform newTarget, float duration)
    {
        overrideTarget = newTarget;
        tauntTimer = Time.time + duration;
    }

    public virtual void ClearTaunt()
    {
        overrideTarget = null;
        tauntTimer = 0f;
    }

    protected virtual Transform GetCurrentTarget()
    {
        if (overrideTarget != null && Time.time < tauntTimer && overrideTarget.gameObject.activeInHierarchy)
            return overrideTarget;

        Transform closest = null;
        float minDist = float.MaxValue;

        foreach (Transform t in possibleTargets)
        {
            if (t == null || !t.gameObject.activeInHierarchy) continue;

            float dist = Vector3.Distance(transform.position, t.position);
            if (dist < minDist && dist < aggroRange)
            {
                minDist = dist;
                closest = t;
            }
        }
        return closest;
    }

    public virtual void SetTargetList(List<Transform> targets)
    {
        possibleTargets = targets.ToArray();
    }

    private void OnCollisionStay(Collision collision)
    {
        if (collision.collider.CompareTag("Player"))
        {
            foreach (ContactPoint contact in collision.contacts)
            {
                if (Vector3.Dot(contact.normal, Vector3.up) > 0.7f)
                {
                    Rigidbody rb = collision.collider.GetComponent<Rigidbody>();
                    if (rb)
                        rb.AddForce(Vector3.up * 5f + transform.forward * 2f, ForceMode.Impulse);
                }
            }
        }
    }

    public bool isAlive() => Health.BaseValue > 0;
}
