using UnityEngine;
using UnityEngine.UI;  // Import the UI namespace
using UnityEngine.EventSystems;  // For EventTrigger

public class ButtonSound : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler, IPointerClickHandler
{
    public AudioClip hoverSound;  // Drag hover sound into this field in Inspector
    public AudioClip clickSound;  // Drag click sound into this field in Inspector
    private AudioSource audioSource;  // To play the sounds

    void Start()
    {
        // Get the AudioSource component from the Button
        audioSource = GetComponent<AudioSource>();
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        // Play hover sound when mouse enters button area
        if (hoverSound != null)
        {
            audioSource.PlayOneShot(hoverSound);
        }
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        // Optionally, you can stop any sound when the pointer exits
        // audioSource.Stop(); // If you want to stop the hover sound immediately after exiting
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        // Play click sound when the button is clicked
        if (clickSound != null)
        {
            audioSource.PlayOneShot(clickSound);
        }
    }
}
