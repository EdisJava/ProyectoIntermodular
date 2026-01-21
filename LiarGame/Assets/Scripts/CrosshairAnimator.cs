using System.Collections;
using UnityEngine;

public class CrosshairAnimator : MonoBehaviour
{
    public float rotationAmount = 180f;  
    public float duration = 0.25f;       

    private RectTransform rectTransform;
    private Coroutine currentAnim;

    void Awake()
    {
        rectTransform = GetComponent<RectTransform>();
    }

    public void PlayAppear()
    {
        if (currentAnim != null)
            StopCoroutine(currentAnim);

        currentAnim = StartCoroutine(AppearAnimation());
    }

    IEnumerator AppearAnimation()
    {
        float t = 0f;

        // Empieza a girar
        rectTransform.localRotation = Quaternion.Euler(0, 0, rotationAmount);

        while (t < duration)
        {
            t += Time.deltaTime;
            float normalized = t / duration;

            float z = Mathf.Lerp(rotationAmount, 0f, normalized);
            rectTransform.localRotation = Quaternion.Euler(0, 0, z);

            yield return null;
        }

        rectTransform.localRotation = Quaternion.identity;
    }
}