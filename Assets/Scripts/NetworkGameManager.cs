using UnityEngine;
using Unity.Netcode;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using System.Collections;
public class NetworkGameManager : NetworkBehaviour
{
    private bool isGameStarted = false;
    private NetworkVariable<bool> isGamePaused = new NetworkVariable<bool>(true, NetworkVariableReadPermission.Everyone, NetworkVariableWritePermission.Server);
    [SerializeField] private GameObject pauseMenu; // Reference to the pause menu UI
    [SerializeField] private Button resumeButton; // Reference to the resume button
    [SerializeField] private Button backToMenuButton;
    private void Update()
    {
        // Only the host can start the game
        if (IsHost && Input.GetKeyDown(KeyCode.Space) && !isGameStarted)
        {
            StartGame();
        }
        if (IsHost && Input.GetKeyDown(KeyCode.Escape))
        {
            TogglePauseGame();
        }

        // Update Time.timeScale and pause menu visibility based on the synchronized pause state
        Time.timeScale = isGamePaused.Value ? 0f : 1f;
        if (pauseMenu != null)
        {
            pauseMenu.SetActive(isGamePaused.Value);
        }

    }

    private void Start()
    {
        // Initialize the pause menu
        if (pauseMenu != null)
        {
            pauseMenu.SetActive(false); // Hide the pause menu initially
        }

        // Add listener 

        if (resumeButton != null)
        {
            resumeButton.onClick.AddListener(ResumeGame);
        }
        if (backToMenuButton != null)
        {
            backToMenuButton.onClick.AddListener(BackToMainMenu);
        }
    }

    private void TogglePauseGame()
    {
        // Toggle the pause state on the server
        isGamePaused.Value = !isGamePaused.Value;
        pauseMenu.SetActive(true);
    }

    private void ResumeGame()
    {
        // Only the host can resume the game
        if (IsHost)
        {
            isGamePaused.Value = false;
            pauseMenu.SetActive(false);
        }
    }

    private void StartGame()
    {
        isGameStarted = true;

        // Unpause the game
        isGamePaused.Value = false;
    }

    private void BackToMainMenu()
    {
        // Only the host can return to the main menu
        if (IsHost)
        {
            NetworkManager.Singleton.SceneManager.LoadScene("Menu", LoadSceneMode.Single);
        }
    }
}
