using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StatsManagerUI : MonoBehaviour
{
    public StatDisplay statDisplayPrefab;
    public Transform statDisplayContainer;

    public void DisplayStats(BaseCharacter character)
    {
        foreach (Transform child in statDisplayContainer)
        {
            Destroy(child.gameObject);
        }

        AddStatDisplay("Max Health", character.maxHealth);
        AddStatDisplay("Cooldown Reduction", character.cooldownReduction);
        AddStatDisplay("Damage Reduction", character.armor);
        AddStatDisplay("Movement Speed", character.moveSpeed);
        AddStatDisplay("Crit Chance", character.critChance);
        AddStatDisplay("Crit Damage", character.critDamage);
        AddStatDisplay("Max Shield", character.Shield);
        AddStatDisplay("Shield", character.maxShield);



    }
    public void AddStatDisplay(string name, Stat stat)
    {
        var display = Instantiate(statDisplayPrefab, statDisplayContainer);
        display.SetStat(name, stat.BaseValue, stat.GetValue(), stat.Description);
    }

    }
