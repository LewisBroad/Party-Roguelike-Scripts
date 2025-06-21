using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class SimpleStatDisplayUI : MonoBehaviour
{
    public BaseCharacter character; // Assign your player in inspector
    public TextMeshProUGUI statText; // Link your text component

    void Update()
    {

        if (character == null || statText == null)
        {
            Debug.Log("Character or statText is null. Please assign them in the inspector.");
            return;
        }
        // Update the text with the character's stats

        statText.text = $"Max Health: {character.maxHealth.GetValue()}\n" +
                        $"Max Shield: {character.maxShield.GetValue()}\n" +
                        $"Armor: {character.armor.GetValue()}\n" +
                        $"Cooldown Reduction: {character.cooldownReduction.GetValue()}%\n" +
                        $"Crit Chance: {character.critChance.GetValue()}%\n" +
                        $"Crit Damage: {character.critDamage.GetValue()}%" +
                        $"Move Speed: {character.moveSpeed.GetValue()}\n"+
                        $"Shield: {character.Shield.GetValue()}\n";

    }
}
