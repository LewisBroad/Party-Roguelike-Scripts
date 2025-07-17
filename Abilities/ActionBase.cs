using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class ActionBase : ScriptableObject
{
    [Header("Action Settings")]
    public string actionName;
    public string actionDescription;
    public double fireRate = 1f;
    public double damage;
    public double fireTimer;
    public Sprite icon;

    protected ActionModifier mods = new ActionModifier();

    public virtual void SetModifiers(ActionModifier modifiers)
    {
        mods = modifiers;

    }

    public double GetModifiedFireRate() => fireRate * mods.fireRateModifier;


    public virtual void Initialize(GameObject user) {
/*        var modifiers = user.GetComponent<ModifierManager>();
        if (modifiers != null)
        {
            bulletSpeed *= modifiers.GetModifier("BulletSpeed");
            fireRate *= modifiers.GetModifier("FireRate");
            minSpread *= modifiers.GetModifier("Accuracy");
            maxSpread *= modifiers.GetModifier("Accuracy");
        }*/
    }
    public virtual void Activate(GameObject user)
    {
        Debug.Log($"{actionName} activated by {user.name}");
    }
    public virtual void UpdateAction() 
    {
        if (fireTimer > 0)
        {
            fireTimer -= Time.deltaTime;
        }
    }
    public virtual bool IsFiring(ICharacterInput input)
    {
        return false;
    }
    public bool CanUse() => fireTimer <= 0f;

    public void ResetFireTimer() => fireTimer = 1f / fireRate;

    public abstract void Use(GameObject user);
    public virtual void Use(GameObject user, Vector3 targetPosition)
    {
        Use(user); // Default: fallback to normal use
    }
}
