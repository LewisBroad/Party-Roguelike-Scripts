using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Actions/MeleeSlashAction")]
public class MeleeAction : ActionBase
{
    public float range = 2f;
    public int damage = 10;

    public override void Use(GameObject user)
    {
        Debug.Log("Slash attack triggered.");
        // Raycast or trigger area detection for enemies
    }
}
