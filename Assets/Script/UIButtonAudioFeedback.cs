using UnityEngine;
using UnityEngine.EventSystems;

public class UIButtonAudioFeedback : MonoBehaviour, ISelectHandler
{
    [Header("Son spécifique à ce bouton")]
    public AudioClip voiceClip;

    [Header("Audio commun (assigné par UIManager)")]
    public AudioSource audioSource;
    public AudioClip hoverClip;

    public void OnSelect(BaseEventData eventData)
    {
        if (audioSource == null) return;

        // Jouer le son de sélection commun
        if (hoverClip != null)
        {
            audioSource.PlayOneShot(hoverClip);
        }

        // Jouer la voix associée si dispo
        if (voiceClip != null)
        {
            audioSource.PlayOneShot(voiceClip);
        }
    }
}
