using UnityEngine;

public class DecoyMimic : MonoBehaviour
{
    public float mimicDuration = 20f;
    public ActionBase primaryAction;
    public float fireRange = 20f;

    private double fireTimer;
    private double fireRate = 1f;
    private Transform currentTarget;

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

        Destroy(gameObject, mimicDuration);     //  Move it here
        TauntNearbyEnemies();                   //  Also move here so it's only called once we have the final duration
    }

    void Update()
    {
        if (primaryAction == null) return;

        fireTimer -= Time.deltaTime;
        if (fireTimer <= 0f)
        {
            FindTarget();
            if (currentTarget != null)
            {
                FireAtTarget(currentTarget);
                fireTimer = 1f / fireRate;
            }
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
        float tauntRadius = 15f; // Adjustable
        float tauntDuration = mimicDuration; // Matches mimic duration

        Collider[] hits = Physics.OverlapSphere(transform.position, tauntRadius);
        foreach (var col in hits)
        {
            EnemyBase enemy = col.GetComponent<EnemyBase>();
            if (enemy != null)
            {
                enemy.TauntTo(transform, tauntDuration);
            }
        }
    }
    private void OnDestroy()
    {
        ClearTaunts();
    }
    void ClearTaunts()
    {
        float tauntRadius = 15f;

        Collider[] hits = Physics.OverlapSphere(transform.position, tauntRadius);
        foreach (var col in hits)
        {
            EnemyBase enemy = col.GetComponent<EnemyBase>();
            if (enemy != null && enemy.overrideTarget == transform)
            {
                enemy.ClearTaunt();
            }
        }
    }
}
