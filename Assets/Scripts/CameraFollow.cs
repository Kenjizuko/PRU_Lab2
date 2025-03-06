using UnityEngine;
using Unity.Netcode;
using Unity.Cinemachine;
using System.Collections;

public class CameraFollow : MonoBehaviour
{
    private CinemachineVirtualCamera virtualCam;

    private void Start()
    {
        virtualCam = GetComponent<CinemachineVirtualCamera>();
        StartCoroutine(WaitForLocalPlayer());
    }

    private IEnumerator WaitForLocalPlayer()
    {
        GameObject localPlayer = null;

        // Keep checking until the local player is spawned
        while (localPlayer == null)
        {
            foreach (var player in GameObject.FindGameObjectsWithTag("Player"))
            {
                // Get NetworkObject component and check if this is the local player
                var networkObject = player.GetComponent<NetworkObject>();
                if (networkObject != null && networkObject.IsOwner)
                {
                    localPlayer = player;
                    break; // Stop searching once we find the local player
                }
            }
            yield return null; // Wait for next frame
        }

        // Assign the camera to follow the local player
        virtualCam.Follow = localPlayer.transform;
    }
}
