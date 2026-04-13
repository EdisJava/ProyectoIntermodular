using UnityEngine;

/*
* Script para manejar la interaccion con un alumno.
* 
* Metodos:
*   - Interact(): Metodo principal que se llama al interactuar con el alumno.
*   - CasualTalk(): Metodo que se llama en la fase de dialogo casual.
*   - InvestigationTalk(): Metodo que se llama en la fase de investigacion.
*   - Accuse(): Metodo que se llama en la fase de decision.
*   
*   Variables:
*   - data: Datos del alumno.
*   - alreadyAsked: Si el alumno ya ha sido preguntado.
*
*   Funcionamiento:
*   - Al interactuar con el alumno, se llama al metodo Interact().
*   - El metodo Interact() llama al metodo correspondiente segun la fase actual del dia.
*   - En la fase de dialogo casual, se llama al metodo CasualTalk().
*   - En la fase de investigacion, se llama al metodo InvestigationTalk().
*   - En la fase de decision, se llama al metodo Accuse().
*   - El metodo ResetForNewDay() se llama al inicio de cada dia.
*
*   Flujo:
*   1. El jugador interactua con el alumno.
*   2. Se llama al metodo Interact().
*   3. Se determina la fase actual del dia.
*   4. Se llama al metodo correspondiente segun la fase.
*   5. Se muestra el dialogo del alumno.
*   6. El jugador puede interactuar con otro alumno.
*/

public class StudentInteraction : MonoBehaviour
{
    public StudentData data;

    private bool alreadyAsked = false;

    /*
    * Metodo principal que se llama al interactuar con el alumno.
    */
    public void Interact()
    {
        //Si el dia actual no es el dia en que aparece el alumno, no se puede interactuar con el.
        if (GameManager.Instance.currentDay != data.appearsOnDay)
            return;

        //Determina la fase actual del dia y llama al metodo correspondiente.
        switch (GameManager.Instance.currentDayPhase)
        {
            case DayPhase.CasualTalk:
                CasualTalk();
                break;

            case DayPhase.Investigation:
                InvestigationTalk();
                break;
        }
    }

    /*
    * Metodo que se llama en la fase de dialogo casual.
    */
    void CasualTalk()
    {
        Debug.Log(data.studentName + ": " + data.casualDialogue);

        //Si el alumno es victima, se registra como encontrado.
        if (data.isVictim)
        {
            GameManager.Instance.FoundVictim();
        }
    }

    /*
    * Metodo que se llama en la fase de investigacion.
    */
    void InvestigationTalk()
    {
        //Si el alumno ya ha sido preguntado, no se puede volver a preguntar.
        if (alreadyAsked)
        {
            Debug.Log("Ya hablaste con esta persona.");
            return;
        }

        //Si no quedan preguntas, no se puede preguntar.
        if (!GameManager.Instance.CanAskQuestion())
        {
            Debug.Log("No te quedan preguntas hoy.");
            return;
        }

        //Usa una pregunta.
        GameManager.Instance.UseQuestion();
        //Marca el alumno como preguntado.
        alreadyAsked = true;

        //Muestra el dialogo de verdad o mentira segun el caso.
        if (data.lies)
            Debug.Log(data.studentName + ": Creo que fue X (mentira)");
        else
            Debug.Log(data.studentName + ": Creo que fue el acosador real");
    }

    /*
    * Metodo que se llama en la fase de decision.
    */
    public void Accuse()
    {
        //Registra la decision del jugador.
        GameManager.Instance.RegisterDecision(data.isBully);
    }
}
