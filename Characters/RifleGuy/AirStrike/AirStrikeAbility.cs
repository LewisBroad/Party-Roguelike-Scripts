using UnityEngine;

[CreateAssetMenu(menuName = "Abilities/Air Strike Ability")]
public class AirStrikeAbility : AbilityBase
{
    public GameObject airStrikeMarkerPrefab; // The beacon prefab (should include AirstrikeBeacon)
    public GameObject explosionPrefab;
    public GameObject projectilePrefab;
    public GameObject strikeDecalPrefab;

    public float delayBeforeStrike = 1.5f;
    public float strikeRadius = 8.0f;
    public double damage = 100f;
    public int numberOfStrikes = 5;
    public float timeBetweenStrikes = 0.3f;
    public float projectileHeight = 30f;

    public override void ActivateAbility(GameObject player)
    {
        Vector3 throwOrigin = player.transform.position + Vector3.up * 1.5f;
        Vector3 throwDir;

        // Raycast from screen center to determine throw direction
        Camera cam = Camera.main;
        Ray ray = cam.ViewportPointToRay(new Vector3(0.5f, 0.5f, 0));
        if (Physics.Raycast(ray, out RaycastHit hit, 100f))
        {
            Vector3 targetPoint = hit.point;
            throwDir = (targetPoint - throwOrigin).normalized + Vector3.up * 0.2f; // Add small arc
        }
        else
        {
            throwDir = cam.transform.forward + Vector3.up * 0.2f; // Fallback direction
        }

        GameObject beaconObj = Instantiate(airStrikeMarkerPrefab, throwOrigin, Quaternion.identity);
        Rigidbody rb = beaconObj.GetComponent<Rigidbody>();
        if (rb)
            rb.collisionDetectionMode = CollisionDetectionMode.Continuous;
            rb.linearVelocity = throwDir.normalized * 15f;

        AirstrikeBeacon beacon = beaconObj.GetComponent<AirstrikeBeacon>();
        if (beacon)
        {
            beacon.explosionPrefab = explosionPrefab;
            beacon.projectilePrefab = projectilePrefab;
            beacon.strikeDecalPrefab = strikeDecalPrefab;

            beacon.delayBeforeStrike = delayBeforeStrike;
            beacon.strikeRadius = strikeRadius;
            beacon.damage = damage;
            beacon.numberOfStrikes = numberOfStrikes;
            beacon.timeBetweenStrikes = timeBetweenStrikes;
            beacon.projectileHeight = projectileHeight;

            beacon.Initialize(player);
        }
    }
}

/*private IEnumerator DelayedStrike(Vector3 position, GameObject marker, GameObject decal, GameObject source)
{
    yield return new WaitForSeconds(delayBeforeStrike);
    Destroy(marker);

    for (int i = 0; i < numberOfStrikes; i++)
    {
        Vector3 offset = Random.insideUnitCircle * strikeRadius;
        Vector3 strikePos = position + new Vector3(offset.x, 0, offset.y);

        if (projectilePrefab != null)
        {
            Vector3 startPos = strikePos + Vector3.up * projectileHeight;
            GameObject proj = Instantiate(projectilePrefab, startPos, Quaternion.identity);
            proj.GetComponent<Rigidbody>().linearVelocity = Vector3.down * 30f;
            Destroy(proj, 2f); // fallback
        }

        yield return new WaitForSeconds(0.2f); // delay before blast

        GameObject explosion = Instantiate(explosionPrefab, strikePos, Quaternion.identity);

        Collider[] hits = Physics.OverlapSphere(strikePos, 2.5f);
        foreach (Collider hit in hits)
        {
            EnemyBase enemy = hit.GetComponent<EnemyBase>();
            if (enemy != null)
            {
                enemy.TakeDamage(damage, source);
            }
        }

        CameraShake.Instance?.ShakeCamera(0.5f, 0.3f); // shake (optional)
        yield return new WaitForSeconds(timeBetweenStrikes);
    }
    if (marker != null)
    {
        Destroy(marker);
    }
    if (decal != null)
    {
        Destroy(decal);
    }
}*/


/*private IEnumerator DelayedStrike(Vector3 position, GameObject marker, GameObject decal, GameObject source)
{
    yield return new WaitForSeconds(delayBeforeStrike);
    Destroy(marker);

    for (int i = 0; i < numberOfStrikes; i++)
    {
        Vector3 offset = Random.insideUnitCircle * strikeRadius;
        Vector3 strikePos = position + new Vector3(offset.x, 0, offset.y);

        if (projectilePrefab != null)
        {
            Vector3 startPos = strikePos + Vector3.up * projectileHeight;
            GameObject proj = Instantiate(projectilePrefab, startPos, Quaternion.identity);
            proj.GetComponent<Rigidbody>().linearVelocity = Vector3.down * 30f;
            Destroy(proj, 2f); // fallback
        }

        yield return new WaitForSeconds(0.2f); // delay before blast

        GameObject explosion = Instantiate(explosionPrefab, strikePos, Quaternion.identity);

        Collider[] hits = Physics.OverlapSphere(strikePos, 2.5f);
        foreach (Collider hit in hits)
        {
            EnemyBase enemy = hit.GetComponent<EnemyBase>();
            if (enemy != null)
            {
                enemy.TakeDamage(damage, source);
            }
        }

        CameraShake.Instance?.ShakeCamera(0.5f, 0.3f); // shake (optional)
        yield return new WaitForSeconds(timeBetweenStrikes);
    }
    if (marker != null)
    {
        Destroy(marker);
    }
    if (decal != null)
    {
        Destroy(decal);
    }
}*/
