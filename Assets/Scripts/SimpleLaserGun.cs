using UnityEngine;
using UnityEngine.InputSystem;
using TMPro;

public class SimpleLaserGun : MonoBehaviour
{
    [Header("Input Settings")]
    [SerializeField] private InputActionProperty triggerAction;

    [Header("Laser Settings")]
    [SerializeField] private LineRenderer laserLine;
    [SerializeField] private Transform laserStartPoint;
    [SerializeField] private float laserMaxDistance = 50f;
    [SerializeField] private Color laserColor = Color.red;
    [SerializeField] private float laserWidth = 0.02f;

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

    private void Start()
    {
        if (laserLine != null)
        {
            laserLine.startWidth = laserWidth;
            laserLine.endWidth = laserWidth;
            laserLine.material = new Material(Shader.Find("Sprites/Default"));
            laserLine.startColor = laserColor;
            laserLine.endColor = laserColor;
            laserLine.enabled = false;
        }

        audioSource = GetComponent<AudioSource>();
        if (audioSource == null)
        {
            audioSource = gameObject.AddComponent<AudioSource>();
        }
        audioSource.playOnAwake = false;

        currentAmmo = maxAmmo;
        UpdateAmmoUI();
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
                lastShootTime = Time.time;
                canShoot = false;
                currentAmmo--;
                UpdateAmmoUI();

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
