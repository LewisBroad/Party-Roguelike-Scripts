using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

public class WeaponBase : ScriptableObject
{
    public string weaponName;
    public string description;
    public float fireRate;
    public float damage;

    public Sprite Icon;

    public float fireCooldown;

    public virtual void UpdateCooldown()
    {
        if (fireCooldown > 0)
        {
            fireCooldown -= Time.deltaTime;
        }

    }
    public bool CanFire() => fireCooldown <= 0;

    public void ResetCooldown() => fireCooldown = fireRate;

    public virtual void Fire(GameObject player)
    {
        // Base firing logic (if any)
        Debug.Log($"{weaponName} fired by {player.name}");
    }
}
