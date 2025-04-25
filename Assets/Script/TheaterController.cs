using UnityEngine;
using System.Collections;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using UnityEngine.EventSystems;
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

    [Header("UI Fin de Niveau")]
    public GameObject panelFin;
    public Button buttonRecommencer;
    public Button buttonSuite;
    public GameObject panelFinFirstSelect;

    [Header("Input System")]
    public InputActionReference leftClickAction;
    public InputActionReference rightClickAction;

    void Start()
    {
        tutoEnd = false;
        musiqueAudio.pitch = 1f;

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
        // UI fin désactivée au départ
        panelFin.SetActive(false);
        buttonRecommencer.onClick.AddListener(ReloadScene);
        buttonSuite.onClick.AddListener(OnSuite);
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
            if (!hasFinished && !action.hasTriggered && timer >= action.startTime && timer <= action.endTime)
            {
                isActionWindow = true;
                CheckInput(action);
            }
            else if (!hasFinished && !action.hasTriggered && timer > action.endTime)
            {
                if (action.tapCount >= action.requiredTaps)
                    TriggerSuccess(action);
                else
                    TriggerFail(action);
            }
        }

        // Clique dans le vide
        if ((!hasFinished && leftClickAction.action.WasPerformedThisFrame() || !hasFinished && rightClickAction.action.WasPerformedThisFrame()) && !isActionWindow)
        {
            Debug.Log("Clic dans le vide !");
            StartCoroutine(ClicDansLeVide());
        }

        // Passage du tuto à la narration
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
        if (!hasFinished && leftClickAction.action.WasPerformedThisFrame())
        {
            if (action.expectedClick == "Gauche")
            {
                action.tapCount++;
                PlayGoodSound();
            }
            else
                TriggerFail(action);
            

        }
        else if (!hasFinished && rightClickAction.action.WasPerformedThisFrame())
        {
            if (action.expectedClick == "Droite")
            {
                action.tapCount++;
                PlayGoodSound();
            }
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
            yield return null;
        }
    }

    void TriggerSuccess(RhythmAction action)
    {
        action.hasTriggered = true;
        successCount++;
        
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
            sfxAudio.pitch = Random.Range(0.80f, 1.20f);
            musiqueAudio.pitch -= 0.05f;
            sfxAudio.PlayOneShot(badClickSFX);
        }
    }

    void PlayGoodSound()
    {
        if (goodClickSFX != null && sfxAudio != null)
        {
            sfxAudio.pitch = Random.Range(0.90f, 1.10f);
            sfxAudio.PlayOneShot(goodClickSFX);
        }
    }
    void ShowFinPanel(GameObject panelToShow, GameObject buttonToSelect)
    {
        panelToShow.SetActive(true); // Affiche le panel de fin
        EventSystem.current.SetSelectedGameObject(null); // Efface la sélection précédente
        if (buttonToSelect != null)
        {
            EventSystem.current.SetSelectedGameObject(buttonToSelect); // Sélectionne le bouton passé en paramètre
        }
    }
    void EvaluatePerformance()
    {
        float accuracy = (float)successCount / totalActions * 100f;
        CalculateRank(accuracy);

        ShowFinPanel(panelFin, buttonRecommencer.gameObject); // Pré-sélectionne le bouton "Recommencer"
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
    // Recommencer la scène
    void ReloadScene()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex); // Recharge la scène actuelle
    }

    // 👉 Action du bouton "Suite"
    void OnSuite()
    {
        Debug.Log("Continuer ou retourner au menu...");
        // Exemple : SceneManager.LoadScene("NomDeLaSceneSuivante");
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
