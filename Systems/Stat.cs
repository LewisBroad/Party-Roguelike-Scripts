using System.Collections;
using System.Collections.Generic;
using UnityEngine;


[System.Serializable]
public class Stat
{
    public float BaseValue;
    public string Description;
    public List<float> modifiers = new List<float>();
    public float ModifiedValue => BaseValue * (1 + Modifier);
    public float Modifier = 0f;

    public float Value
    {
        get
        {
            float finalValue = BaseValue;
            foreach (float mod in modifiers)
            {
                finalValue += mod;
            }
            return finalValue;
        }
    }

    public void ApplyModifier(float modAmount) => Modifier += modAmount;
    public void ResetModifiers() => Modifier = 0f;

    public float GetValue()
    {
        float finalValue = BaseValue;
        foreach (float mod in modifiers)
            finalValue += mod;

        finalValue *= (1 + Modifier); // Apply percentage modifier
        return finalValue;
    }
}

