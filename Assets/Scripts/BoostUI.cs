using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class BoostUIManager : MonoBehaviour
{
    public static BoostUIManager Instance;

    [SerializeField] private Slider boostSlider; // Reference to the UI Slider


    private PlayerController playerController; // Reference to the player's controller

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

    public void RegisterPlayer(PlayerController player)
    {
        playerController = player;
        UpdateBoostUI();
    }

    public void UpdateBoostUI()
    {
        if (playerController != null)
        {
            float currentBoostEnergy = playerController.GetCurrentBoostEnergy();
            float maxBoostEnergy = playerController.GetMaxBoostEnergy();

            if (boostSlider != null)
            {
                boostSlider.value = currentBoostEnergy / maxBoostEnergy; // Update slider value
            }

        }
    }
}