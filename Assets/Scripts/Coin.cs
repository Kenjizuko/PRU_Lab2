using Unity.Netcode;
using UnityEngine;

public class Coin : NetworkBehaviour
{
    [SerializeField] float coinValue = 10f;
    [SerializeField] private AudioClip coinSound;
    [SerializeField] ParticleSystem particle;
    private AudioSource audioSource;

    void Start()
    {
        audioSource = GameObject.FindWithTag("Player").GetComponent<AudioSource>();
    }


    void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player")) // Make sure Player has the "Player" tag
        {
            PlayerController player = other.GetComponent<PlayerController>();
            if (player != null && player.IsOwner)
            {
                ScoreManager.Instance.AddPoints(coinValue); // Add score
                if (audioSource != null && coinSound != null)
                {
                    audioSource.PlayOneShot(coinSound); // Correct usage   
                }
                Instantiate(particle, transform.position, Quaternion.identity);
                DestroyCoinServerRpc();
            }
        }
    }

    [ServerRpc(RequireOwnership = false)]
    private void DestroyCoinServerRpc()
    {
        // Destroy the coin on all clients
        DestroyCoinClientRpc();
    }

    [ClientRpc]
    private void DestroyCoinClientRpc()
    {
        // Destroy the coin locally on all clients
        Destroy(gameObject);
    }
}
