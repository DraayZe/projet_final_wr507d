using UnityEngine;
using UnityEngine.InputSystem;

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
    [SerializeField] private float shootVolume = 0.5f;

    private AudioSource audioSource;
    private bool wasShooting = false;

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
    }

    private void Update()
    {
        float triggerValue = triggerAction.action.ReadValue<float>();
        bool isShooting = triggerValue > 0.5f;

        if (isShooting)
        {
            if (!wasShooting)
            {
                PlayShootSound();
            }
            ShowLaser();
        }
        else
        {
            HideLaser();
        }

        wasShooting = isShooting;
    }

    private void PlayShootSound()
    {
        if (shootSound != null && audioSource != null)
        {
            audioSource.PlayOneShot(shootSound, shootVolume);
        }
    }

    private void ShowLaser()
    {
        if (laserLine == null || laserStartPoint == null) return;

        laserLine.enabled = true;

        laserLine.SetPosition(0, laserStartPoint.position);

        RaycastHit hit;
        if (Physics.Raycast(laserStartPoint.position, laserStartPoint.forward, out hit, laserMaxDistance))
        {
            laserLine.SetPosition(1, hit.point);

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
