using UnityEngine;

public class HealthStationAura : MonoBehaviour
{
    public float healPerSecond = 10f;
    public bool canGiveShield = false; // Set to true if you want the aura to also give shield

    private void OnTriggerStay(Collider other)
    {
        Debug.Log("Healing Aura Triggered");
        if (other.TryGetComponent(out IDamageable dmg))
        {
            dmg.TakeDamage(-healPerSecond * Time.deltaTime, gameObject); // Heal
            if (other.TryGetComponent(out BaseCharacter character))
            {
                Debug.Log("BaseCharacter Detected");
                if (canGiveShield)
                {
                    if (character.maxShield.BaseValue >= 150) return; // Prevents infinite shield increase
                    float deltaShield = healPerSecond / 4 * Time.deltaTime;
                    if (character.maxShield.BaseValue <= character.maxHealth.BaseValue)
                    {
                        character.maxShield.BaseValue += deltaShield; // Heal

                    }
                    if (character.Shield.BaseValue > character.maxShield.BaseValue)
                    {
                        character.AddShield(deltaShield); // Properly adds and invokes event

                    }
                }

            }
        }

    }
}