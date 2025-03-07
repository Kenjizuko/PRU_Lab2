using UnityEngine;
using Unity.Netcode;
using UnityEngine.SceneManagement;
using Unity.Netcode.Transports.UTP;
using TMPro;


public class Menu : MonoBehaviour
{
    public TMP_InputField ngrokInput; // Input field for Ngrok address
    private UnityTransport transport;
    private static string ipAddress = "127.0.0.1"; // Default IP for localhost
    private void Start()
    {
        transport = FindObjectOfType<UnityTransport>();
    }

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
            transport = FindObjectOfType<UnityTransport>();
            if (transport != null)
            {
                transport.SetConnectionData("0.0.0.0", 7777); // Host listens on all interfaces
                NetworkManager.Singleton.StartHost();
            }
            SceneManager.sceneLoaded -= OnGameSceneLoadedHost;
            Time.timeScale = 0f;
        }
    }

    public void OnJoinButtonClicked()
    {
        string ngrokAddress = ngrokInput.text; // Get IP from input field

        if (string.IsNullOrEmpty(ngrokAddress))
        {
            Debug.LogError("Ngrok address is empty! Enter a valid address.");
            return;
        }

        string[] splitAddress = ngrokAddress.Split(':');
        if (splitAddress.Length != 2)
        {
            Debug.LogError("Invalid Ngrok address format! Use '0.tcp.ngrok.io:12345'.");
            return;
        }

        string ip = splitAddress[0];
        if (!ushort.TryParse(splitAddress[1], out ushort port))
        {
            Debug.LogError("Invalid port! Ensure it's a number.");
            return;
        }

        SceneManager.LoadScene("Level1", LoadSceneMode.Single);
        SceneManager.sceneLoaded += (scene, mode) => OnGameSceneLoadedClient(scene, mode, ip, port);
    }

    private void OnGameSceneLoadedClient(Scene scene, LoadSceneMode mode, string ip, ushort port)
    {
        if (scene.name == "Level1")
        {
            transport = FindObjectOfType<UnityTransport>();
            if (transport != null)
            {
                Debug.Log($"Connecting to {ip}:{port}");
                transport.SetConnectionData(ip, port);
                NetworkManager.Singleton.StartClient();
            }
            SceneManager.sceneLoaded -= (s, m) => OnGameSceneLoadedClient(s, m, ip, port);
            Time.timeScale = 0f;
        }
    }

    public void QuitGame()
    {
        Application.Quit(); // Works in a build, not in the editor
        Debug.Log("Game Quit!"); // Debugging for editor
    }
}
