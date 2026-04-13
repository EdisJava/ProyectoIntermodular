using UnityEngine;
using System.Collections.Generic;

/*
* Script para manejar el escenario del dia.
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
*   - Si han cambiado, actualiza la visualizacion de objetivos.
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

[CreateAssetMenu(fileName = "NuevoEscenarioDia", menuName = "Juego/Escenario del Dia")]
public class DayScenario : ScriptableObject
{
    public string victimName;
    public string teacherName; 

    public List<StudentDailyConfig> characterConfigs;

    [Header("Textos de Objetivo")]
    [TextArea(2, 4)] public string goalFindVictim = "Encuentra al acosado";
    [TextArea(2, 4)] public string goalInvestigate = "Interroga a los alumnos";
    [TextArea(2, 4)] public string goalReport = "Habla con el profesor";

    [Header("Dialogos del Profesor de Hoy")]
    public DialogueData teacherCasualDialogue;
    public DialogueData teacherDecisionDialogue;
    public DialogueData postDecisionDialogue; // Dialogo para cuando ya has acusado
}




[System.Serializable]
public class StudentDailyConfig
{
    public string characterName;
    public DialogueData casualDialogue;      // Arrastra aqui el archivo de charla normal
    public DialogueData truthDialogue;       // Arrastra aqui el guion con la verdad
    public DialogueData lieDialogue;         // Arrastra aqui el guion con la mentira
    public bool isLiarToday;                 // Hoy le toca mentir?


}