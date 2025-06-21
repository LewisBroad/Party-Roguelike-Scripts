using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CharacterStats : MonoBehaviour
{
    public Stat maxHealth = new Stat { BaseValue = 100f };
    public Stat cooldownReduction = new Stat { BaseValue = 0f };
    public Stat damageMultiplier = new Stat { BaseValue = 1f };
    public Stat damageReduction = new Stat { BaseValue = 0f };
    public Stat attackSpeed = new Stat { BaseValue = 1f };
    public Stat movementSpeed = new Stat { BaseValue = 5f };
    public Stat critChance = new Stat { BaseValue = 0f };
    public Stat critDamage = new Stat { BaseValue = 1.5f };
    public Stat Shield = new Stat { BaseValue = 0f };

    public void ApplyUpgrade(StatUpgrade upgrade)
    {
        /*    switch (upgrade.statType)
           {
              case StatType.maxHealth: maxHealth.ApplyModifier(upgrade.modifier); break;
               case StatType.CooldownReduction: cooldownReduction.ApplyModifier(upgrade.modifier); break;
               case StatType.DamageMultiplier: damageMultiplier.ApplyModifier(upgrade.modifier); break;
               case StatType.DamageReduction: damageReduction.ApplyModifier(upgrade.modifier); break;
               case StatType.AttackSpeed: attackSpeed.ApplyModifier(upgrade.modifier); break;
               case StatType.MovementSpeed: movementSpeed.ApplyModifier(upgrade.modifier); break;
               case StatType.CritChance: critChance.ApplyModifier(upgrade.modifier); break;
               case StatType.CritDamage: critDamage.ApplyModifier(upgrade.modifier); break;
               case StatType.maxShield: Shield.ApplyModifier(upgrade.modifier); break;}*/
    }
}



