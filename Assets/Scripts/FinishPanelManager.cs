using UnityEngine;
using TMPro;

public class FinishPanelManager : MonoBehaviour
{
    public static FinishPanelManager Instance { get; private set; }

    [SerializeField] private GameObject finishPanel;
    [SerializeField] private TMP_Text scoreText;
    [SerializeField] private ParticleSystem finishParticle;
    [SerializeField] private AudioClip finishSound;
    private AudioSource audioSource;

    private void Awake()
    {
        if (Instance == null) Instance = this;
        else Destroy(gameObject);
        audioSource = GetComponent<AudioSource>();

    }

    public void ShowFinishPanel()
    {
        if (finishPanel != null)
        {
            finishParticle.Play();
            audioSource.Play();
            finishPanel.SetActive(true);
            DisplayScore();
        }
        else
        {
            Debug.LogError("Finish Panel is not assigned!");
        }
    }

    private void DisplayScore()
    {
        float finalScore = ScoreManager.Instance?.GetScore() ?? 0;
        Debug.Log($"Game Over! Final Score: {finalScore}");

        if (scoreText != null)
        {
            scoreText.text = "Your score: " + finalScore.ToString();
        }
        else
        {
            Debug.LogError("Score Text is not assigned!");
        }

        ScoreManager.Instance?.SaveHighScore();
    }
}
