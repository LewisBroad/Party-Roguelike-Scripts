using System.Collections;
using System.Collections.Generic;
using System.Xml;
using UnityEngine;

public class CharacterLoader : MonoBehaviour
{
    public CharcterData characterData;
    private BaseCharacter character;

    void Awake()
    {
        character = GetComponent<BaseCharacter>();
        if(characterData != null && character != null)
        {
            character.maxHealth.BaseValue = characterData.maxHealth;
            character.maxShield.BaseValue = characterData.maxShield;
            character.armor.BaseValue = characterData.armor;
            character.moveSpeed.BaseValue = characterData.moveSpeed;
            character.critChance.BaseValue = characterData.critChance;
            character.critDamage.BaseValue = characterData.critDamage;

            character.primaryAbility = Instantiate(characterData.primary);
            character.secondaryAbility = Instantiate(characterData.secondary);


            for (int i = 0; i < character.skills.Length; i++)
            {
                if(i < characterData.skills.Length && characterData.skills[i] != null)
                {
                    character.skills[i] = Instantiate(characterData.skills[i]);
                }

            }


        }
    }


}
