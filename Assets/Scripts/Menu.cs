using UnityEngine;
using Unity.Netcode;
using UnityEngine.SceneManagement;

public class Menu : MonoBehaviour
{

    public void OnHostButtonClicked()
    {
        // Load the GameScene first
        SceneManager.LoadScene("Level1", LoadSceneMode.Single);

        // Once the scene is loaded, start the host
        SceneManager.sceneLoaded += OnGameSceneLoadedHost;
    }

    private void OnGameSceneLoadedHost(Scene scene, LoadSceneMode mode)
    {
        if (scene.name == "Level1")
        {
            // Start the host
            NetworkManager.Singleton.StartHost();

            // Unsubscribe from the event to avoid multiple calls
            SceneManager.sceneLoaded -= OnGameSceneLoadedHost;

            // Pause the game until the host decides to start
            Time.timeScale = 0f;
        }
    }

    public void OnJoinButtonClicked()
    {
        // Load the GameScene first
        SceneManager.LoadScene("Level1", LoadSceneMode.Single);

        // Once the scene is loaded, start the client
        SceneManager.sceneLoaded += OnGameSceneLoadedClient;
    }

    private void OnGameSceneLoadedClient(Scene scene, LoadSceneMode mode)
    {
        if (scene.name == "Level1")
        {
            // Start the client
            NetworkManager.Singleton.StartClient();

            // Unsubscribe from the event to avoid multiple calls
            SceneManager.sceneLoaded -= OnGameSceneLoadedClient;

            // Pause the game until the host starts
            Time.timeScale = 0f;
        }
    }

    public void QuitGame()
    {
        Application.Quit(); // Works in a build, not in the editor
        Debug.Log("Game Quit!"); // Debugging for editor
    }
}
