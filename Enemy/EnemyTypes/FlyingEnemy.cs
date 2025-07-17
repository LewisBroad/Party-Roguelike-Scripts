using System.IO;
using UnityEditor.VersionControl;
using System.Collections;

using UnityEngine;

public class FlyingEnemy : EnemyBase, IPooledObject
{
    public float moveSpeed = 5f;
    public float flyHeight = 3f;
    public float hoverRange = 1f;
    public float chaseHeightOffset = 1.5f;
    public float stoppingDistance = 1f;
    private float attackCooldown = 1.5f;
    private float lastAttackTime = -Mathf.Infinity;
    public float attackRange = 2f;      // Range to trigger attack
    public float RangedAttackRange = 10f; // Range for ranged attacks
    public float repositionThreshold = 0.5f; // Optional jitter prevention


    [SerializeField] private float roamRadius = 5f; // Radius within which the enemy roams when idle
    [SerializeField] private float roamCooldown = 3f; // Speed at which the enemy roams when idle
    private Vector3 currentRoamTarget;
    private float roamTimer;
    public float idleHoverHeight = 3f; // Height at which the enemy hovers when idle

    public GameObject projectilePrefab;
    public Transform projectileSpawnPoint;
    public float projectileSpeed = 10f;
    public float meleeDamage = 10f;
    public float meleeRadius = 1.5f; // Radius for melee attack hit detection
    public LayerMask damageableLayers;

    //private Rigidbody rb;
    public void OnObjectSpawn()
    {
        Health.BaseValue = maxHealth.BaseValue;
        currentState = enemyState.idle;
        //transform.position = new Vector3(transform.position.x, idleHoverHeight, transform.position.z);
    }

    protected override void Start()
    {
        // Disable NavMeshAgent and gravity
        InitialiseHealth();
        InitialiseHealthBar();
        //Vector3 clampedSpawn = new Vector3(transform.position.x, idleHoverHeight, transform.position.z);
        //transform.position = clampedSpawn; // Ensure the enemy starts at the correct height
        if (agent != null) agent.enabled = false;
        rb = GetComponent<Rigidbody>();
        rb.useGravity = false;
        StartCoroutine(EnablePhysicsNextFrame());

        rb.constraints = RigidbodyConstraints.FreezeRotation | RigidbodyConstraints.FreezePositionY;
    }
    private IEnumerator EnablePhysicsNextFrame()
    {
        yield return null;
        rb.isKinematic = false;
    }

    protected override void Update()
    {
        base.Update();

        if (currentState == enemyState.Angered)
        {
            playerTarget = GetCurrentTarget(); //  Always re-check best target

            if (playerTarget != null)
            {
                ChaseAndAttackPlayer();
            }
        }

        if (currentState == enemyState.idle)
        {
            IdleHover();
            IdleRoam();
        }
    }

    /*private void ChasePlayerInAir()
    {
        Vector3 targetPosition = playerTarget.position + Vector3.up * chaseHeightOffset;
        float distanceToPlayer = Vector3.Distance(transform.position, targetPosition);
        if (distanceToPlayer > attackRange)
        {
            // Not close enough — move closer
            Vector3 direction = (targetPosition - transform.position).normalized;
            rb.MovePosition(transform.position + direction * moveSpeed * Time.deltaTime);
        }
        else if (distanceToPlayer <= attackRange + repositionThreshold)
        {
            // Within attack range — stop and attack
            Attack();
        }

        // Optionally: rotate toward player
        Vector3 lookDirection = (playerTarget.position - transform.position).normalized;
        lookDirection.y = 0f;
        if (lookDirection != Vector3.zero)
        {
            Quaternion lookRotation = Quaternion.LookRotation(lookDirection);
            transform.rotation = Quaternion.Slerp(transform.rotation, lookRotation, Time.deltaTime * 5f);
        }
    }*/

    private void ChaseAndAttackPlayer()
    {
        // Vector3 targetPosition = playerTarget.position + Vector3.up * chaseHeightOffset;
        //float distanceToPlayer = Vector3.Distance(transform.position, targetPosition);

        Transform currentTarget = GetCurrentTarget();
        if(currentTarget == null) return;

        Vector3 targetPosition = currentTarget.position + Vector3.up * chaseHeightOffset;
        float distanceToTarget = Vector3.Distance(transform.position, targetPosition);

        Vector3 lookDirection = (currentTarget.position - transform.position).normalized;
        lookDirection.y = 0f; // Keep the enemy looking horizontally
        if (lookDirection != Vector3.zero)
        {
            Quaternion lookRotation = Quaternion.LookRotation(lookDirection);
            transform.rotation = Quaternion.Slerp(transform.rotation, lookRotation, Time.deltaTime * 5f);
        }
        if (Time.time >= lastAttackTime + attackCooldown)
        {
            if (distanceToTarget <= attackRange)
            {
                // Within melee attack range
                MeleeAttack();
            }
            else if (distanceToTarget <= RangedAttackRange)
            {
                // Within ranged attack range
                RangedAttack();
            }
            else if (distanceToTarget > stoppingDistance + repositionThreshold)
            {
                // Only move if we're farther than the stopping distance
                Vector3 direction = (targetPosition - transform.position).normalized;
                rb.MovePosition(transform.position + direction * moveSpeed * Time.deltaTime);
            }
        }
        else if (distanceToTarget > stoppingDistance + repositionThreshold)
        {
            Vector3 direction = (targetPosition - transform.position).normalized;
            rb.MovePosition(transform.position + direction * moveSpeed * Time.deltaTime);
        }
    }

    private void MeleeAttack()
    {
        Debug.Log("Flying Enemy Melee Attack!");
        Collider[] hits = Physics.OverlapSphere(transform.position + transform.forward * meleeRadius, meleeRadius, damageableLayers);
        foreach (var hit in hits)
        {
            IDamageable dmg = hit.GetComponent<IDamageable>();
            if(dmg != null)
            {
                dmg.TakeDamage(meleeDamage, gameObject);
            }
        }
        lastAttackTime = Time.time;
    }

    private void RangedAttack()
    {
        Debug.Log("FlyingEnemy: Ranged Projectile Attack!");
        if (projectilePrefab != null && projectileSpawnPoint != null)
        {
            GameObject proj = Instantiate(projectilePrefab, projectileSpawnPoint.position, Quaternion.identity);
            Rigidbody projRb = proj.GetComponent<Rigidbody>();
            if (projRb != null)
            {
                //Vector3 dir = (playerTarget.position - projectileSpawnPoint.position).normalized;
                Transform currentTarget = GetCurrentTarget();
                if (currentTarget == null) return;

                Vector3 dir = (currentTarget.position - projectileSpawnPoint.position).normalized;
                projRb.linearVelocity = dir * projectileSpeed;
            }
        }
        lastAttackTime = Time.time;
    }



    public override void Attack()
    {
        if (Time.time >= lastAttackTime + attackCooldown)
        {
            base.Attack();
            // Add damage logic or projectile here
            lastAttackTime = Time.time;
        }
    }

    protected override void Die()
    {
        base.Die();
        // Optionally: play flying death animation or falling effect
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position + transform.forward * meleeRadius, meleeRadius);
    }

    private void IdleHover()
    {
        Vector3 targetPos = new Vector3(transform.position.x, idleHoverHeight, transform.position.z);
        Vector3 smoothedPos = Vector3.Lerp(transform.position, targetPos, Time.deltaTime * moveSpeed);
        rb.MovePosition(smoothedPos);
    }

    private void IdleRoam()
    {
        if(Vector3.Distance(transform.position, currentRoamTarget) < 0.5f || roamTimer <= 0f){
            PickNewRoamTarget();
            roamTimer = roamCooldown;
        }
        roamTimer -= Time.deltaTime;
        Vector3 direction = (currentRoamTarget - transform.position).normalized;
        rb.MovePosition(transform.position + direction * moveSpeed *0.5f * Time.deltaTime);
    }
    private void PickNewRoamTarget()
    {
        Vector2 randomCircle = Random.insideUnitCircle * roamRadius;
        currentRoamTarget = new Vector3(
            transform.position.x + randomCircle.x,
            idleHoverHeight,
            transform.position.z + randomCircle.y
        );
    }
    protected override void BecomeAngered()
    {
        base.BecomeAngered();
        rb.constraints = RigidbodyConstraints.FreezeRotation | RigidbodyConstraints.FreezePositionY;
    }
}
