using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ActionModifier
{
    public float fireRateModifier = 1f;
    public float damageModifier = 1f;
    public float accuracyModifier = 1f;
    public float recoilModifier = 1f;
    public float spreadModifier = 1f;

    public void ApplyTo(ActionBase Action)
    {
        Action.fireRate *= (1 + (0.2 * fireRateModifier));
        Action.damage *= (1 + (0.2 * damageModifier));
    }


}
