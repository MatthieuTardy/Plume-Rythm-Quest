using UnityEngine;
using UnityEngine.EventSystems;

public class UIButtonAudioFeedback : MonoBehaviour, ISelectHandler
{
    [Header("Son sp�cifique � ce bouton")]
    public AudioClip voiceClip;

    [Header("Audio commun (assign� par UIManager)")]
    public AudioSource audioSource;
    public AudioClip hoverClip;

    public void OnSelect(BaseEventData eventData)
    {
        if (audioSource == null) return;

        // Jouer le son de s�lection commun
        if (hoverClip != null)
        {
            audioSource.PlayOneShot(hoverClip);
        }

        // Jouer la voix associ�e si dispo
        if (voiceClip != null)
        {
            audioSource.PlayOneShot(voiceClip);
        }
    }
}
