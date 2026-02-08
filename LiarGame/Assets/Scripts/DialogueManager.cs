using UnityEngine;
using TMPro;
using System.Collections;

public class DialogueManager : MonoBehaviour
{
    public static DialogueManager Instance;

    [Header("UI Elements")]
    public GameObject dialoguePanel;
    public TextMeshProUGUI dialogueText;
    public TextMeshProUGUI nameText;

    private Vector3 originalPosition;
    private Transform currentCharacter;
    private bool isTalking = false;

    void Awake() => Instance = this;

    public void ShowDialogue(string name, string text, Transform characterTransform, Vector3 centerPos)
    {
        if (isTalking) return;
        isTalking = true;

        currentCharacter = characterTransform;
        originalPosition = currentCharacter.position; // Guardamos donde estaba

        // 1. Mover al personaje al frente (puedes usar una animación o simplemente el teletransporte)
        currentCharacter.position = centerPos;
        currentCharacter.localScale *= 1.2f; // Lo hacemos un poco más grande para el "cara a cara"

        // 2. Mostrar UI
        dialoguePanel.SetActive(true);
        nameText.text = name;
        dialogueText.text = text;
    }

    public void CloseDialogue()
    {
        if (!isTalking) return;

        // 1. Devolver al personaje a su sitio
        currentCharacter.position = originalPosition;
        currentCharacter.localScale /= 1.2f;

        // 2. Ocultar UI
        dialoguePanel.SetActive(false);
        isTalking = false;
    }
}