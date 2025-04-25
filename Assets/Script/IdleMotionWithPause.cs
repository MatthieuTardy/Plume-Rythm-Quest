using UnityEngine;
using System.Collections;

public class IdleMotionDelayedStart : MonoBehaviour
{
    public float amplitude = 0.5f;
    public float frequency = 1f;

    public float minStartDelay = 1f;
    public float maxStartDelay = 5f;

    private Vector3 startPos;
    private bool isIdle = false;
    private float motionTime = 0f;

    void Start()
    {
        startPos = transform.position;
        StartCoroutine(StartIdleAfterDelay());
    }

    void Update()
    {
        if (!isIdle) return;

        motionTime += Time.deltaTime;
        float offsetY = Mathf.Sin(motionTime * frequency) * amplitude;
        transform.position = startPos + new Vector3(0f, offsetY, 0f);
    }

    IEnumerator StartIdleAfterDelay()
    {
        float delay = Random.Range(minStartDelay, maxStartDelay);
        yield return new WaitForSeconds(delay);
        isIdle = true;
    }
}
