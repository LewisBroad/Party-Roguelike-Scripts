using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Upgrades/Action Upgrade")]
public class ActionUpgrade : ScriptableObject
{
    public string actionName;
    public ActionModifier modifier;
}
