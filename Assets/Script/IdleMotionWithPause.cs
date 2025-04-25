using UnityEngine;
using System.Collections;

public class IdleMotionDelayedStart : MonoBehaviour
{
    public float amplitude = 0.02f;
    public float frequency = 2.5f;  // fréquence normale
    public float minStartDelay = 1f;
    public float maxStartDelay = 5f;

    private Vector3 startPos;
    private bool isIdle = false;
    private float motionTime = 0f;
    private float currentFrequency;

    void Start()
    {
        startPos = transform.position;
        currentFrequency = frequency;
        StartCoroutine(StartIdleAfterDelay());
    }

    void Update()
    {
        if (!isIdle) return;

        motionTime += Time.deltaTime;
        float offsetY = Mathf.Sin(motionTime * currentFrequency) * amplitude;
        transform.position = startPos + new Vector3(0f, offsetY, 0f);
    }

    IEnumerator StartIdleAfterDelay()
    {
        float delay = Random.Range(minStartDelay, maxStartDelay);
        yield return new WaitForSeconds(delay);
        isIdle = true;
    }

    public void BoostFrequency(float boostValue = 20f, float duration = 2f)
    {
        StartCoroutine(BoostFrequencyRoutine(boostValue, duration));
    }

    private IEnumerator BoostFrequencyRoutine(float boostValue, float duration)
    {
        float originalFrequency = currentFrequency;
        currentFrequency = boostValue;
        yield return new WaitForSeconds(duration);
        currentFrequency = originalFrequency;
    }
}