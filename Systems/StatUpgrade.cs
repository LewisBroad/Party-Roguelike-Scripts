using System.Collections;
using System.Collections.Generic;
using System.Data;
using UnityEngine;

[CreateAssetMenu(menuName = "Upgrades/Stat Upgrade")]
public class StatUpgrade : ScriptableObject
{
    public enum StatType
    {
        maxHealth,
        CooldownReduction,
        DamageMultiplier,
        DamageReduction,
        AttackSpeed,
        MovementSpeed,
        CritChance,
        CritDamage,
        maxShield
    }
    public StatType statToUpgrade;
    public float upgradeAmount;
    public string upgradeName;
    public string upgradeDescription;

    public void Apply(BaseCharacter character)
    {
        switch (statToUpgrade)
        {
            case StatType.maxHealth: character.maxHealth.ApplyModifier(upgradeAmount); break;
            case StatType.CooldownReduction: character.cooldownReduction.ApplyModifier(upgradeAmount); break;
            case StatType.DamageReduction: character.armor.ApplyModifier(upgradeAmount); break;
            case StatType.MovementSpeed: character.moveSpeed.ApplyModifier(upgradeAmount); break;
            case StatType.CritChance: character.critChance.ApplyModifier(upgradeAmount); break;
            case StatType.CritDamage: character.critDamage.ApplyModifier(upgradeAmount); break;
            case StatType.maxShield: character.maxShield.ApplyModifier(upgradeAmount); break;
        }
    }

}


