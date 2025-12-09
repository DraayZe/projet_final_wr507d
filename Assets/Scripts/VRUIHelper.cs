using UnityEngine;

public class VRUIHelper : MonoBehaviour
{
    [Header("VR UI Settings")]
    [SerializeField] private Transform cameraTransform;
    [SerializeField] private bool followCamera = false;
    [SerializeField] private float distanceFromCamera = 2f;
    [SerializeField] private Vector3 offset = new Vector3(0, 0.5f, 0);

    private void Start()
    {
        // Trouver la cam\u00e9ra principale si non assign\u00e9e
        if (cameraTransform == null)
        {
            Camera mainCamera = Camera.main;
            if (mainCamera != null)
            {
                cameraTransform = mainCamera.transform;
            }
        }

        // Positionner l'UI devant la cam\u00e9ra au d\u00e9marrage
        if (cameraTransform != null)
        {
            PositionUI();
        }
    }

    private void LateUpdate()
    {
        if (followCamera && cameraTransform != null)
        {
            PositionUI();
        }
    }

    private void PositionUI()
    {
        // Positionner l'UI devant la cam\u00e9ra
        Vector3 targetPosition = cameraTransform.position + cameraTransform.forward * distanceFromCamera + offset;
        transform.position = targetPosition;

        // Faire face \u00e0 la cam\u00e9ra
        transform.LookAt(cameraTransform);
        transform.Rotate(0, 180, 0); // Inverser pour que le texte soit lisible
    }
}
