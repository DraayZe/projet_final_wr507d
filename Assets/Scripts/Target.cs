using UnityEngine;

public class Target : MonoBehaviour
{
    [Header("Visual Feedback")]
    [SerializeField] private Material normalMaterial;
    [SerializeField] private Material hitMaterial;
    [SerializeField] private float flashDuration = 0.2f;

    private Renderer targetRenderer;
    private bool canBeHit = true;

    private void Start()
    {
        targetRenderer = GetComponent<Renderer>();
        if (targetRenderer != null && normalMaterial != null)
        {
            targetRenderer.material = normalMaterial;
        }
    }

    public void OnHit()
    {
        if (!canBeHit) return;

        if (GameManager.Instance != null)
        {
            GameManager.Instance.AddPoint();
        }

        if (targetRenderer != null && hitMaterial != null)
        {
            StartCoroutine(FlashHit());
        }

        Debug.Log("Cible touchée ! +1 point");
    }

    private System.Collections.IEnumerator FlashHit()
    {
        canBeHit = false;
        targetRenderer.material = hitMaterial;
        yield return new WaitForSeconds(flashDuration);
        targetRenderer.material = normalMaterial;
        canBeHit = true;
    }
}
