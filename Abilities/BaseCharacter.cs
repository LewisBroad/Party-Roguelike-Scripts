using System.Collections;
using System.Collections.Generic;
using System.IO;
using System;
using UnityEngine;

public class BaseCharacter : MonoBehaviour, IDamageable
{
    [Header("Character Stats")]
    public Stat health;
    public Stat armor;
    public Stat critChance;
    public Stat critDamage;
    public Stat moveSpeed;
    public Stat maxHealth;
    public Stat Shield;
    public Stat maxShield;
    public Stat cooldownReduction;

    [Header("Abilities")]
    public ActionBase primaryAbility;
    public ActionBase secondaryAbility;
    public AbilityBase[] skills = new AbilityBase[4]; // 4 skills

    private ICharacterInput input;
    public Transform abilityPoint;
    public event Action OnHealthChange;
    public event Action OnShieldChange;

    protected virtual void Awake()
    {
        input = GetComponent<ICharacterInput>();
        if (input == null)
        {
            Debug.LogError("No ICharacterInput component found on " + gameObject.name);
        }
        if (primaryAbility != null)
        {
            primaryAbility.Initialize(gameObject);
        }
        if (secondaryAbility != null)
        {
            secondaryAbility.Initialize(gameObject);
        }
    }

    protected virtual void Update()
    {
        HandleActions();
        HandleAbilities();
        Held();
        rechargeShield();
        
    }

    private void HandleActions()
    {
        primaryAbility?.UpdateAction();
        secondaryAbility?.UpdateAction();
        // Primary
        if (input.GetPrimary() && primaryAbility != null && primaryAbility.CanUse())
        {
            primaryAbility.Use(gameObject);
            primaryAbility.ResetFireTimer();
        }

        // Secondary
        if (input.GetSecondary() && secondaryAbility != null && secondaryAbility.CanUse())
        {
            secondaryAbility.Use(gameObject);
            secondaryAbility.ResetFireTimer();
        }
        
    }
    private void HandleAbilities()
    {
        // Skills
        for (int i = 0; i < skills.Length; i++)
        {
            if (input.GetSkill(i) && skills[i].IsReady())
            {
                skills[i].Activate(gameObject);
                skills[i].ResetCooldown();
            }

        }
        foreach (var skill in skills)
            skill?.UpdateCooldown();
    }
    private void Held()
    {
        if (primaryAbility != null)
        {
            primaryAbility.UpdateAction();
            if (primaryAbility.IsFiring(input) && primaryAbility.CanUse())
            {
                primaryAbility.Use(gameObject);
                primaryAbility.ResetFireTimer();
            }
        }

        if (secondaryAbility != null)
        {
            secondaryAbility.UpdateAction();
            if (secondaryAbility.IsFiring(input) && secondaryAbility.CanUse())
            {
                secondaryAbility.Use(gameObject);
                secondaryAbility.ResetFireTimer();
            }
        }
    }
    private void rechargeShield()
    {

            if (Shield.BaseValue < maxShield.BaseValue) { 
                AddShield(5f * Time.deltaTime);
        }

            else if (Shield.BaseValue <= maxShield.BaseValue) return;

        
    }

    public void TakeDamage(double amount, GameObject source)
    {
        float dmg = (float)amount;
        if (dmg > 0)
        {
            float effectiveDamage = dmg *(1f - armor.BaseValue / 100f); // Calculate effective damage after armor reduction
            effectiveDamage = Mathf.Max(0, effectiveDamage);
            if (Shield.BaseValue > 0)
            {
                float shieldAbsorb = Mathf.Min(Shield.BaseValue, effectiveDamage);
                Shield.BaseValue -= shieldAbsorb;
                effectiveDamage -= shieldAbsorb;
                OnShieldChange?.Invoke();
            }
            if (effectiveDamage > 0)
            {
                health.BaseValue -= effectiveDamage;
                OnHealthChange?.Invoke();

            }

            Debug.Log($"{gameObject.name} took {amount} damage from {source.name}. Health: {health.BaseValue}, Shield: {Shield.BaseValue}");
        }
        else
        {
            health.BaseValue = Mathf.Min(health.BaseValue - dmg, maxHealth.BaseValue);
            OnHealthChange?.Invoke();
            Debug.Log($"{gameObject.name} healed for {-amount}. Health: {health.BaseValue}");
        }
        if (health.BaseValue <= 0)
        {
            Die();
        }
    }

    public void Die()
    {
        Debug.Log("Character has died.");
        // Handle death logic here (e.g., respawn, game over, etc.)
        Destroy(gameObject);
    }
    public void InvokeShieldChange()
    {
        OnShieldChange?.Invoke();
    }
    public void AddShield(float amount)
    {
        OnShieldChange?.Invoke();

        Shield.BaseValue = Mathf.Min(Shield.BaseValue + amount, maxShield.BaseValue);
    }

    public void AddHealth(float amount)
    {
        health.BaseValue = Mathf.Min(health.BaseValue + amount, maxHealth.BaseValue);
        OnHealthChange?.Invoke();
    }

}
