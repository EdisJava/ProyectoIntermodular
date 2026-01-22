using System.Collections;
using UnityEngine;

public class CrosshairAnimator : MonoBehaviour
{
    [Header("Appear animation")]
    public float appearRotationAmount = 45f;
    public float appearDuration = 0.25f;

    [Header("Idle rotation")]
    public float idleRotationSpeed = 15f; 

    private RectTransform rectTransform;
    private Coroutine currentAnim;
    private Coroutine idleRotationCoroutine;

    void Awake()
    {
        rectTransform = GetComponent<RectTransform>();
    }

    public void PlayAppear()
    {
        if (rectTransform == null)
            rectTransform = GetComponent<RectTransform>();

        if (currentAnim != null)
            StopCoroutine(currentAnim);

        if (idleRotationCoroutine != null)
            StopCoroutine(idleRotationCoroutine);

        currentAnim = StartCoroutine(AppearAndIdle());
    }

    public void StopIdle()
    {
        if (rectTransform == null)
            rectTransform = GetComponent<RectTransform>();

        if (idleRotationCoroutine != null)
        {
            StopCoroutine(idleRotationCoroutine);
            idleRotationCoroutine = null;
        }

        rectTransform.localRotation = Quaternion.identity;
    }

    IEnumerator AppearAndIdle()
    {
        float t = 0f;

       
        rectTransform.localRotation = Quaternion.Euler(0, 0, appearRotationAmount);

        while (t < appearDuration)
        {
            t += Time.deltaTime;
            float normalized = t / appearDuration;

            float z = Mathf.Lerp(appearRotationAmount, 0f, normalized);
            rectTransform.localRotation = Quaternion.Euler(0, 0, z);

            yield return null;
        }

        rectTransform.localRotation = Quaternion.identity;

      
        idleRotationCoroutine = StartCoroutine(IdleRotation());
    }

    IEnumerator IdleRotation()
    {
        while (true)
        {
            rectTransform.Rotate(0f, 0f, -idleRotationSpeed * Time.deltaTime);
            yield return null;
        }
    }
}
