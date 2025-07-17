using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

[CreateAssetMenu(menuName = "Weapons/MachineGun")]

public class MachineGun : ActionBase
{
    public GameObject bulletPrefab;
    private GameUIHandler gameUI;
    public float bulletSpeed = 100f;

    [Header("Spread Settings")]
    public float maxAccuracyTime = 5f;
    public float minSpread = 1f;
    public float maxSpread = 10f;
    private float currentVisualSpread = 0f;
    private float spreadReturnSpeed = 10f;

    [Header("Effects")]
    public float shakeAmplitude = 0.5f;
    public float shakeDuration = 0.1f;
    public AudioClip fireSound;
    public GameObject bulletImpactPrefab;
    public ParticleSystem muzzleFlash;

    public float fireHoldTime = 0f;

    public LayerMask hitMask;
    private Transform firePoint;
    private Camera cam;
    private AudioSource audioSource;
    private ICharacterInput currentInput;

    public override void Initialize(GameObject user)
    {
        Debug.Log("MachineGun initialized");
        Transform foundFirePoint = user.transform.Find("AbilityPoint");
        if (foundFirePoint != null)
        {
            firePoint = foundFirePoint;
        }
        else
        {
            Debug.LogError("Fire point not found on " + user.name);
            firePoint = user.transform; // Fallback to the user's transform
        }
        cam = Camera.main;
        currentInput = user.GetComponent<ICharacterInput>();
        gameUI = GameObject.FindFirstObjectByType<GameUIHandler>();

    }
    public override void Use(GameObject user)
    {
        if(!CanUse()) return;
       // firePoint = user.GetComponent<BaseCharacter>().abilityPoint;
        Fire(user);
        ResetFireTimer();

    }

    public override void UpdateAction()
    {
        bool isFiring = currentInput != null && IsFiring(currentInput);

        if (isFiring)
        {
            fireHoldTime += Time.deltaTime;
            fireHoldTime = Mathf.Min(fireHoldTime, maxAccuracyTime); // Clamp to max accuracy time
        }
        else
        {
            fireHoldTime -= Time.deltaTime;
            fireHoldTime = Mathf.Max(fireHoldTime, 0f); // Clamp to 0
        }
        //Calculate current spread based on hold time
        float holdPercent = Mathf.Clamp01(fireHoldTime / maxAccuracyTime);
        float currentSpread = Mathf.Lerp(maxSpread, minSpread, holdPercent);
        float modSpread = Mathf.Clamp01(mods.spreadModifier);
        currentSpread *= modSpread;

        // Convert angular spread to screen-space pixel offset
        float distance = 10f;
        float spreadRadians = currentSpread * Mathf.Deg2Rad;
        float spreadWorldOffset = Mathf.Tan(spreadRadians) * distance;

        Vector3 rightOffset = cam.transform.position + cam.transform.forward * distance + cam.transform.right * spreadWorldOffset;
        Vector3 screenOffset = cam.WorldToScreenPoint(rightOffset);
        float pixelSpread = Mathf.Abs(screenOffset.x - (Screen.width / 2f));

        // Smoothly update UI crosshair
        currentVisualSpread = Mathf.Lerp(currentVisualSpread, pixelSpread, Time.deltaTime * spreadReturnSpeed);
        if (gameUI != null)
        {
            gameUI.SetCrosshairSpread(currentVisualSpread);
        }


        base.UpdateAction(); // Handles fireTimer
    }

    private void Fire(GameObject user)
    {
        float holdPercent = Mathf.Clamp01(fireHoldTime / maxAccuracyTime);
        float currentSpread = Mathf.Lerp(maxSpread, minSpread, holdPercent);
        float modSpread = Mathf.Clamp01(mods.spreadModifier);
        currentSpread *= modSpread; // Apply spread modifier
        //Debug.Log($"Hold Time: {fireHoldTime}, Hold Percent: {holdPercent}, Current Spread: {currentSpread}");

        float distance = 10f; //distance in front of camera;
        float spreadRadians = currentSpread * Mathf.Deg2Rad; // Convert degrees to radians
        float spreadWorldOffset = Mathf.Tan(spreadRadians) * distance; // Calculate world offset based on spread

        Vector3 rightOffset = cam.transform.position + cam.transform.forward * distance + cam.transform.right * spreadWorldOffset;
        Vector3 screenOffset = cam.WorldToScreenPoint(rightOffset);
        float pixelSpread = Mathf.Abs(screenOffset.x - (Screen.width / 2f));
       

        Vector3 targetPoint;
        Ray ray = cam.ViewportPointToRay(new Vector3(0.5f, 0.5f, 0)); // Center of screen
        if (Physics.Raycast(ray, out RaycastHit hit, 1000f, hitMask))
        {
            targetPoint = hit.point;
        }
        else
        {
            targetPoint = ray.GetPoint(1000); // Just shoot far if no hit
        }


        Vector3 direction = (targetPoint - firePoint.position).normalized;
        direction += cam.transform.right * Random.Range(-currentSpread, currentSpread) * 0.01f;
        direction += cam.transform.up * Random.Range(-currentSpread, currentSpread) * 0.01f;
        direction.Normalize();


        if (bulletPrefab && firePoint)
        {
            GameObject bullet = GameObject.Instantiate(bulletPrefab, firePoint.position, Quaternion.LookRotation(direction));
            Bullet bulletComponent = bullet.GetComponentInChildren<Bullet>();
            if (bulletComponent != null)
            {
                bulletComponent.SetDamage((int)damage); // IMPORTANT: set this BEFORE physics acts
                bulletComponent.SetHitMask(hitMask);
            }

            bullet.GetComponentInChildren<Rigidbody>()?.AddForce(direction * bulletSpeed, ForceMode.Impulse);
        }

        if (muzzleFlash && firePoint)
        {
            ParticleSystem flash = GameObject.Instantiate(muzzleFlash, firePoint.position, firePoint.rotation);
            flash.Play();
        }

        if (audioSource && fireSound)
        {
            audioSource.PlayOneShot(fireSound);
        }

        CameraShake.Instance.ShakeCamera(shakeAmplitude, shakeDuration);
    }
    public override bool IsFiring(ICharacterInput input)
    {
        return input.GetSecondaryHeld();
    }
}
