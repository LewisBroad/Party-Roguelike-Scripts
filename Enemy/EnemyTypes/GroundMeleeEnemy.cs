using UnityEngine;
using UnityEngine.AI;

public class GroundMeleeEnemy : EnemyBase, IPooledObject
{
    public float attackRange = 2f;
    public float attackCooldown = 1f;
    public float attackDamage = 10f;

    private float lastAttackTime = -999;


    public void OnObjectSpawn()
    {
        Health.BaseValue = maxHealth.BaseValue;
        currentState = enemyState.idle;
        if (TryGetComponent<Rigidbody>(out var rb))
        {
            rb.linearVelocity = Vector3.zero;
            rb.angularVelocity = Vector3.zero;
        }

        if (agent != null)
        {
            agent.ResetPath();
            agent.isStopped = false;
        }
    }

    protected override void Start()
    {
        base.Start();
    }

    protected override void Update()
    {
        base.Update();

        if (currentState == enemyState.Angered && agent.enabled)
        {
            // Always get the current best target (decoy if active)
            Transform currentTarget = GetCurrentTarget();

            if (currentTarget != null)
            {
                //playerTarget = currentTarget; // still used in Attack()

                agent.SetDestination(currentTarget.position);
                if (Time.frameCount % 60 == 0 && !agent.pathPending) // Check once per second
                {
                    if (agent.pathStatus == NavMeshPathStatus.PathPartial || agent.pathStatus == NavMeshPathStatus.PathInvalid)
                    {
                        Debug.LogWarning($"{gameObject.name} cannot find a complete path to the target. Path status: {agent.pathStatus}");
                    }
                }
                    float distance = Vector3.Distance(transform.position, currentTarget.position);
                if (distance <= attackRange && Time.time > lastAttackTime + attackCooldown)
                {
                    lastAttackTime = Time.time;
                    Attack();
                }
            }
        }
    }

    public override void Attack()
    {
        base.Attack();
        //IDamageable damageableTarget = playerTarget.GetComponent<IDamageable>();
        Transform target = GetCurrentTarget();
        if (target != null)
        {
            IDamageable damageableTarget = target.GetComponent<IDamageable>();
            if (damageableTarget != null)
            {
                damageableTarget.TakeDamage(attackDamage, gameObject);
            }
            else
            {
                Debug.LogWarning($"{target.name} is not damageable.");
            }
        }
        //replace with damage to call to player if they implement IDamageable
        Debug.Log($"{gameObject.name} attacks for {attackDamage} damage! to {target.name}");
    }
}
