using UnityEngine;
using System.Collections.Generic;

public class NarratorManager : MonoBehaviour
{
    public AudioSource narratorAudio;
    public AudioSource sfxAudio; // Source pour les SFX
    public AudioClip badClickSFX;
    public AudioClip goodClickSFX;


    public TextAsset scriptJSON;
    private List<RhythmAction> actions;
    private float timer;

    void Start()
    {
        RhythmActionList list = JsonUtility.FromJson<RhythmActionList>(scriptJSON.text);
        actions = list.actions;
        narratorAudio.Play();
    }

    void Update()
    {
        timer = narratorAudio.time;

        foreach (var action in actions)
        {
            if (!action.hasTriggered && timer >= action.startTime && timer <= action.endTime)
            {
                CheckInput(action);
            }
            else if (!action.hasTriggered && timer > action.endTime)
            {
                // Le joueur a raté l’action (trop tard)
                MissedAction(action);
            }
        }
    }

    void CheckInput(RhythmAction action)
    {
        if (Input.GetMouseButtonDown(0))
        {
            if (action.expectedClick == "Gauche")
                TriggerSuccess(action);
            else
                TriggerFail(action);
        }
        else if (Input.GetMouseButtonDown(1))
        {
            if (action.expectedClick == "Droite")
                TriggerSuccess(action);
            else
                TriggerFail(action);
        }
    }

    void TriggerSuccess(RhythmAction action)
    {
        Debug.Log($"Réussi : {action.keyword} avec {action.expectedClick}");
        action.hasTriggered = true;
        PlayGoodSound();
    }

    void TriggerFail(RhythmAction action)
    {
        Debug.Log($"Mauvais bouton pour {action.keyword}");
        action.hasTriggered = true;
        PlayBadSound();
    }

    void MissedAction(RhythmAction action)
    {
        Debug.Log($"Action ratée pour {action.keyword}");
        action.hasTriggered = true;
        PlayBadSound();
    }

    void PlayBadSound()
    {
        if (badClickSFX != null && sfxAudio != null)
        {
            sfxAudio.PlayOneShot(badClickSFX);
        }
    }
    void PlayGoodSound()
    {
        if (badClickSFX != null && sfxAudio != null)
        {
            sfxAudio.PlayOneShot(goodClickSFX);
        }
    }
}

[System.Serializable]
public class RhythmAction
{
    public string keyword;
    public float startTime;
    public float endTime;
    public string actionType;
    public string expectedClick; // "Gauche" ou "Droite"
    [System.NonSerialized] public bool hasTriggered = false;
}

[System.Serializable]
public class RhythmActionList
{
    public List<RhythmAction> actions;
}