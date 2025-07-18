using UnityEngine;
using System.Collections.Generic;

public class DecoyMimic : MonoBehaviour
{
    public float mimicDuration = 20f;
    public ActionBase primaryAction;
    public float fireRange = 20f;

    public float tauntRadius = 15f;
    private float tauntDuration; // set in InitializeFromPlayer
    private float tauntCheckInterval = 1f;
    private float tauntCheckTimer;

    private double fireTimer;
    private double fireRate = 1f;
    private Transform currentTarget;
    private List<EnemyBase> tauntedEnemies = new List<EnemyBase>();

    void Start()
    {
        DecoyManager.Instance.SetDecoy(transform);
    }

    public void InitializeFromPlayer(BaseCharacter player)
    {
        if (player.primaryAbility != null)
        {
            primaryAction = Instantiate(player.primaryAbility);
            primaryAction.Initialize(gameObject);
            fireRate = primaryAction.fireRate;
        }
        else
        {
            Debug.LogWarning("DecoyMimic: Player had no primary ability.");
        }

        tauntDuration = mimicDuration;
        Destroy(gameObject, mimicDuration);
        TauntNearbyEnemies();
    }


    void Update()
    {
        if (primaryAction == null) return;

        fireTimer -= Time.deltaTime;
        tauntCheckTimer -= Time.deltaTime;

        if (fireTimer <= 0f)
        {
            FindTarget();
            if (currentTarget != null)
            {
                FireAtTarget(currentTarget);
                fireTimer = 1f / fireRate;
            }
        }

        if (tauntCheckTimer <= 0f)
        {
            TauntNearbyEnemies();
            tauntCheckTimer = tauntCheckInterval;
        }

        primaryAction?.UpdateAction();
    }

    void FindTarget()
    {
        float closestDistance = Mathf.Infinity;
        currentTarget = null;

        EnemyBase[] enemies = Object.FindObjectsByType<EnemyBase>(FindObjectsSortMode.None);

        foreach (var enemy in enemies)
        {
            if (!enemy.gameObject.activeInHierarchy) continue;

            Vector3 dirToEnemy = enemy.transform.position - transform.position;
            float distance = dirToEnemy.magnitude;

            if (distance > fireRange) continue;

            // Line of sight check
            Vector3 rayOrigin = transform.position + Vector3.up * 1.5f;
            if (Physics.Raycast(rayOrigin, dirToEnemy.normalized, out RaycastHit hit, distance))
            {
                if (hit.collider.gameObject != enemy.gameObject) continue;
            }

            if (distance < closestDistance)
            {
                closestDistance = distance;
                currentTarget = enemy.transform;
            }
        }
    }

    void FireAtTarget(Transform target)
    {
        if (target == null || primaryAction == null) return;

        // Rotate to face target
        Vector3 dir = (target.position - transform.position).normalized;
        transform.forward = new Vector3(dir.x, 0f, dir.z);

        if (primaryAction.CanUse())
        {
            Debug.Log($"DecoyMimic firing at {target.name} from {gameObject.name}");
            Vector3 targetPoint = currentTarget.position + Vector3.up * 1f; // aim at chest height
            primaryAction.Use(gameObject, targetPoint);
            primaryAction.ResetFireTimer();
        }

    }
    void TauntNearbyEnemies()
    {
        Collider[] hits = Physics.OverlapSphere(transform.position, tauntRadius);
        foreach (var col in hits)
        {
            EnemyBase enemy = col.GetComponent<EnemyBase>();
            if (enemy != null && !tauntedEnemies.Contains(enemy))
            {
                enemy.TauntTo(transform, tauntDuration);
                tauntedEnemies.Add(enemy);
            }
        }
    }
    private void OnDestroy()
    {
        foreach (var enemy in tauntedEnemies)
        {
            if (enemy != null && enemy.overrideTarget == transform)
            {
                enemy.ClearTaunt();
            }
        }
    }
}