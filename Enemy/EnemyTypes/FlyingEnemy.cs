using System.IO;
using UnityEditor.VersionControl;
using UnityEngine;

public class FlyingEnemy : EnemyBase
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

    public GameObject projectilePrefab;
    public Transform projectileSpawnPoint;
    public float projectileSpeed = 10f;
    public float meleeDamage = 10f;
    public float meleeRadius = 1.5f; // Radius for melee attack hit detection
    public LayerMask damageableLayers;

    //private Rigidbody rb;

    protected override void Start()
    {
        // Disable NavMeshAgent and gravity
        InitialiseHealth();
        InitialiseHealthBar();
        if (agent != null) agent.enabled = false;
        rb = GetComponent<Rigidbody>();
        rb.useGravity = false;
        rb.isKinematic = false; // Let physics apply movement if needed
    }

    protected override void Update()
    {
        base.Update();

        if (currentState == enemyState.Angered && playerTarget != null)
        {
            //ChasePlayerInAir();
            ChaseAndAttackPlayer();
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
        Vector3 targetPosition = playerTarget.position + Vector3.up * chaseHeightOffset;
        float distanceToPlayer = Vector3.Distance(transform.position, targetPosition);

        Vector3 lookDirection = (playerTarget.position - transform.position).normalized;
        lookDirection.y = 0f; // Keep the enemy looking horizontally
        if (lookDirection != Vector3.zero)
        {
            Quaternion lookRotation = Quaternion.LookRotation(lookDirection);
            transform.rotation = Quaternion.Slerp(transform.rotation, lookRotation, Time.deltaTime * 5f);
        }
        if (Time.time >= lastAttackTime + attackCooldown)
        {
            if (distanceToPlayer <= attackRange)
            {
                // Within melee attack range
                MeleeAttack();
            }
            else if (distanceToPlayer <= RangedAttackRange)
            {
                // Within ranged attack range
                RangedAttack();
            }
            else if (distanceToPlayer > stoppingDistance + repositionThreshold)
            {
                // Only move if we're farther than the stopping distance
                Vector3 direction = (targetPosition - transform.position).normalized;
                rb.MovePosition(transform.position + direction * moveSpeed * Time.deltaTime);
            }
        }
        else if (distanceToPlayer > stoppingDistance + repositionThreshold)
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
                Vector3 dir = (playerTarget.position - projectileSpawnPoint.position).normalized;
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
}
