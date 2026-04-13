using UnityEngine;
using TMPro;
using System.Collections;

/*
* Script para manejar el dialogo de un alumno.
* 
* Metodos:
*   - StartDialogue(): Metodo principal que se llama al interactuar con el alumno.
*   - ShowLine(): Metodo que muestra el dialogo del alumno.
*   
*   Variables:
*   - dialoguePanel: Panel que muestra el dialogo.
*   - dialogueText: Texto que muestra el dialogo.
*   - dialogueLines: Array de strings que contiene el dialogo.
*   - didDialogueStart: Si el dialogo ha comenzado.
*   - lineIndex: Indice de la linea actual.
*   - typingSpeed: Velocidad de escritura del dialogo.
*
*   Funcionamiento:
*   - Al interactuar con el alumno, se llama al metodo StartDialogue().
*   - El metodo StartDialogue() muestra el dialogo del alumno.
*   - El metodo ShowLine() muestra el dialogo del alumno linea por linea.
*
*   Flujo:
*   1. El jugador interactua con el alumno.
*   2. Se llama al metodo StartDialogue().
*   3. Se muestra el dialogo del alumno.
*   4. El jugador puede interactuar con otro alumno.
*/

public class Dialogue : MonoBehaviour
{

    

    [SerializeField] private GameObject dialoguePanel;
    [SerializeField] private TMP_Text dialogueText;
    [SerializeField, TextArea(4,6)] private string[] dialogueLines;

    
    private bool didDialogueStart;
    private int lineIndex;
    //Velocidad de escritura del dialogo.
    private float typingSpeed = 0.03f;


    void Update()
    {
        
    }

    /*
    * Metodo principal que se llama al interactuar con el alumno.
    */
    public void StartDialogue()
    {
        // Si no esta en 2D, no se puede iniciar el dialogo.
        if (!GameManager.Instance.IsIn2D()) return;

        // CASO 1: Fase de Charla Casual
        if (GameManager.Instance.currentDayPhase == DayPhase.CasualTalk)
        { 
            {
                GameManager.Instance.FoundVictim();
            }
            didDialogueStart = true;
        dialoguePanel.SetActive(true);
        lineIndex = 0;
        StartCoroutine(ShowLine());
        }
    }

    /*
    * Metodo que muestra el dialogo del alumno linea por linea.
    */
    private IEnumerator ShowLine()
    {
        //Limpia el texto.
        dialogueText.text = string.Empty;

        //Muestra el dialogo linea por linea.
        foreach (char ch in dialogueLines[lineIndex])
        {
            dialogueText.text += ch;
            //Espera un tiempo determinado antes de mostrar la siguiente letra.
            yield return new WaitForSeconds(typingSpeed);
        }
    }
}
