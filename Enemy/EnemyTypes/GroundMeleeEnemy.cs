using UnityEngine;
using UnityEngine.AI;

public class GroundMeleeEnemy : EnemyBase
{
    public float attackRange = 2f;
    public float attackCooldown = 1f;
    public float attackDamage = 10f;

    private float lastAttackTime = -999;

    protected override void Start()
    {
        base.Start();
    }

    protected override void Update()
    {
        base.Update();
        if (currentState == enemyState.Angered && playerTarget != null && agent.enabled)
        {
            agent.SetDestination(playerTarget.position);
            float distance = Vector3.Distance(transform.position, playerTarget.position);
            if (distance <= attackRange && Time.time > lastAttackTime + attackCooldown)
            {
                lastAttackTime = Time.time;
                Attack();
            }
        }
    }

    public override void Attack()
    {
        base.Attack();
        IDamageable damageableTarget = playerTarget.GetComponent<IDamageable>();
        if (damageableTarget != null)
        {
            damageableTarget.TakeDamage(attackDamage, gameObject);
        }
        else
        {
            Debug.LogWarning($"{playerTarget.name} is not damageable.");
        }
        //replace with damage to call to player if they implement IDamageable
        Debug.Log($"{gameObject.name} attacks for {attackDamage} damage! to {playerTarget.name}");
    }
}
