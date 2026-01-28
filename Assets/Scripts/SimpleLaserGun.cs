using UnityEngine;
using UnityEngine.InputSystem;
using TMPro;
using System.Collections;

public class SimpleLaserGun : MonoBehaviour
{
    [Header("Input Settings")]
    [SerializeField] private InputActionProperty triggerAction;

    [Header("Laser Settings")]
    [SerializeField] private LineRenderer laserLine;
    [SerializeField] private Transform laserStartPoint;
    [SerializeField] private float laserMaxDistance = 50f;
    [SerializeField] private float laserWidth = 0.02f;

    [Header("Laser Colors")]
    [SerializeField] private Color laserColorFull = Color.green;
    [SerializeField] private Color laserColorMid = Color.yellow;
    [SerializeField] private Color laserColorLow = Color.red;
    [SerializeField] private int lowAmmoThreshold = 3;

    [Header("Recoil Settings")]
    [SerializeField] private float recoilDistance = 0.05f;
    [SerializeField] private float recoilDuration = 0.1f;
    [SerializeField] private Transform gunModel;

    [Header("Audio Settings")]
    [SerializeField] private AudioClip shootSound;
    [SerializeField] private AudioClip reloadSound;
    [SerializeField] private float shootVolume = 0.5f;
    [SerializeField] private float reloadVolume = 1f;

    [Header("UI")]
    [SerializeField] private TextMeshProUGUI ammoText;

    [Header("Cooldown Settings")]
    [SerializeField] private float shootCooldown = 0.5f;

    [Header("Ammo Settings")]
    [SerializeField] private int maxAmmo = 10;
    [SerializeField] private float reloadDuration = 4f;

    private AudioSource audioSource;
    private bool wasShooting = false;
    private float lastShootTime = 0f;
    private bool canShoot = true;
    private int currentAmmo;
    private bool isReloading = false;
    private float reloadEndTime = 0f;
    private Vector3 gunOriginalLocalPos;
    private bool isRecoiling = false;

    private void Start()
    {
        if (laserLine != null)
        {
            laserLine.startWidth = laserWidth;
            laserLine.endWidth = laserWidth;
            laserLine.material = new Material(Shader.Find("Sprites/Default"));
            laserLine.enabled = false;
        }

        audioSource = GetComponent<AudioSource>();
        if (audioSource == null)
        {
            audioSource = gameObject.AddComponent<AudioSource>();
        }
        audioSource.playOnAwake = false;

        if (gunModel != null)
        {
            gunOriginalLocalPos = gunModel.localPosition;
        }

        currentAmmo = maxAmmo;
        UpdateAmmoUI();
        UpdateLaserColor();
    }

    private void UpdateAmmoUI()
    {
        if (ammoText != null)
        {
            if (isReloading)
            {
                ammoText.text = "Rechargement...";
            }
            else
            {
                ammoText.text = $"{currentAmmo} / {maxAmmo}";
            }
        }
    }

    private void UpdateLaserColor()
    {
        if (laserLine == null) return;

        Color currentColor;

        if (currentAmmo <= lowAmmoThreshold)
        {
            currentColor = laserColorLow;
        }
        else if (currentAmmo <= maxAmmo / 2)
        {
            currentColor = laserColorMid;
        }
        else
        {
            currentColor = laserColorFull;
        }

        laserLine.startColor = currentColor;
        laserLine.endColor = currentColor;
    }

    private void ApplyRecoil()
    {
        if (gunModel != null && !isRecoiling)
        {
            StartCoroutine(RecoilCoroutine());
        }
    }

    private IEnumerator RecoilCoroutine()
    {
        isRecoiling = true;

        Vector3 recoilPos = gunOriginalLocalPos - Vector3.forward * recoilDistance;
        float elapsed = 0f;
        float halfDuration = recoilDuration / 2f;

        while (elapsed < halfDuration)
        {
            elapsed += Time.deltaTime;
            float t = elapsed / halfDuration;
            gunModel.localPosition = Vector3.Lerp(gunOriginalLocalPos, recoilPos, t);
            yield return null;
        }

        elapsed = 0f;
        while (elapsed < halfDuration)
        {
            elapsed += Time.deltaTime;
            float t = elapsed / halfDuration;
            float easedT = 1f - Mathf.Pow(1f - t, 2f);
            gunModel.localPosition = Vector3.Lerp(recoilPos, gunOriginalLocalPos, easedT);
            yield return null;
        }

        gunModel.localPosition = gunOriginalLocalPos;
        isRecoiling = false;
    }

    private void Update()
    {
        if (isReloading)
        {
            if (Time.time >= reloadEndTime)
            {
                isReloading = false;
                currentAmmo = maxAmmo;
                canShoot = true;
                UpdateAmmoUI();
                UpdateLaserColor();
            }
            HideLaser();
            return;
        }

        if (!canShoot && Time.time >= lastShootTime + shootCooldown)
        {
            canShoot = true;
        }

        float triggerValue = triggerAction.action.ReadValue<float>();
        bool isShooting = triggerValue > 0.5f;

        if (isShooting)
        {
            bool canFire = canShoot && !wasShooting && currentAmmo > 0;
            ShowLaser(canFire);

            if (canFire)
            {
                PlayShootSound();
                ApplyRecoil();
                lastShootTime = Time.time;
                canShoot = false;
                currentAmmo--;
                UpdateAmmoUI();
                UpdateLaserColor();

                if (currentAmmo <= 0)
                {
                    StartReload();
                }
            }
        }
        else
        {
            HideLaser();
        }

        wasShooting = isShooting;
    }

    private void StartReload()
    {
        isReloading = true;
        reloadEndTime = Time.time + reloadDuration;

        if (reloadSound != null && audioSource != null)
        {
            audioSource.PlayOneShot(reloadSound, reloadVolume);
        }

        UpdateAmmoUI();
        Debug.Log("Rechargement en cours...");
    }

    private void PlayShootSound()
    {
        if (shootSound != null && audioSource != null)
        {
            audioSource.PlayOneShot(shootSound, shootVolume);
        }
    }

    private void ShowLaser(bool canHitTargets = true)
    {
        if (laserLine == null || laserStartPoint == null) return;

        laserLine.enabled = true;

        laserLine.SetPosition(0, laserStartPoint.position);

        RaycastHit hit;
        if (Physics.Raycast(laserStartPoint.position, laserStartPoint.forward, out hit, laserMaxDistance))
        {
            laserLine.SetPosition(1, hit.point);

            if (canHitTargets)
            {
                Target target = hit.collider.GetComponent<Target>();
                if (target != null)
                {
                    target.OnHit();
                }

                BalloonTarget balloon = hit.collider.GetComponent<BalloonTarget>();
                if (balloon != null)
                {
                    balloon.OnHit();
                }

                UIButtonTarget uiButton = hit.collider.GetComponent<UIButtonTarget>();
                if (uiButton != null)
                {
                    uiButton.OnHit();
                }
            }
        }
        else
        {
            laserLine.SetPosition(1, laserStartPoint.position + laserStartPoint.forward * laserMaxDistance);
        }
    }

    private void HideLaser()
    {
        if (laserLine != null)
        {
            laserLine.enabled = false;
        }
    }
}
