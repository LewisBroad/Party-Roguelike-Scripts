using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UpgradeManager : MonoBehaviour
{
    public List<StatUpgrade> availableUpgrades;
    public List<ActionUpgrade> actionUpgrades;
    //public List<AbilityUpgrade> abilityUpgrades;

    public void ApplyStatToCharacter(CharacterStats stats, StatUpgrade upgrade)
    {
        stats.ApplyUpgrade(upgrade);
    }

    public void ApplyActionUpgradeToAction(ActionBase action, ActionUpgrade upgrade)
    {
        upgrade.modifier.ApplyTo(action);
    }

}
