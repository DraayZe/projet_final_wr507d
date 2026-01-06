using UnityEngine;
using TMPro;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance { get; private set; }

    [Header("Game Settings")]
    [SerializeField] private float gameDuration = 60f;

    [Header("UI")]
    [SerializeField] private TextMeshProUGUI scoreText;
    [SerializeField] private TextMeshProUGUI timerText;
    [SerializeField] private GameObject gameOverPanel;
    [SerializeField] private TextMeshProUGUI finalScoreText;
    [SerializeField] private TextMeshProUGUI lastScoreText;

    [Header("Audio")]
    [SerializeField] private AudioSource musicSource;

    [Header("Target Spawning")]
    [SerializeField] private TargetSpawner targetSpawner;

    private int score = 0;
    private int lastScore = 0;
    private float timeRemaining;
    private bool gameActive = false;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            Destroy(gameObject);
        }
    }

    private void Start()
    {
        timeRemaining = gameDuration;
        UpdateUI();

        if (gameOverPanel != null)
        {
            gameOverPanel.SetActive(false);
        }
    }

    private void Update()
    {
        if (gameActive)
        {
            timeRemaining -= Time.deltaTime;

            if (timeRemaining <= 0)
            {
                timeRemaining = 0;
                EndGame();
            }

            UpdateUI();
        }
    }

    public void StartGame()
    {
        if (gameActive) return;

        gameActive = true;
        score = 0;
        timeRemaining = gameDuration;

        if (musicSource != null)
        {
            musicSource.Play();
        }

        if (gameOverPanel != null)
        {
            gameOverPanel.SetActive(false);
        }

        if (lastScoreText != null && lastScore > 0)
        {
            lastScoreText.text = $"Dernier score : {lastScore}";
        }

        if (targetSpawner != null)
        {
            targetSpawner.StartSpawning();
        }

        Debug.Log("Jeu démarré !");
        UpdateUI();
    }

    public void AddPoint()
    {
        if (!gameActive) return;

        score++;
        UpdateUI();
    }

    private void EndGame()
    {
        gameActive = false;
        lastScore = score;

        if (musicSource != null)
        {
            musicSource.Stop();
        }

        if (gameOverPanel != null)
        {
            gameOverPanel.SetActive(true);
        }

        if (finalScoreText != null)
        {
            finalScoreText.text = $"Score final : {score}";
        }

        if (lastScoreText != null)
        {
            lastScoreText.text = $"Dernier score : {lastScore}";
        }

        if (targetSpawner != null)
        {
            targetSpawner.StopSpawning();
        }

        Debug.Log($"Jeu terminé ! Score final : {score}");
    }

    private void UpdateUI()
    {
        if (scoreText != null)
        {
            scoreText.text = $"Score: {score}";
        }

        if (timerText != null)
        {
            int minutes = Mathf.FloorToInt(timeRemaining / 60);
            int seconds = Mathf.FloorToInt(timeRemaining % 60);
            timerText.text = string.Format("{0:00}:{1:00}", minutes, seconds);
        }
    }

    public void RestartGame()
    {
        gameActive = false;
        score = 0;
        timeRemaining = gameDuration;

        if (musicSource != null)
        {
            musicSource.Stop();
        }

        if (gameOverPanel != null)
        {
            gameOverPanel.SetActive(false);
        }

        UpdateUI();
        StartGame();
    }
}
