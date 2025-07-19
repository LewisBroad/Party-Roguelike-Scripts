using UnityEngine.AI;
using UnityEngine;

public class AICharacterController : MonoBehaviour
{
    public Transform followTarget;
    public float followDistance = 5f;
    public float attackRange = 10f;
    public float abilityCooldown = 5f;

    private float abilityTimer = 0f;
    private NavMeshAgent agent;
    private EnemyBase currentTarget;

    private CharacterCombat combat; // Your character's weapon/ability interface

    void Start()
    {
        agent = GetComponent<NavMeshAgent>();
        combat = GetComponent<CharacterCombat>();
    }

    void Update()
    {
        FollowPlayer();
        ScanForEnemies();
        HandleCombat();
    }

    public void SetFollowTarget(Transform target)
    {
        followTarget = target;
    }

    void FollowPlayer()
    {
        if (Vector3.Distance(transform.position, followTarget.position) > followDistance)
            agent.SetDestination(followTarget.position);
    }

    void ScanForEnemies()
    {
        Collider[] enemies = Physics.OverlapSphere(transform.position, attackRange, LayerMask.GetMask("Enemy"));
        float closestDist = float.MaxValue;

        foreach (var col in enemies)
        {
            float dist = Vector3.Distance(transform.position, col.transform.position);
            if (dist < closestDist)
            {
                closestDist = dist;
                currentTarget = col.GetComponent<EnemyBase>();
            }
        }
    }

    void HandleCombat()
    {
        if (currentTarget == null || !currentTarget.gameObject.activeInHierarchy)
            return;

        if (Vector3.Distance(transform.position, currentTarget.transform.position) <= combat.attackRange)
        {
            combat.Attack(currentTarget);

            if (abilityTimer <= 0f && combat.CanUseAbility(currentTarget))
            {
                combat.UseAbility(currentTarget);
                abilityTimer = abilityCooldown;
            }
        }

        abilityTimer -= Time.deltaTime;
    }
}
