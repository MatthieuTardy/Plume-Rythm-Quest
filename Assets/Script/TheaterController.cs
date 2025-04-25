using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.InputSystem; // 🎯 Nouveau Input System

public class NarratorManager : MonoBehaviour
{
    public AudioSource narratorAudio;
    public AudioSource tutoAudio;
    public AudioSource musiqueAudio;
    public AudioSource sfxAudio;

    public AudioClip badClickSFX;
    public AudioClip goodClickSFX;

    private AudioClip narratorClip;
    private AudioClip narratorTutoClip;
    private AudioClip musiqueClip;

    private int totalActions = 0;
    private int successCount = 0;
    private int failCount = 0;
    private bool hasFinished = false;
    private bool tutoEnd;

    public AudioClip Splus;
    public AudioClip S;
    public AudioClip A;
    public AudioClip B;
    public AudioClip C;
    public AudioClip D;

    public TextAsset scriptJSON;
    private List<RhythmAction> actions;

    public string audioClip;
    public string tutoClip;
    public string musicClip;
    private float timer;

    [Header("Input System")]
    public InputActionReference leftClickAction;
    public InputActionReference rightClickAction;

    void Start()
    {
        tutoEnd = false;

        // Charger le script JSON
        RhythmActionList list = JsonUtility.FromJson<RhythmActionList>(scriptJSON.text);
        actions = list.actions;
        totalActions = actions.Count;

        tutoClip = list.tutoClip;
        audioClip = list.audioClip;
        musicClip = list.musiqueClip;

        narratorTutoClip = Resources.Load<AudioClip>(tutoClip);
        narratorClip = Resources.Load<AudioClip>(audioClip);
        musiqueClip = Resources.Load<AudioClip>(musicClip);

        tutoAudio.clip = narratorTutoClip;
        narratorAudio.clip = narratorClip;
        musiqueAudio.clip = musiqueClip;

        musiqueAudio.Play();
        tutoAudio.Play();

        // Activer les actions du nouveau système
        leftClickAction.action.Enable();
        rightClickAction.action.Enable();
    }

    void OnDisable()
    {
        // Bonnes pratiques : désactiver proprement
        leftClickAction.action.Disable();
        rightClickAction.action.Disable();
    }

    void Update()
    {
        timer = narratorAudio.time;

        bool isActionWindow = false;

        foreach (var action in actions)
        {
            if (!action.hasTriggered && timer >= action.startTime && timer <= action.endTime)
            {
                isActionWindow = true;
                CheckInput(action);
            }
            else if (!action.hasTriggered && timer > action.endTime)
            {
                if (action.tapCount >= action.requiredTaps)
                    TriggerSuccess(action);
                else
                    TriggerFail(action);
            }
        }

        // ❌ Clique dans le vide
        if ((leftClickAction.action.WasPerformedThisFrame() || rightClickAction.action.WasPerformedThisFrame()) && !isActionWindow)
        {
            Debug.Log("Clic dans le vide !");
            StartCoroutine(ClicDansLeVide());
        }

        // 🎓 Passage du tuto à la narration
        if ((tutoAudio.time >= tutoAudio.clip.length || leftClickAction.action.WasPerformedThisFrame()) && !tutoEnd)
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
        if (leftClickAction.action.WasPerformedThisFrame())
        {
            if (action.expectedClick == "Gauche")
                action.tapCount++;
            else
                TriggerFail(action);
        }
        else if (rightClickAction.action.WasPerformedThisFrame())
        {
            if (action.expectedClick == "Droite")
                action.tapCount++;
            else
                TriggerFail(action);
        }
    }

    public IEnumerator ClicDansLeVide()
    {
        if (tutoEnd)
        {
            PlayBadSound();

            if (successCount > 0)
            {
                successCount--;
                failCount++;
            }

            narratorAudio.Pause();
            narratorAudio.Play();
            yield return null;
        }
    }

    void TriggerSuccess(RhythmAction action)
    {
        Debug.Log($"Réussi : {action.keyword} avec {action.expectedClick}");
        action.hasTriggered = true;
        successCount++;
        PlayGoodSound();
    }

    void TriggerFail(RhythmAction action)
    {
        Debug.Log($"Mauvais bouton pour {action.keyword}");
        action.hasTriggered = true;
        failCount++;
        StartCoroutine(PausePlay());
    }

    public IEnumerator PausePlay()
    {
        PlayBadSound();
        narratorAudio.Pause();
        narratorAudio.Play();
        yield return null;
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
        if (goodClickSFX != null && sfxAudio != null)
        {
            sfxAudio.PlayOneShot(goodClickSFX);
        }
    }

    void EvaluatePerformance()
    {
        float accuracy = (float)successCount / totalActions * 100f;
        CalculateRank(accuracy);
    }

    void CalculateRank(float accuracy)
    {
        if (accuracy == 100f)
        {
            narratorAudio.clip = Splus;
            narratorAudio.Play();
        }
        else if (accuracy >= 85f)
        {
            narratorAudio.clip = S;
            narratorAudio.Play();
        }
        else if (accuracy >= 75f)
        {
            narratorAudio.clip = A;
            narratorAudio.Play();
        }
        else if (accuracy >= 50f)
        {
            narratorAudio.clip = B;
            narratorAudio.Play();
        }
        else if (accuracy >= 30f)
        {
            narratorAudio.clip = C;
            narratorAudio.Play();
        }
        else
        {
            narratorAudio.clip = D;
            narratorAudio.Play();
        }
    }
}

// 📦 Classes de données

[System.Serializable]
public class RhythmAction
{
    public string keyword;
    public float startTime;
    public float endTime;
    public string expectedClick;
    public int requiredTaps = 1;
    [System.NonSerialized] public int tapCount = 0;
    [System.NonSerialized] public bool hasTriggered = false;
}

[System.Serializable]
public class RhythmActionList
{
    public string tutoClip;
    public string audioClip;
    public string musiqueClip;
    public List<RhythmAction> actions;
}
