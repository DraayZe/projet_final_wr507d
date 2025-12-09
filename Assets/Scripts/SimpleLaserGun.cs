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
    }

    private void Update()
    {
        float triggerValue = triggerAction.action.ReadValue<float>();

        if (triggerValue > 0.5f)
        {
            ShowLaser();
        }
        else
        {
            HideLaser();
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
