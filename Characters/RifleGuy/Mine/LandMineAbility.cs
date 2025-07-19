using UnityEngine;

[CreateAssetMenu(menuName = "Abilities/LandMine")]
public class LandMineAbility : AbilityBase
{
    public GameObject minePrefab;
    public float throwForce = 5f;
    public float maxAimDistance = 50f;

    public override void ActivateAbility(GameObject player)
    {
        if (minePrefab == null)
        {
            Debug.LogWarning("LandMine prefab not assigned.");
            return;
        }

        Camera cam = Camera.main;
        if (cam == null)
        {
            Debug.LogWarning("No main camera found.");
            return;
        }

        // Get aim target position using a raycast from the center of the screen
        Ray ray = cam.ViewportPointToRay(new Vector3(0.5f, 0.5f)); // center of screen
        Vector3 targetPoint = ray.origin + ray.direction * maxAimDistance;

        if (Physics.Raycast(ray, out RaycastHit hit, maxAimDistance))
        {
            targetPoint = hit.point;
        }

        Vector3 spawnPos = player.transform.position + Vector3.up * 1f;
        GameObject mine = Instantiate(minePrefab, spawnPos, Quaternion.identity);

        // Throw toward aim direction
        if (mine.TryGetComponent(out Rigidbody rb))
        {
            Vector3 throwDir = (targetPoint - spawnPos).normalized;
            rb.AddForce(throwDir * throwForce, ForceMode.Impulse);
        }

        if (mine.TryGetComponent(out LandMine lm))
        {
            lm.source = player;
        }
    }
}
