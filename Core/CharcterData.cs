using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Characters/CharacterData")]
public class CharcterData : ScriptableObject
{
    public string characterName;
    public float maxHealth;
    public float maxShield;
    public float armor;
    public float moveSpeed;
    public float critChance;
    public float critDamage;

    public ActionBase primary;
    public ActionBase secondary;

    public AbilityBase[] skills = new AbilityBase[4];
}
