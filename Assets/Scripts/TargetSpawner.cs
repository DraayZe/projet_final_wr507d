using System.Collections.Generic;
using UnityEngine;

public class TargetSpawner : MonoBehaviour
{
    [Header("Spawn Settings")]
    [SerializeField] private GameObject balloonPrefab;
    [SerializeField] private int maxActiveTargets = 10;
    [SerializeField] private float respawnDelay = 0.5f;

    [Header("Spawn Zone")]
    [SerializeField] private Vector3 spawnAreaSize = new Vector3(5f, 3f, 1f);
    [SerializeField] private Vector3 spawnAreaCenter = Vector3.zero;

    [Header("Movement Settings")]
    [SerializeField] private float movingTargetPercentage = 0.3f;

    [Header("Debug")]
    [SerializeField] private bool showGizmos = true;

    private List<BalloonTarget> activeTargets = new List<BalloonTarget>();
    private Queue<BalloonTarget> targetPool = new Queue<BalloonTarget>();
    private bool gameStarted = false;

    private void Start()
    {
        InitializePool();
    }

    private void InitializePool()
    {
        for (int i = 0; i < maxActiveTargets * 2; i++)
        {
            CreateNewBalloon();
        }
    }

    private BalloonTarget CreateNewBalloon()
    {
        GameObject balloon = Instantiate(balloonPrefab, transform);
        balloon.SetActive(false);
        BalloonTarget target = balloon.GetComponent<BalloonTarget>();

        if (target == null)
        {
            target = balloon.AddComponent<BalloonTarget>();
        }

        targetPool.Enqueue(target);
        return target;
    }

    public void StartSpawning()
    {
        if (gameStarted) return;

        gameStarted = true;

        for (int i = 0; i < maxActiveTargets; i++)
        {
            SpawnTarget();
        }
    }

    public void StopSpawning()
    {
        gameStarted = false;

        foreach (BalloonTarget target in activeTargets)
        {
            if (target != null)
            {
                target.gameObject.SetActive(false);
                targetPool.Enqueue(target);
            }
        }

        activeTargets.Clear();
    }

    private void SpawnTarget()
    {
        if (targetPool.Count == 0)
        {
            CreateNewBalloon();
        }

        BalloonTarget target = targetPool.Dequeue();
        Vector3 randomPosition = GetRandomSpawnPosition();

        target.transform.position = randomPosition;
        target.gameObject.SetActive(true);

        bool shouldMove = Random.value < movingTargetPercentage;
        target.Initialize(this, shouldMove);

        activeTargets.Add(target);
    }

    private Vector3 GetRandomSpawnPosition()
    {
        Vector3 worldCenter = transform.position + spawnAreaCenter;

        float randomX = Random.Range(-spawnAreaSize.x / 2, spawnAreaSize.x / 2);
        float randomY = Random.Range(-spawnAreaSize.y / 2, spawnAreaSize.y / 2);
        float randomZ = Random.Range(-spawnAreaSize.z / 2, spawnAreaSize.z / 2);

        return worldCenter + new Vector3(randomX, randomY, randomZ);
    }

    public void OnTargetHit(BalloonTarget target)
    {
        activeTargets.Remove(target);
        target.gameObject.SetActive(false);
        targetPool.Enqueue(target);

        if (gameStarted)
        {
            Invoke(nameof(SpawnTarget), respawnDelay);
        }
    }

    private void OnDrawGizmos()
    {
        if (!showGizmos) return;

        Gizmos.color = Color.yellow;
        Vector3 worldCenter = transform.position + spawnAreaCenter;
        Gizmos.DrawWireCube(worldCenter, spawnAreaSize);
    }

    public void SetMaxTargets(int count)
    {
        maxActiveTargets = count;
    }
}
