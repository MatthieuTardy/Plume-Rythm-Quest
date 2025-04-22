using System.Collections.Generic;
using UnityEngine;

public class NarratorManager : MonoBehaviour
{
    public AudioSource narratorAudio;
    public TextAsset scriptJSON;
    private List<RhythmAction> actions;
    private float timer;

    void Start()
    {
        actions = JsonUtility.FromJson<RhythmActionList>(scriptJSON.text).actions;
        narratorAudio.Play();
    }

    void Update()
    {
        timer = narratorAudio.time;

        foreach (var action in actions)
        {
            if (!action.hasTriggered && timer >= action.startTime && timer <= action.endTime)
            {
                TriggerAction(action);
                action.hasTriggered = true;
            }
        }
    }

    void TriggerAction(RhythmAction action)
    {
        Debug.Log($"Action déclenchée : {action.keyword} → {action.actionType}");
        // ➕ Ici on déclenche la bonne action dans le jeu (son, retour, vibration, feedback audio...)
    }
}

[System.Serializable]
public class RhythmAction
{
    public string keyword;
    public float startTime;
    public float endTime;
    public string actionType;
    [System.NonSerialized] public bool hasTriggered = false;
}

[System.Serializable]
public class RhythmActionList
{
    public List<RhythmAction> actions;
}