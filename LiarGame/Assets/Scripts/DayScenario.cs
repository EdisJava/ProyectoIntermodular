using UnityEngine;
using System.Collections.Generic;

[CreateAssetMenu(fileName = "New Day", menuName = "Game/Day Scenario")]
public class DayScenario : ScriptableObject
{
    [Header("Configuración del Día")]
    public string victimName; // El nombre del personaje acosado hoy
    public string bullyName;  // El nombre del acosador hoy

    [Header("Diálogos de este día")]
    // Aquí guardamos qué dice cada personaje en este día específico
    public List<CharacterDialogue> characterDialogues;
}

[System.Serializable]
public class CharacterDialogue
{
    public string characterName;
    [TextArea] public string casualText; // "Hola, ¿qué tal el examen?"
    [TextArea] public string truthText;  // Pista verdadera
    [TextArea] public string lieText;    // Pista falsa
    public bool isLiarToday;             // ¿Miente hoy?
}