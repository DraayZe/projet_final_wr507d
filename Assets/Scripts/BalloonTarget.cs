using UnityEngine;
using System.Collections;

public class BalloonTarget : MonoBehaviour
{
    [Header("Pop Settings")]
    [SerializeField] private GameObject popEffect;
    [SerializeField] private AudioClip popSound;
    [SerializeField] private float popEffectDuration = 1f;

    [Header("Inflate Before Pop")]
    [SerializeField] private float inflateScale = 1.3f;
    [SerializeField] private float inflateDuration = 0.15f;

    [Header("Movement Settings")]
    [SerializeField] private bool isMoving = false;
    [SerializeField] private float moveSpeed = 1f;
    [SerializeField] private float moveRange = 2f;
    [SerializeField] private MovementType movementType = MovementType.UpDown;

    private Vector3 startPosition;
    private Vector3 originalScale;
    private float moveTimer = 0f;
    private bool canBeHit = true;
    private TargetSpawner spawner;

    public enum MovementType
    {
        UpDown,
        LeftRight,
        Circle,
        Random
    }

    private void Start()
    {
        startPosition = transform.position;
        originalScale = transform.localScale;
    }

    private void Update()
    {
        if (isMoving)
        {
            MoveTarget();
        }
    }

    public void Initialize(TargetSpawner targetSpawner, bool shouldMove)
    {
        spawner = targetSpawner;
        isMoving = shouldMove;
        startPosition = transform.position;
        canBeHit = true;
        moveTimer = Random.Range(0f, 100f);

        if (originalScale == Vector3.zero)
        {
            originalScale = transform.localScale;
        }
    }

    public void OnHit()
    {
        if (!canBeHit) return;

        canBeHit = false;
        isMoving = false;

        if (GameManager.Instance != null)
        {
            GameManager.Instance.AddPoint();
        }

        StartCoroutine(InflateAndPop());
    }

    private IEnumerator InflateAndPop()
    {
        Vector3 targetScale = originalScale * inflateScale;
        float elapsed = 0f;

        while (elapsed < inflateDuration)
        {
            elapsed += Time.deltaTime;
            float t = elapsed / inflateDuration;
            float easedT = 1f - Mathf.Pow(1f - t, 3f);
            transform.localScale = Vector3.Lerp(originalScale, targetScale, easedT);
            yield return null;
        }

        if (popSound != null)
        {
            AudioSource.PlayClipAtPoint(popSound, transform.position);
        }

        if (popEffect != null)
        {
            GameObject effect = Instantiate(popEffect, transform.position, Quaternion.identity);
            Destroy(effect, popEffectDuration);
        }

        if (spawner != null)
        {
            spawner.OnTargetHit(this);
        }

        transform.localScale = originalScale;

        Debug.Log("Ballon éclaté ! +1 point");
    }

    private void MoveTarget()
    {
        moveTimer += Time.deltaTime * moveSpeed;

        switch (movementType)
        {
            case MovementType.UpDown:
                float yOffset = Mathf.Sin(moveTimer) * moveRange;
                transform.position = startPosition + Vector3.up * yOffset;
                break;

            case MovementType.LeftRight:
                float xOffset = Mathf.Sin(moveTimer) * moveRange;
                transform.position = startPosition + Vector3.right * xOffset;
                break;

            case MovementType.Circle:
                float x = Mathf.Cos(moveTimer) * moveRange;
                float y = Mathf.Sin(moveTimer) * moveRange;
                transform.position = startPosition + new Vector3(x, y, 0);
                break;

            case MovementType.Random:
                float rx = Mathf.PerlinNoise(moveTimer, 0f) * moveRange * 2 - moveRange;
                float ry = Mathf.PerlinNoise(0f, moveTimer) * moveRange * 2 - moveRange;
                transform.position = startPosition + new Vector3(rx, ry, 0);
                break;
        }
    }

    public void ResetTarget()
    {
        canBeHit = true;
        moveTimer = Random.Range(0f, 100f);
        transform.localScale = originalScale;
    }
}
