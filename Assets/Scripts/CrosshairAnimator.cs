using System.Collections;
using UnityEngine;

/*
* Script para manejar la animacion del cursor.
* 
* Metodos:
*   - PlayAppear(): Metodo que reproduce la animacion de aparecer.
*   - StopIdle(): Metodo que detiene la animacion de reposo.
*
*   Variables:
*   - appearRotationAmount: Cantidad de rotacion al aparecer.
*   - appearDuration: Duracion de la animacion de aparecer.
*   - idleRotationSpeed: Velocidad de rotacion en reposo.
*
*   Funcionamiento:
*   - Al aparecer, rota el cursor y luego vuelve a su posicion original.
*   - En reposo, rota el cursor lentamente.
*
*   Flujo:
*   1. El jugador interactua con el alumno.
*   2. Se llama al metodo Interact().
*   3. Se determina la fase actual del dia.
*   4. Se llama al metodo correspondiente segun la fase.
*   5. Se muestra el dialogo del alumno.
*   6. El jugador puede interactuar con otro alumno.
*/

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

    /*
    * Metodo que se llama al inicio.
    */
    void Awake()
    {
        rectTransform = GetComponent<RectTransform>();
    }

    /*
    * Metodo que reproduce la animacion de aparecer.
    */
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

    /*
    * Metodo que detiene la animacion de reposo.
    */
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

    /*
    * Metodo que reproduce la animacion de aparecer y reposo.
    */
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

    /*
    * Metodo que reproduce la animacion de reposo.
    */
    IEnumerator IdleRotation()
    {
        while (true)
        {
            rectTransform.Rotate(0f, 0f, -idleRotationSpeed * Time.deltaTime);
            yield return null;
        }
    }
}
