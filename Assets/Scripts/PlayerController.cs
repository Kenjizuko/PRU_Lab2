
using Unity.Cinemachine;
using Unity.Netcode;
using UnityEngine;


public class PlayerController : NetworkBehaviour
{
    [SerializeField] float torque = 1f;
    [SerializeField] float boostSpeed = 30f;
    [SerializeField] float baseSpeed = 20f;
    [SerializeField] float rotationDamping = 0.95f;
    [SerializeField] TrailRenderer windTrail;
    [SerializeField] float flipAngleThreshold = 180f;
    [SerializeField] private AudioSource coinSound;

    [Header("Boost Settings")]
    [SerializeField] float maxBoostEnergy = 5f; // Total boost energy
    [SerializeField] float boostConsumptionRate = 1f; // Energy consumed per second
    [SerializeField] float boostRechargeRate = 0.5f; // Energy recharged per second
    private float currentBoostEnergy; // Current boost energy

    Rigidbody2D rb2d;
    SurfaceEffector2D surfaceEffector2D;
    bool isInAir = false;
    float lastRotation = 0f;
    float rotationAccumulated = 0f;
    int comboMultiplier = 1;
    private bool isGameOver = false;


    public float GetCurrentBoostEnergy() => currentBoostEnergy;
    public float GetMaxBoostEnergy() => maxBoostEnergy;

    public override void OnNetworkSpawn()
    {
        if (IsOwner) // Only execute for the local player
        {
            CinemachineVirtualCamera cam = FindObjectOfType<CinemachineVirtualCamera>();
            if (cam != null)
            {
                cam.Follow = transform;
            }
            // Initialize boost energy
            currentBoostEnergy = maxBoostEnergy;

            // Register the player with the BoostUIManager
            BoostUIManager.Instance?.RegisterPlayer(this);
        }
        IgnorePlayerCollisions();
    }

    void Start()
    {
        rb2d = GetComponent<Rigidbody2D>();
        surfaceEffector2D = FindObjectOfType<SurfaceEffector2D>();
        if (IsServer) // Only the server handles collision ignoring
        {
            IgnorePlayerCollisions();
        }
    }

    private void IgnorePlayerCollisions()
    {
        GameObject[] players = GameObject.FindGameObjectsWithTag("Player");
        Debug.Log($"Found {players.Length} players in the scene.");

        foreach (var p1 in players)
        {
            foreach (var p2 in players)
            {
                if (p1 != p2)
                {
                    CapsuleCollider2D col1 = p1.GetComponent<CapsuleCollider2D>();
                    CapsuleCollider2D col2 = p2.GetComponent<CapsuleCollider2D>();

                    if (col1 != null && col2 != null)
                    {
                        Physics2D.IgnoreCollision(col1, col2);
                        Debug.Log($"Disabled collisions between {p1.name} and {p2.name}");
                    }
                }
            }
        }
    }

    void Update()
    {
        if (!IsOwner || isGameOver) return;
        if (!IsOwner) return;
        Rotate();
        RespondToBoost();
        BoostUIManager.Instance?.UpdateBoostUI(); // Update the boost UI

    }
    public void SetGameOver()
    {
        isGameOver = true;
        enabled = false; // Disable player controls
    }

    void RespondToBoost()
    {
        
            if (Input.GetKey(KeyCode.Space))
            {
                if (currentBoostEnergy > 0)
                {
                    // Consume boost energy
                    currentBoostEnergy -= boostConsumptionRate * Time.deltaTime;
                    currentBoostEnergy = Mathf.Max(currentBoostEnergy, 0); // Clamp to 0

                    // Apply boost
                    surfaceEffector2D.speed = boostSpeed;
                    windTrail.emitting = true;
                }
                else
                {
                    // Out of boost energy
                    surfaceEffector2D.speed = baseSpeed;
                    windTrail.emitting = false;
                }
            }
            else
            {
                // Recharge boost energy
                currentBoostEnergy += boostRechargeRate * Time.deltaTime;
                currentBoostEnergy = Mathf.Min(currentBoostEnergy, maxBoostEnergy); // Clamp to max

                // Reset to base speed
                surfaceEffector2D.speed = baseSpeed;
                windTrail.emitting = false;
            }
        
    }

    void Rotate()
    {
        float rotationInput = Input.GetAxisRaw("Horizontal"); // -1 (left), 1 (right), 0 (no input)
        rb2d.AddTorque(-rotationInput * torque); // Apply torque based on input

        if (rotationInput == 0)
        {
            rb2d.angularVelocity *= rotationDamping; // Smoothly stop rotation when no input
        }

        if (isInAir)
        {
            TrackFlipRotation();
        }
    }

    void TrackFlipRotation()
    {
        float currentRotation = rb2d.rotation;
        float rotationDelta = currentRotation - lastRotation;

        // Handle wrapping around 360 degrees
        if (rotationDelta > 180) rotationDelta -= 360;
        if (rotationDelta < -180) rotationDelta += 360;

        rotationAccumulated += rotationDelta;
        lastRotation = currentRotation;

        // Check for a full flip (360 degrees in either direction)
        if (Mathf.Abs(rotationAccumulated) >= flipAngleThreshold)
        {
            ScoreFlip();
            rotationAccumulated = 0f; // Reset for the next flip
        }
    }

    void ScoreFlip()
    {
        int scoreToAdd = 20 * comboMultiplier;
        ScoreManager.Instance?.AddPoints(scoreToAdd);
        Debug.Log($"Flip scored! +{scoreToAdd} points (Combo x{comboMultiplier})");
        coinSound.Play(); // Play sound effect
        comboMultiplier++; // Increase combo multiplier for consecutive flips
    }

    void OnCollisionEnter2D(Collision2D collision)
    {
        if (isInAir)
        {
            if (rotationAccumulated != 0f)
            {
                Debug.Log("Landing without a full flip. Resetting combo.");
            }
            ResetFlipTracking();
        }
    }




    void OnCollisionExit2D(Collision2D collision)
    {
        isInAir = true;
        lastRotation = rb2d.rotation; // Reset rotation tracking when jumping
    }


    void ResetFlipTracking()
    {
        isInAir = false;
        rotationAccumulated = 0f;
        comboMultiplier = 1; // Reset combo multiplier on landing
    }

    void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Coin")) // Check if the collided object is a coin
        {
            CollectCoin(collision.gameObject);
        }
    }

    private void CollectCoin(GameObject coin)
    {
        if (IsOwner) // Only the owner can collect coins
        {
            // Add points to the player's score in the ScoreManager
            if (IsServer)
            {
                ScoreManager.Instance?.AddPoints(10f);
            }
            else
            {
                CollectCoinServerRpc();
            }
        }
    }

    [ServerRpc(RequireOwnership = false)]
    private void CollectCoinServerRpc()
    {
        ScoreManager.Instance?.AddPoints(10f);
    }

}
