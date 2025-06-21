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
        firePoint = user.GetComponent<BaseCharacter>().abilityPoint;
        cam = Camera.main;
        damage = 100f; // Set the damage value
    }
    public override void Use(GameObject user)
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
    }
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


}
