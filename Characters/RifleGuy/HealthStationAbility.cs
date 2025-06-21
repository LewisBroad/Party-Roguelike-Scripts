using UnityEngine;

[CreateAssetMenu(menuName = "Abilities/Health Station Ability")]
public class HealthStationAbility : AbilityBase
{
    [Header("Station Prefab")]
    public GameObject healthStationPrefab;

    [Header("Spawn Settings")]
    public float spawnOffset = 1.0f;
    public float throwForce = 5f;
    public float stationLifetime = 10f;

    [Header("Rise Settings")]
    public float riseHeight = 1.0f;
    public float riseSpeed = 2.0f;

    [Header("Healing Settings")]
    public float healPerSecond = 20f;
    public float healRadius = 5f;

    public override void ActivateAbility(GameObject player)
    {
        Debug.Log("Health Station Ability Activated!");
        Vector3 spawnPos = player.transform.position + player.transform.forward * spawnOffset;

        GameObject stationGO = Instantiate(healthStationPrefab, spawnPos, Quaternion.identity);


        HealthStation station = stationGO.GetComponentInChildren<HealthStation>();

        if (station == null)
        {
            Debug.LogError("Health Station prefab does not have a HealthStation component.");
            Destroy(stationGO);
            return;
        }

        station.Initialize(
            riseHeight: riseHeight,
            riseSpeed: riseSpeed,
            stationLifetime: stationLifetime,
            healPerSecond: healPerSecond,
            radius: healRadius);

        Rigidbody stationRb = stationGO.GetComponent<Rigidbody>();
        if (stationRb != null && throwForce > 0f)
        {
            Vector3 throwDirection = Camera.main.transform.forward;
            stationRb.AddForce(throwDirection * throwForce, ForceMode.Impulse);
            //stationRb.AddForce(player.transform.forward * throwForce, ForceMode.Impulse);
        }
    }

}
