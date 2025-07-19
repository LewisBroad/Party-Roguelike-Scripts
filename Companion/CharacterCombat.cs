using UnityEngine;

public class CharacterCombat : MonoBehaviour
{
    public float attackRange = 10f;

    public void Attack(EnemyBase target)
    {
        // Shoot or melee depending on character
    }

    public bool CanUseAbility(EnemyBase target)
    {
        // Logic for whether the ability is suitable (e.g., AoE if multiple targets)
        return true;
    }

    public void UseAbility(EnemyBase target)
    {
        // Trigger one of the abilities, possibly by type
    }
}
