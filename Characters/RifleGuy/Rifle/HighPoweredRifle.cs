using Cinemachine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Weapons/HighPoweredRifle")]
public class HighPoweredRifle : ActionBase
{
    public GameObject bulletPrefab;
    public float bulletSpeed = 20f;
    public float range = 100f;
    public LayerMask hitMask;
    public LineRenderer beamPrefab;
    public AudioClip shotSound;
    public GameObject bulletImpactPrefab;
    public Transform firePoint;
    public Camera cam;

    public override void Initialize(GameObject user)
    {
        Debug.Log("HighPoweredRifle initialized");
        // Initialize any necessary components or variables here
        Transform foundFirePoint = user.transform.Find("AbilityPoint");
        if(foundFirePoint != null)
        {
            firePoint = foundFirePoint;
        }
        else
        {
            Debug.LogError("Fire point not found on " + user.name);
            firePoint = user.transform; // Fallback to the user's transform
        }
        cam = Camera.main;
        damage = 100f; // Set the damage value
    }
/*    public override void Use(GameObject user)
    {
        if(!CanUse()) return;
        

        // Raycast from camera center
        Ray ray = cam.ViewportPointToRay(new Vector3(0.5f, 0.5f, 0));
        Vector3 startPoint = firePoint.position;
        Vector3 targetPoint;
        if (Physics.Raycast(ray, out RaycastHit camHit, range, hitMask, QueryTriggerInteraction.Collide))
        {
            targetPoint = camHit.point;
        }
        else
        {
            targetPoint = ray.origin + ray.direction * range;
        }

        Vector3 direction = (targetPoint - firePoint.position).normalized;
        Vector3 endPoint = firePoint.position + direction * range;
        if (Physics.Raycast(startPoint, direction, out RaycastHit hit, range, hitMask, QueryTriggerInteraction.Collide))
        {
            Debug.Log($"[Raycast] Hit: {hit.collider.name}, Tag: {hit.collider.tag}, Layer: {LayerMask.LayerToName(hit.collider.gameObject.layer)}, IsTrigger: {hit.collider.isTrigger}");
            endPoint = hit.point;
            // Apply damage
                if (hit.collider.TryGetComponent(out IDamageable dmg))
            {

                dmg.TakeDamage(damage, user);
                Debug.Log("Hit: " + hit.collider.name + " for " + dmg);
            }

            if (bulletImpactPrefab) {
                GameObject impact = Instantiate(bulletImpactPrefab, hit.point, Quaternion.LookRotation(hit.normal));
                Destroy(impact, 1f);
            }
        }
            // No hit, beam goes straight
            SpawnBeam(startPoint, endPoint);

        if (shotSound)
        {
            AudioSource.PlayClipAtPoint(shotSound, firePoint.position);
        }
        CameraShake.Instance.ShakeCamera(5f, .1f);

        BaseCharacter character = user.GetComponent<BaseCharacter>();
        //character.maxHealth.ApplyModifier(-0.1f); // Example of applying a stat change
        UpdateAction();
    }*/
    private void SpawnBeam(Vector3 start, Vector3 end)
    {
        LineRenderer beam = Instantiate(beamPrefab);
        beam.positionCount = 2;
        beam.useWorldSpace = true;
        beam.SetPosition(0, start);
        beam.SetPosition(1, end);
        beam.startWidth = 0.1f;
        beam.endWidth = 0.1f;

        // Destroy the beam after a short time
        Object.Destroy(beam.gameObject, 0.05f);
    }
    public override void Use(GameObject user)
    {
        if (!CanUse()) return;

        // Default behavior: use camera for player
        cam = Camera.main;
        Ray ray = cam.ViewportPointToRay(new Vector3(0.5f, 0.5f, 0));
        Vector3 targetPoint = ray.origin + ray.direction * range;

        if (Physics.Raycast(ray, out RaycastHit camHit, range, hitMask, QueryTriggerInteraction.Collide))
        {
            targetPoint = camHit.point;
        }

        Fire(user, targetPoint);
    }

    public override void Use(GameObject user, Vector3 targetPosition)
    {
        if (!CanUse()) return;
        Fire(user, targetPosition);
    }

    private void Fire(GameObject user, Vector3 targetPoint)
    {
        Vector3 startPoint = firePoint != null ? firePoint.position : user.transform.position;
        Vector3 direction = (targetPoint - startPoint).normalized;
        Vector3 endPoint = startPoint + direction * range;

        if (Physics.Raycast(startPoint, direction, out RaycastHit hit, range, hitMask, QueryTriggerInteraction.Collide))
        {
            endPoint = hit.point;

            if (hit.collider.TryGetComponent(out IDamageable dmg))
            {
                dmg.TakeDamage(damage, user);
                Debug.Log($"Hit: {hit.collider.name} for {damage}");
            }

            if (bulletImpactPrefab)
            {
                GameObject impact = Instantiate(bulletImpactPrefab, hit.point, Quaternion.LookRotation(hit.normal));
                Object.Destroy(impact, 1f);
            }
        }

        SpawnBeam(startPoint, endPoint);

        if (shotSound)
        {
            AudioSource.PlayClipAtPoint(shotSound, startPoint);
        }

        CameraShake.Instance.ShakeCamera(5f, .1f);
        UpdateAction();
    }



}
