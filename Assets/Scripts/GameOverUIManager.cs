using UnityEngine;
using TMPro;
using Unity.Netcode;
using UnityEngine.SceneManagement;

public class GameOverUIManager : MonoBehaviour
{
    public static GameOverUIManager Instance;

    [SerializeField] private GameObject gameOverPanel; // Reference to the Game Over Panel
    [SerializeField] private TMP_Text finalScoreText; // Reference to the Final Score Text
    [SerializeField] ParticleSystem particle;

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

    public void OnBackToMainMenuClicked()
    {
        Debug.Log("Returning to Main Menu...");

        // Stop the host if running in multiplayer
        if (NetworkManager.Singleton != null && NetworkManager.Singleton.IsHost)
        {
            Debug.Log("Stopping host...");
            NetworkManager.Singleton.Shutdown(); // Shutdown the network manager
        }

        // Load the main menu scene
        SceneManager.LoadScene("Menu"); 
    }

    public void ShowGameOver(float finalScore)
    {
        Debug.Log("Game Over");
        particle.Stop(true, ParticleSystemStopBehavior.StopEmittingAndClear);
        gameOverPanel.SetActive(true); // Activate the Game Over Panel
        finalScoreText.text = $"Final Score: {finalScore}"; // Display the final score
        
    }
}