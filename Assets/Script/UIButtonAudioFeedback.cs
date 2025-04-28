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


        if (audioSource.isPlaying)
        {
            audioSource.Stop();
        }


        if (hoverClip != null)
        {
            audioSource.PlayOneShot(hoverClip);
        }

        if (voiceClip != null)
        {
            audioSource.PlayOneShot(voiceClip);
        }
    }
}