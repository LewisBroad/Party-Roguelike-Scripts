using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

public abstract class AbilityBase : ScriptableObject
{
    [Header("Ability Settings")]
    public string abilityName;
    public string description;
    public float cooldown;
    protected float cooldownTimer;
    public Sprite icon;

    public virtual void Activate(GameObject user)
    {
        // Base activation logic (if any)
        Debug.Log($"{abilityName} activated by {user.name}");

        if (!IsReady()) return;

        ActivateAbility(user);
        ResetCooldown();
    }

    public virtual void UpdateCooldown()
    {
        if (cooldownTimer > 0)
        {
            cooldownTimer -= Time.deltaTime;
        }
    }

    public bool IsReady() => cooldownTimer <= 0;

    public void ResetCooldown() => cooldownTimer = cooldown;

    // Abstract method to be implemented by derived classes

    public abstract void ActivateAbility(GameObject player);
}