using UnityEngine;

public class UIFollowPlayer : MonoBehaviour
{
    [Header("Follow Settings")]
    [SerializeField] private Transform playerCamera;
    [SerializeField] private Vector3 offset = new Vector3(0, 0.5f, 2);
    [SerializeField] private bool followRotation = true;

    private void Update()
    {
        if (playerCamera == null) return;

        transform.position = playerCamera.position + playerCamera.forward * offset.z + playerCamera.up * offset.y + playerCamera.right * offset.x;

        if (followRotation)
        {
            transform.rotation = Quaternion.LookRotation(transform.position - playerCamera.position);
        }
    }
}
