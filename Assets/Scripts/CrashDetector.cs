using UnityEngine;
using Unity.Netcode;

public class CrashDetector : NetworkBehaviour
{
    private bool isGameOver = false;

   

    void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Ground") && IsOwner && !isGameOver)
        {
            HandleGameOver();
        }
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Obstacle") && IsOwner && !isGameOver)
        {
            HandleGameOver();
        }
    }

    private void HandleGameOver()
    {
        isGameOver = true;

        // Save the player's high score
        ScoreManager.Instance?.SaveHighScore();

        // Display the player's score
        DisplayScore();

        // Disable player controls (optional)
        GetComponent<PlayerController>().SetGameOver();

        // Notify the server that this player is game over
        GameOverServerRpc(OwnerClientId);

        DestroyPlayerServerRpc(OwnerClientId);
    }

    [ServerRpc]
    private void DestroyPlayerServerRpc(ulong clientId)
    {
        // Notify all clients to destroy the player object
        DestroyPlayerClientRpc(clientId);
    }

    [ClientRpc]
    private void DestroyPlayerClientRpc(ulong clientId)
    {
        if (clientId == OwnerClientId)
        {
            Debug.Log($"Destroying player {clientId}.");
            Destroy(gameObject); // Destroy the player object
        }
    }

    [ServerRpc]
    private void GameOverServerRpc(ulong clientId)
    {
        // Notify all clients that this player is game over
        GameOverClientRpc(clientId);
    }

    [ClientRpc]
    private void GameOverClientRpc(ulong clientId)
    {
        if (clientId == OwnerClientId)
        {
            Debug.Log($"Player {clientId} is game over.");
        }
    }

    private void DisplayScore()
    {
        if (IsOwner)
        {
            float finalScore = ScoreManager.Instance?.GetScore() ?? 0;
            Debug.Log($"Game Over! Final Score: {finalScore}");

            // Show the Game Over UI
            GameOverUIManager.Instance?.ShowGameOver(finalScore);
        }
    }

}