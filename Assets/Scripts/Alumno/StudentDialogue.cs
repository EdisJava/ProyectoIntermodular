using UnityEngine;

/*
* Script para manejar el dialogo de un alumno.
*
* Metodos:
*   - Talk(): Metodo principal que se llama al interactuar con el alumno.
*   - CasualTalk(): Metodo que se llama en la fase de dialogo casual.
*   - InvestigationTalk(): Metodo que se llama en la fase de investigacion.
*   - DecisionTalk(): Metodo que se llama en la fase de decision.
*   - ResetForNewDay(): Metodo que se llama al inicio de cada dia.
*   
*   Variables:
*   - studentName: Nombre del alumno.
*   - role: Rol del alumno.
*   - liesAboutBully: Si el alumno miente al ser preguntado.
*   - casualDialogue: Dialogo casual del alumno.
*   - victimDialogue: Dialogo de victima del alumno.
*   - truthDialogue: Dialogo de verdad del alumno.
*   - lieDialogue: Dialogo de mentira del alumno.
*   - alreadyAsked: Si el alumno ya ha sido preguntado.
*
*   Funcionamiento:
*   - Al interactuar con el alumno, se llama al metodo Talk().
*   - El metodo Talk() llama al metodo correspondiente segun la fase actual del dia.
*   - En la fase de dialogo casual, se llama al metodo CasualTalk().
*   - En la fase de investigacion, se llama al metodo InvestigationTalk().
*   - En la fase de decision, se llama al metodo DecisionTalk().
*   - El metodo ResetForNewDay() se llama al inicio de cada dia.
*
*   Flujo:
*   1. El jugador interactua con el alumno.
*   2. Se llama al metodo Talk().
*   3. Se determina la fase actual del dia.
*   4. Se llama al metodo correspondiente segun la fase.
*   5. Se muestra el dialogo del alumno.
*   6. El jugador puede interactuar con otro alumno.
*/






public enum StudentRole
{
    Normal,
    Victim,
    Bully
}


/*
* Clase principal que maneja el dialogo de un alumno.
*/
public class StudentDialogue : MonoBehaviour
{
    [Header("Identidad")] 
    public string studentName;
    public StudentRole role;

    [Header("Comportamiento")]
    public bool liesAboutBully; 

    [Header("Dialogos")]
    [TextArea] public string casualDialogue;
    [TextArea] public string victimDialogue;
    [TextArea] public string truthDialogue;
    [TextArea] public string lieDialogue;

    private bool alreadyAsked = false;

    /*
    * Metodo principal que se llama al interactuar con el alumno.
    */
    public void Talk()
    {
        //Obtiene la instancia del GameManager.
        var gm = GameManager.Instance;

        
        //Determina la fase actual del dia y llama al metodo correspondiente.
        switch (gm.currentDayPhase)
        {
            case DayPhase.CasualTalk:
                CasualTalk();
                break;

            case DayPhase.Investigation:
                InvestigationTalk();
                break;

            case DayPhase.Decision:
                DecisionTalk();
                break;
        }
    }

    // ---------------- FASE 1 ----------------

    /*
    * Metodo que se llama en la fase de dialogo casual.
    */
    void CasualTalk()
    {
        //Si el alumno es victima, se muestra el dialogo de victima.
        if (role == StudentRole.Victim)
        {
            Debug.Log($"{studentName}: {victimDialogue}");
            GameManager.Instance.FoundVictim();
        }
        else
        {
            Debug.Log($"{studentName}: {casualDialogue}");
        }
    }

    // ---------------- FASE 2 ----------------

    /*
    * Metodo que se llama en la fase de investigacion.
    */
    void InvestigationTalk()
    {
        if (alreadyAsked)
        {
            Debug.Log($"{studentName}: Ya te dije todo lo que sabia.");
            return;
        }
        
        if (!GameManager.Instance.CanAskQuestion())
        {
            Debug.Log("No puedes hacer mas preguntas hoy.");
            return;
        }

        //Marca el alumno como preguntado.
        alreadyAsked = true;
        //Usa una pregunta.
        GameManager.Instance.UseQuestion();

        if (liesAboutBully)
            Debug.Log($"{studentName}: {lieDialogue}");
        else
            Debug.Log($"{studentName}: {truthDialogue}");
    }

    // ---------------- FASE 3 ----------------

    /*
    * Metodo que se llama en la fase de decision.
    */
    void DecisionTalk()
    {
        Debug.Log($"{studentName}: Deberas hablar con el profesor.");
    }

    // ---------------- RESET POR DIA ----------------

    /*
    * Metodo que se llama al inicio de cada dia.
    */
    public void ResetForNewDay()
    {
        alreadyAsked = false;
    }
}
