using UnityEngine;
using TMPro;
using System.Collections;

public class Dialogue : MonoBehaviour
{

    [SerializeField] private GameObject dialoguePanel;
    [SerializeField] private TMP_Text dialogueText;
    [SerializeField, TextArea(4,6)] private string[] dialogueLines;

    private bool didDialogueStart;
    private int lineIndex;
    private float typingSpeed = 0.03f;

    void Update()
    {
        
    }

    public void StartDialogue()
    {
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

    private IEnumerator ShowLine()
    {
        dialogueText.text = string.Empty;

        foreach (char ch in dialogueLines[lineIndex])
        {
            dialogueText.text += ch;
            yield return new WaitForSeconds(typingSpeed);
        }
    }
}
