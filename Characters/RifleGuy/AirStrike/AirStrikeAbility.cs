using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using Unity.Burst.CompilerServices;



[CreateAssetMenu(menuName = "Abilities/Air Strike Ability")]
public class AirStrikeAbility : AbilityBase
{
    public GameObject airStrikeMarkerPrefab;
    public GameObject explosionPrefab;
    public float DelayBeforeStrike = 2.0f;
    public float strikeRadius = 5.0f;
    public double damage = 100f;

    public override void ActivateAbility(GameObject player)
    {
        Debug.Log("Air Strike Ability Activated!");

        Vector3 targetPosition = player.transform.position + player.transform.forward * 10f;

        GameObject marker = Instantiate(airStrikeMarkerPrefab, targetPosition, Quaternion.identity);
        player.GetComponent<MonoBehaviour>().StartCoroutine(DelayedStrike(targetPosition, marker, player));
    }
    private IEnumerator DelayedStrike(Vector3 position, GameObject marker, GameObject source)
    {
        yield return new WaitForSeconds(DelayBeforeStrike);
        Destroy(marker);

        GameObject explosion = Instantiate(explosionPrefab, position, Quaternion.identity);
        // Damage logic here (e.g., Physics.OverlapSphere, apply damage to enemies)

        Collider[] hits = Physics.OverlapSphere(position, strikeRadius);
        foreach (Collider hit in hits)
        {
            EnemyBase enemy = hit.GetComponent<EnemyBase>();
            if (enemy != null)
            {
                enemy.TakeDamage(damage, source);
            }
        }
    }
}
