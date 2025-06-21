using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Actions/GunFireAction")]
public class RangedAction : ActionBase
{
    public GameObject projectilePrefab;
    public float projectileSpeed = 20f;

    public override void Use(GameObject user)
    {
        Transform firePoint = user.GetComponent<BaseCharacter>().abilityPoint;
        GameObject bullet = Instantiate(projectilePrefab, firePoint.position, firePoint.rotation);
        Rigidbody rb = bullet.GetComponent<Rigidbody>();
        rb.linearVelocity = firePoint.forward * projectileSpeed;
    }
}