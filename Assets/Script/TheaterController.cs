using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.InputSystem;

public class NarratorManager : MonoBehaviour
{
    [Header("Audio")]
    public AudioSource narratorAudio;
    public AudioSource tutoAudio;
    public AudioSource sfxAudio;
    public AudioClip badClickSFX;
    public AudioClip goodClickSFX;

    [Header("Input Actions")]
    public InputActionReference submitAction;  // pour "Gauche"
    public InputActionReference cancelAction;  // pour "Droite"

    private AudioClip narratorClip;
    private AudioClip narratorTutoClip;

    private int totalActions = 0;
    private int successCount = 0;
    private int failCount = 0;
    private bool hasFinished = false;
    private bool tutoEnd = false;

    public TextAsset scriptJSON;
    private List<RhythmAction> actions;
    public string audioClip;
    public string tutoClip;
    private float timer;

    void Start()
    {
        // Activer les actions Input System
        submitAction.action.Enable();
        cancelAction.action.Enable();

        // Charger le JSON
        RhythmActionList list = JsonUtility.FromJson<RhythmActionList>(scriptJSON.text);
        actions = list.actions;
        totalActions = actions.Count;
        tutoClip = list.tutoClip;
        audioClip = list.audioClip;

        narratorTutoClip = Resources.Load<AudioClip>(tutoClip);
        narratorClip = Resources.Load<AudioClip>(audioClip);

        tutoAudio.clip = narratorTutoClip;
        narratorAudio.clip = narratorClip;

        tutoAudio.Play();
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
                MissedAction(action);
            }
        }

        // Passer du tuto au gameplay
        if ((tutoAudio.time >= tutoAudio.clip.length || submitAction.action.WasPressedThisFrame()) && !tutoEnd)
        {
            tutoAudio.Stop();
            tutoEnd = true;
            narratorAudio.Play();
        }

        if (!hasFinished && narratorAudio.time >= narratorAudio.clip.length)
        {
            hasFinished = true;
            EvaluatePerformance();
        }
    }

    void CheckInput(RhythmAction action)
    {
        if (submitAction.action.WasPressedThisFrame())
        {
            if (action.expectedClick == "Gauche")
                TriggerSuccess(action);
            else
                TriggerFail(action);
        }
        else if (cancelAction.action.WasPressedThisFrame())
        {
            if (action.expectedClick == "Droite")
                TriggerSuccess(action);
            else
                TriggerFail(action);
        }
    }

    void TriggerSuccess(RhythmAction action)
    {
        Debug.Log($"✅ Réussi : {action.keyword} avec {action.expectedClick}");
        action.hasTriggered = true;
        successCount++;
        PlayGoodSound();
    }

    void TriggerFail(RhythmAction action)
    {
        Debug.Log($"❌ Mauvais bouton pour {action.keyword}");
        action.hasTriggered = true;
        failCount++;
        StartCoroutine(PausePlay());
    }

    void MissedAction(RhythmAction action)
    {
        Debug.Log($"⏱ Action ratée pour {action.keyword}");
        action.hasTriggered = true;
        failCount++;
        StartCoroutine(PausePlay());
    }

    public IEnumerator PausePlay()
    {
        PlayBadSound();
        narratorAudio.Pause();
        yield return new WaitForSecondsRealtime(1f);
        narratorAudio.Play();
    }

    void PlayBadSound()
    {
        if (badClickSFX != null && sfxAudio != null)
            sfxAudio.PlayOneShot(badClickSFX);
    }

    void PlayGoodSound()
    {
        if (goodClickSFX != null && sfxAudio != null)
            sfxAudio.PlayOneShot(goodClickSFX);
    }

    void EvaluatePerformance()
    {
        float accuracy = (float)successCount / totalActions * 100f;
        string rank = CalculateRank(accuracy);

        Debug.Log($" Fin du niveau !");
        Debug.Log($"Réussites : {successCount} / {totalActions}");
        Debug.Log($" Fails : {failCount}");
        Debug.Log($" Précision : {accuracy:F1}%");
        Debug.Log($" Rank : {rank}");
    }

    string CalculateRank(float accuracy)
    {
        if (accuracy == 100f) return "S+";
        if (accuracy >= 85f) return "S";
        if (accuracy >= 75f) return "A";
        if (accuracy >= 50f) return "B";
        if (accuracy >= 30f) return "C";
        return "D";
    }
}

[System.Serializable]
public class RhythmAction
{
    public string keyword;
    public float startTime;
    public float endTime;
    public string expectedClick;
    [System.NonSerialized] public bool hasTriggered = false;
}

[System.Serializable]
public class RhythmActionList
{
    public string tutoClip;
    public string audioClip;
    public List<RhythmAction> actions;
}
