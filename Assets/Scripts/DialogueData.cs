using UnityEngine;
using System.Collections.Generic;

/*
* Script para manejar los datos de dialogo.
* 
* Metodos:
*   - Update(): Metodo que se llama cada frame.
*   - UpdateGoalDisplay(): Metodo que actualiza la visualizacion de objetivos.
*   - ApplyUppercase(): Metodo que aplica mayusculas al texto.
*
*   Variables:
*   - goalText: Texto que muestra los objetivos.
*   - fixedFontAsset: Fuente fija para el texto.
*   - lastPhase: Fase anterior.
*   - lastDay: Dia anterior.
*   - lastAccusedState: Estado de acusacion anterior.
*   - lastPrologueReadState: Estado de lectura del prologo anterior.
*   - lastSceneName: Nombre de la escena anterior.
*
*   Funcionamiento:
*   - Al actualizar, verifica si la fase, el dia, el estado de acusacion o el nombre de la escena han cambiado.
*   - Si han cambiado, actualiza la visualizacion de objetos.
*   - Aplica mayusculas al texto.
*
*   Flujo:
*   1. El jugador interactua con el alumno.
*   2. Se llama al metodo Interact().
*   3. Se determina la fase actual del dia.
*   4. Se llama al metodo correspondiente segun la fase.
*   5. Se muestra el dialogo del alumno.
*   6. El jugador puede interactuar con otro alumno.
*/

[CreateAssetMenu(fileName = "NuevaConversacion", menuName = "Dialogo/Conversacion")]
public class DialogueData : ScriptableObject
{
    [System.Serializable]
    public struct DialogueLine
    {
        public string characterName;
        [TextArea] public string text;
        public Sprite expression; // La cara que pone en esta frase

        [Header("Efectos de la Linea")]
        public AudioClip lineSound;    // Sonido al empezar la frase
        public AudioClip typingVoice;
        public Sprite backgroundOverride;  // Si pones uno, cambia el fondo del escenario

        public bool hasOptions;
        public bool isFinalButton; // Marca esto para que al terminar el dialogo cargue el MainMenu
        public List<DialogueOption> options;
    }

    public List<DialogueLine> lines;
}

[System.Serializable]
public struct DialogueOption
{
    public string optionText;
    public DialogueData nextDialogue; // Hacia donde va la charla si elige esto
    public bool isInterrogation;
   
    public bool isFinalDecision; // Marca esto en el Inspector para las opciones de "Chivarse"
    public bool isCorrectAccusation; // Marca esto solo en la opcion del culpable real
}