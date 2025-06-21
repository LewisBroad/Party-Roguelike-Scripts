using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Game/Character")]
public class CharacterDefinition : ScriptableObject
{
    public string characterName;
    public ActionBase primary;
    public ActionBase secondary;
    public AbilityBase[] abilities;
}
