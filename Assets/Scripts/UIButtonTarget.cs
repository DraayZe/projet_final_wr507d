using UnityEngine;
using UnityEngine.Events;

public class UIButtonTarget : MonoBehaviour
{
    [Header("Visual Feedback")]
    [SerializeField] private Material normalMaterial;
    [SerializeField] private Material hitMaterial;
    [SerializeField] private float flashDuration = 0.2f;

    [Header("Button Action")]
    [SerializeField] private UnityEvent onButtonHit;

    private Renderer buttonRenderer;
    private bool canBeHit = true;

    private void Start()
    {
        buttonRenderer = GetComponent<Renderer>();
        if (buttonRenderer != null && normalMaterial != null)
        {
            buttonRenderer.material = normalMaterial;
        }
    }

    public void OnHit()
    {
        if (!canBeHit) return;

        onButtonHit?.Invoke();

        if (buttonRenderer != null && hitMaterial != null)
        {
            StartCoroutine(FlashHit());
        }

        Debug.Log("Bouton activé par le laser !");
    }

    private System.Collections.IEnumerator FlashHit()
    {
        canBeHit = false;
        buttonRenderer.material = hitMaterial;
        yield return new WaitForSeconds(flashDuration);
        buttonRenderer.material = normalMaterial;
        canBeHit = true;
    }
}
