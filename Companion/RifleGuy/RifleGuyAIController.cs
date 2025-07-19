using UnityEngine;
using UnityEngine.AI;
using System;
using System.Collections.Generic;
using static UnityEngine.Rendering.DebugUI;
using UnityEngine.TextCore.Text;

[RequireComponent(typeof(NavMeshAgent))]
public class RifleGuyAIController : MonoBehaviour, IDamageable
{
    public CharcterData characterData;
    public Transform playerToFollow;
    public float followDistance = 10f;
    public float attackRange = 20f;
    public LayerMask enemyMask;

    [Header("Character Stats")]
    public Stat health;
    public Stat armor;
    public Stat critChance;
    public Stat critDamage;
    public Stat moveSpeed;
    public Stat maxHealth;
    public Stat Shield;
    public Stat maxShield;
    public Stat cooldownReduction;

    [Header("Weapons")]
    public ActionBase primaryWeapon;
    public ActionBase secondaryWeapon;
    public Transform firePoint;

    [Header("AI Settings)")]
    private float roamRadius = 5f;
    private float roamCooldown = 3f;
    private float roamTimer = 0f;
    private Vector3 lastRoamDestination;
    private float idleTimer = 0f;
    private float idleDuration = 2f;
    private float decisionCooldown = 3f;
    private float decisionTimer = 0f;

    [Header("Healing Logic")]
    public float lowHealthThreshold = 0.3f; // 30% health
    public float healUntilThreshold = 0.6f; // Return to combat at 60% health
    private HealthStation nearestBeacon;
    private float beaconCheckRate = 1f;
    private float beaconCheckTimer = 0f;

    private AbilityBase[] abilities;

    private NavMeshAgent agent;
    public EnemyBase currentTarget;
    private float checkRate = 0.5f;
    private float checkTimer = 0f;
    private float abilityCheckRate = 1f;
    private float abilityCheckTimer = 0f;
    public event Action OnHealthChange;
    public event Action OnShieldChange;

    private enum AIState { FollowPlayer, Roam, Attack, UseAbility, SeekHealing }
    private AIState currentState = AIState.FollowPlayer;


    void Awake()
    {
        if (firePoint == null)
            firePoint = transform.Find("AbilityPoint");

    }
    void Start()
    {


        ApplyCharacterData();
        agent = GetComponent<NavMeshAgent>();
    }

    void Update()
    {
        checkTimer += Time.deltaTime;
        beaconCheckTimer += Time.deltaTime;
        if (checkTimer >= checkRate)
        {
            checkTimer = 0f;
            UpdateTarget();
        }
        if (beaconCheckTimer >= beaconCheckRate)
        {
            beaconCheckTimer = 0f;
            CheckHealthStatus();
        }

        abilityCheckTimer += Time.deltaTime;
        if (abilityCheckTimer >= abilityCheckRate)
        {
            abilityCheckTimer = 0f;
            TryUseAbilities();
        }
        foreach (var ability in abilities)
        {
            ability?.UpdateCooldown();
        }



        primaryWeapon?.UpdateAction();
        secondaryWeapon?.UpdateAction();

        HandleState();
        rechargeShield();
    }

    void HandleState()
    {
        switch (currentState)
        {
            case AIState.FollowPlayer:
                FollowPlayerLogic();
                break;

            case AIState.Attack:
                AttackLogic();
                break;

            case AIState.UseAbility:
                // Reserved for later
                break;

            case AIState.SeekHealing:
                SeekHealingLogic();
                break;
        }
    }


    void UpdateTarget()
    {
        EnemyBase[] enemies = FindObjectsByType<EnemyBase>(FindObjectsSortMode.None);
        EnemyBase closest = null;
        float closestDist = Mathf.Infinity;

        foreach (EnemyBase enemy in enemies)
        {
            float dist = Vector3.Distance(transform.position, enemy.transform.position);
            if (dist < closestDist && dist < attackRange && enemy.isAlive())
            {
                closest = enemy;
                closestDist = dist;
            }
        }

        currentTarget = closest;
        currentState = currentTarget ? AIState.Attack : AIState.FollowPlayer;
    }

    void FollowPlayerLogic()
    {
        if (playerToFollow == null) return;

        float dist = Vector3.Distance(transform.position, playerToFollow.position);

        // If too far, always follow directly
        if (dist > followDistance)
        {
            agent.SetDestination(playerToFollow.position);
            return;
        }

        // Occasionally decide what to do (wander or idle)
        decisionTimer += Time.deltaTime;
        if (decisionTimer >= decisionCooldown)
        {
            decisionTimer = 0f;
            int decision = UnityEngine.Random.Range(0, 3); // 0 = idle, 1/2 = wander

            if (decision == 0)
            {
                idleTimer = idleDuration;
                agent.ResetPath(); // Stop moving
            }
            else
            {
                Vector3 randomOffset = UnityEngine.Random.insideUnitSphere * roamRadius;
                randomOffset.y = 0;

                Vector3 targetPos = playerToFollow.position + randomOffset;
                if (NavMesh.SamplePosition(targetPos, out NavMeshHit hit, 2f, NavMesh.AllAreas))
                {
                    agent.SetDestination(hit.position);
                }
            }
        }

        // Handle idle timing
        if (idleTimer > 0f)
        {
            idleTimer -= Time.deltaTime;
            agent.ResetPath(); // Stay still while idling
        }
    }



    private void AttackLogic()
    {
        if (currentTarget == null || !currentTarget.isAlive())
        {
            currentState = AIState.FollowPlayer;
            return;
        }

        float dist = Vector3.Distance(transform.position, currentTarget.transform.position);
        agent.SetDestination(currentTarget.transform.position);
        FaceTarget(currentTarget.transform); // replace transform.LookAt(...)

        //transform.LookAt(currentTarget.transform);

        // Use weapons based on range
        if (dist <= attackRange)
        {
            if (dist > 10f && primaryWeapon != null && primaryWeapon.CanUse())
            {
                primaryWeapon.Use(gameObject, currentTarget.transform.position);
                primaryWeapon.ResetFireTimer();
            }
            else if (dist <= 10f && secondaryWeapon != null && secondaryWeapon.CanUse())
            {
                secondaryWeapon.Use(gameObject, currentTarget.transform.position);
                secondaryWeapon.ResetFireTimer();
            }
        }
    }
    private void ApplyCharacterData()
    {
        if (characterData == null)
        {
            Debug.LogWarning("CharacterData is missing!");
            return;
        }

        maxHealth.BaseValue = characterData.maxHealth;
        maxShield.BaseValue = characterData.maxShield;
        armor.BaseValue = characterData.armor;
        moveSpeed.BaseValue = characterData.moveSpeed;
        critChance.BaseValue = characterData.critChance;
        critDamage.BaseValue = characterData.critDamage;

        // Clone ActionBase instances so they don't share timers
        primaryWeapon = Instantiate(characterData.primary);
        secondaryWeapon = Instantiate(characterData.secondary);

        primaryWeapon.Initialize(gameObject);
        secondaryWeapon.Initialize(gameObject);

        // Abilities
        abilities = new AbilityBase[characterData.skills.Length];
        for (int i = 0; i < abilities.Length; i++)
        {
            abilities[i] = Instantiate(characterData.skills[i]);
        }
    }
    public void TakeDamage(double amount, GameObject source)
    {
        float dmg = (float)amount;
        if (dmg > 0)
        {
            float effectiveDamage = dmg * (1f - armor.BaseValue / 100f); // Calculate effective damage after armor reduction
            effectiveDamage = Mathf.Max(0, effectiveDamage);
            if (Shield.BaseValue > 0)
            {
                float shieldAbsorb = Mathf.Min(Shield.BaseValue, effectiveDamage);
                Shield.BaseValue -= shieldAbsorb;
                effectiveDamage -= shieldAbsorb;
                OnShieldChange?.Invoke();
            }
            if (effectiveDamage > 0)
            {
                health.BaseValue -= effectiveDamage;
                OnHealthChange?.Invoke();
            }

            Debug.Log($"{gameObject.name} took {amount} damage from {source.name}. Health: {health.BaseValue}, Shield: {Shield.BaseValue}");
        }
        else
        {
            health.BaseValue = Mathf.Min(health.BaseValue - dmg, maxHealth.BaseValue);
            OnHealthChange?.Invoke();
            Debug.Log($"{gameObject.name} healed for {-amount}. Health: {health.BaseValue}");
        }
        if (health.BaseValue <= 0)
        {
            Die();
        }
    }
    private void rechargeShield()
    {

        if (Shield.BaseValue < maxShield.BaseValue)
        {
            AddShield(5f * Time.deltaTime);
        }

        else if (Shield.BaseValue <= maxShield.BaseValue) return;
    }
    public void Die()
    {
        Debug.Log("Character has died.");
        // Handle death logic here (e.g., respawn, game over, etc.)
        Destroy(gameObject);
    }
    public void InvokeShieldChange()
    {
        OnShieldChange?.Invoke();
    }
    public void AddShield(float amount)
    {
        OnShieldChange?.Invoke();

        Shield.BaseValue = Mathf.Min(Shield.BaseValue + amount, maxShield.BaseValue);
    }

    public void AddHealth(float amount)
    {
        health.BaseValue = Mathf.Min(health.BaseValue + amount, maxHealth.BaseValue);
        OnHealthChange?.Invoke();
    }
    private void TryUseAbilities()
    {
        if (abilities == null || abilities.Length == 0)
            return;

        float healthPercent = health.BaseValue / maxHealth.BaseValue;
        int nearbyEnemies = CountNearbyEnemies(8f);

        foreach (var ability in abilities)
        {
            if (ability == null || !ability.IsReady())
                continue;

            string abilityName = ability.name.ToLower();

            // Use personal heal station if under 40% HP
            if (abilityName.Contains("healstation") && healthPercent < 0.4f)
            {
                ability.Activate(gameObject);
                return;
            }

            // Use decoy if low HP and enemies are nearby
            if (abilityName.Contains("decoy") && healthPercent < 0.8f && nearbyEnemies > 0)
            {
                ability.Activate(gameObject);
                return;
            }

            // Use landmine close to enemy
            if (abilityName.Contains("landmine") && currentTarget != null &&
                Vector3.Distance(transform.position, currentTarget.transform.position) < 7f)
            {
                ability.Activate(gameObject);
                return;
            }

            // Use airstrike if 3+ enemies nearby
            if (abilityName.Contains("air") && nearbyEnemies >= 3)
            {
                ability.Activate(gameObject);
                return;
            }
        }
    }


    private int CountNearbyEnemies(float radius)
    {
        Collider[] hits = Physics.OverlapSphere(transform.position, radius, enemyMask);
        int count = 0;
        foreach (Collider hit in hits)
        {
            EnemyBase enemy = hit.GetComponent<EnemyBase>();
            if (enemy != null && enemy.isAlive())
                count++;
        }
        return count;
    }

    void CheckHealthStatus()
    {
        float healthRatio = health.BaseValue / maxHealth.BaseValue;

        if (healthRatio <= lowHealthThreshold)
        {
            HealthStation[] beacons = FindObjectsByType<HealthStation>(FindObjectsSortMode.None);
            HealthStation best = null;
            float closest = Mathf.Infinity;

            foreach (var beacon in beacons)
            {
                float dist = Vector3.Distance(transform.position, beacon.transform.position);
                if (dist < closest)
                {
                    best = beacon;
                    closest = dist;
                }
            }

            if (best != null)
            {
                nearestBeacon = best;
                currentState = AIState.SeekHealing;
            }
        }
        else if (currentState == AIState.SeekHealing && healthRatio > healUntilThreshold)
        {
            currentState = AIState.FollowPlayer;
        }
    }
    void SeekHealingLogic()
    {
        if (nearestBeacon == null)
        {
            currentState = AIState.FollowPlayer;
            return;
        }

        if (!nearestBeacon.IsInside(transform.position))
        {
            agent.SetDestination(nearestBeacon.transform.position);
        }
        else
        {
            agent.ResetPath(); // Stop moving inside the beacon
        }
        if (currentTarget != null && currentTarget.isAlive())
        {
            AttackLogic(); // Continue fighting while healing
        }
        // Optional: face the player or idle anim
        if (playerToFollow != null)
            FaceTarget(playerToFollow);
    }
    void FaceTarget(Transform target)
    {
        Vector3 direction = target.position - transform.position;
        direction.y = 0; // prevent vertical tilt
        if (direction.sqrMagnitude > 0.01f)
        {
            Quaternion rot = Quaternion.LookRotation(direction);
            transform.rotation = Quaternion.Slerp(transform.rotation, rot, Time.deltaTime * 10f);
        }
    }


}
