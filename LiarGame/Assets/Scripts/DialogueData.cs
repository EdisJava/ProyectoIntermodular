using UnityEngine;
using System.Collections.Generic;

[CreateAssetMenu(fileName = "NuevaConversacion", menuName = "Dialogo/Conversacion")]
public class DialogueData : ScriptableObject
{
    [System.Serializable]
    public struct DialogueLine
    {
        public string characterName;
        [TextArea] public string text;
        public Sprite expression; // La cara que pone en esta frase
        public bool hasOptions;
        public List<DialogueOption> options;
    }

    public List<DialogueLine> lines;
}

[System.Serializable]
public struct DialogueOption
{
    public string optionText;
    public DialogueData nextDialogue; // Hacia dónde va la charla si eliges esto
    public bool isInterrogation;
}