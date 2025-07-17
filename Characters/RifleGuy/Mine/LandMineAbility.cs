using UnityEngine;

[CreateAssetMenu(menuName = "Abilities/LandMine")]
public class LandMineAbility : AbilityBase
{
    public GameObject minePrefab;

    public override void ActivateAbility(GameObject player)
    {
        Vector3 dropPosition = player.transform.position + player.transform.forward * 1f;
        GameObject mine = Instantiate(minePrefab, dropPosition, Quaternion.identity);
        LandMine lm = mine.GetComponent<LandMine>();
        if (lm != null)
            lm.source = player;
    }
}
