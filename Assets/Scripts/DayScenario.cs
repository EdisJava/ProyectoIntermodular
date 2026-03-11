using UnityEngine;
using System.Collections.Generic;

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

    [Header("Diálogos del Profesor de Hoy")]
    public DialogueData teacherCasualDialogue;
    public DialogueData teacherDecisionDialogue;
    public DialogueData postDecisionDialogue; // Diálogo para cuando ya has acusado
}




[System.Serializable]
public class StudentDailyConfig
{
    public string characterName;
    public DialogueData casualDialogue;      // Arrastra aquí el archivo de charla normal
    public DialogueData truthDialogue;       // Arrastra aquí el guion con la verdad
    public DialogueData lieDialogue;         // Arrastra aquí el guion con la mentira
    public bool isLiarToday;                 // ¿Hoy le toca mentir?


}