using UnityEngine;
using Unity.Netcode;

public class FinishLine : NetworkBehaviour
{
    private bool hasFinished = false;

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Finish") && IsOwner && !hasFinished)
        {
            hasFinished = true;

            // Play effect and sound


            // Notify the server that this player has finished
            FinishGameServerRpc(OwnerClientId);

            DestroyPlayerServerRpc(OwnerClientId);
        }
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

    [ServerRpc(RequireOwnership = false)]
    private void FinishGameServerRpc(ulong clientId)
    {
        // Notify only the finishing player to show the finish panel
        ShowFinishPanelClientRpc(clientId);
    }

    [ClientRpc]
    private void ShowFinishPanelClientRpc(ulong clientId)
    {
        if (NetworkManager.Singleton.LocalClientId != clientId) return;

        Debug.Log($"Player {clientId} finished!");

        // Call FinishPanelManager to show UI
        FinishPanelManager.Instance?.ShowFinishPanel();
    }
}
