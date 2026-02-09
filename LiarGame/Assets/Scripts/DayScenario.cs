using UnityEngine;
using System.Collections.Generic;

[CreateAssetMenu(fileName = "NuevoEscenarioDia", menuName = "Juego/Escenario del Dia")]
public class DayScenario : ScriptableObject
{
    public string victimName;
    public List<StudentDailyConfig> characterConfigs;
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