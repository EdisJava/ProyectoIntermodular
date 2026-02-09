using UnityEngine;

[CreateAssetMenu(fileName = "DayData", menuName = "Game/DayData")]
public class DayData : ScriptableObject
{
    [Header("Identificación")]
    public int dayNumber = 1;
    public string displayName = "Día 1";

    [Header("Diálogos")]
    public DialogueLine[] dialogues;

    [Header("Decisiones")]
    public DecisionPoint[] decisions;

    [Header("Áreas/pantallas 2D disponibles este día")]
    // Sólo se guardan identificadores (strings) que deben corresponder con las puertas en escena.
    public string[] availableAreaNames;

    [System.Serializable]
    public class DialogueLine
    {
        public string speaker;
        [TextArea(2,6)] public string text;
    }

    [System.Serializable]
    public class DecisionPoint
    {
        [Tooltip("Aparece después de la línea con este índice (0-based)")]
        public int afterDialogueIndex;
        public string[] options;
        [Tooltip("Índice (0-based) de la opción mala")]
        public int badOptionIndex;
        [TextArea] public string badOptionFeedback;
    }
}