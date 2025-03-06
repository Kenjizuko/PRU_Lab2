using UnityEngine;
using UnityEngine.UI;

public class PopupManager : MonoBehaviour
{
    public GameObject controlsPopup; // Assign the ControlsPopup Panel in the Inspector
    public Button controlButton;
    public Button closeButton;

    void Start()
    {
        controlsPopup.SetActive(false); // Hide the popup at start

        controlButton.onClick.AddListener(ShowPopup);
        closeButton.onClick.AddListener(HidePopup);
    }

    void ShowPopup()
    {
        controlsPopup.SetActive(true);
    }

    void HidePopup()
    {
        controlsPopup.SetActive(false);
    }
}
