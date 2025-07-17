using UnityEngine;

[CreateAssetMenu(menuName = "Abilities/Decoy")]
public class DecoyAbility : AbilityBase
{
    public GameObject decoyPrefab;
    public float duration = 15f;

    public override void ActivateAbility(GameObject player)
    {
        GameObject decoy = Instantiate(decoyPrefab, player.transform.position + Vector3.up, Quaternion.identity);

        if (decoy.TryGetComponent(out Rigidbody rb))
        {
            Vector3 throwDir = player.transform.forward * 5f + Vector3.up * 2f;
            rb.AddForce(throwDir, ForceMode.Impulse);
        }

        if (decoy.TryGetComponent(out DecoyMimic mimic))
        {
            BaseCharacter baseChar = player.GetComponent<BaseCharacter>();
            if (baseChar != null)
            {
                mimic.mimicDuration = duration;
                mimic.InitializeFromPlayer(baseChar);
            }
            else
                Debug.LogWarning("DecoyAbility: No BaseCharacter component found on player.");
        }
    }
}