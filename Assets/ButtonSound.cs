using UnityEngine;
using UnityEngine.UI;

public class ButtonSound : MonoBehaviour
{
    public Button myButton;
    public AudioSource audioSource;
    public AudioClip clickSound;

    void Start()
    {
        if (myButton != null)
            myButton.onClick.AddListener(PlaySound);
    }

    void PlaySound()
    {
        if (audioSource != null && clickSound != null)
            audioSource.PlayOneShot(clickSound);
    }
}
