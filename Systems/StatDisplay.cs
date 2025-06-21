using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using UnityEngine.UI;

public class StatDisplay : MonoBehaviour
{
    public Text statNameText;
    public Text statDescriptionText;
    public Text statValueText;
    // Start is called before the first frame update
    
    public void SetStat(string name, float baseValue, float modifiedValue, string description)
    {
        statNameText.text = name;
        statDescriptionText.text = description;
        statValueText.text = modifiedValue != baseValue ? $"{modifiedValue}({baseValue})" : $"{baseValue}";
    }
}
