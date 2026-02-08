using UnityEngine;
using TMPro;
using UnityEngine.UI;
using System.Collections;

public class DialogueManager : MonoBehaviour
{
    public static DialogueManager Instance;

    [Header("UI Elementos")]
    public GameObject dialoguePanel;
    public TextMeshProUGUI textDisplay;
    public TextMeshProUGUI nameDisplay;
    public Image portraitDisplay; // Imagen del personaje en el centro
    public GameObject optionsParent;
    public GameObject optionButtonPrefab;

    private DialogueData currentData;
    private int lineIndex;
    private bool isTyping;
    private StudentNPC currentNPC;

    void Awake() => Instance = this;

    public void StartDialogue(DialogueData data, StudentNPC npc)
    {
        currentData = data;
        currentNPC = npc;
        lineIndex = 0;

        dialoguePanel.SetActive(true);
        portraitDisplay.gameObject.SetActive(true);
        optionsParent.SetActive(false);

        // Efecto visual: el NPC se prepara para hablar
        currentNPC.EnterFocus();

        DisplayLine();
    }

    void DisplayLine()
    {
        StopAllCoroutines();
        var line = currentData.lines[lineIndex];

        nameDisplay.text = line.characterName;
        portraitDisplay.sprite = line.expression; // Cambia la expresión de la cara
        StartCoroutine(TypeLine(line.text));
    }

    IEnumerator TypeLine(string text)
    {
        isTyping = true;
        textDisplay.text = "";
        foreach (char c in text)
        {
            textDisplay.text += c;
            yield return new WaitForSeconds(0.02f);
        }
        isTyping = false;

        // Si hay opciones, las mostramos al terminar el texto
        if (currentData.lines[lineIndex].hasOptions) ShowOptions();
    }

    public void OnClickPanel() // Llamar desde un botón invisible en el panel
    {
        if (isTyping)
        {
            StopAllCoroutines();
            textDisplay.text = currentData.lines[lineIndex].text;
            isTyping = false;
            if (currentData.lines[lineIndex].hasOptions) ShowOptions();
        }
        else if (!currentData.lines[lineIndex].hasOptions)
        {
            NextLine();
        }
    }

    void NextLine()
    {
        lineIndex++;
        if (lineIndex < currentData.lines.Count) DisplayLine();
        else CloseDialogue();
    }

    void ShowOptions()
    {
        optionsParent.SetActive(true);
        foreach (Transform child in optionsParent.transform) Destroy(child.gameObject);

        foreach (var opt in currentData.lines[lineIndex].options)
        {
            GameObject btn = Instantiate(optionButtonPrefab, optionsParent.transform);
            btn.GetComponentInChildren<TextMeshProUGUI>().text = opt.optionText;
            btn.GetComponent<Button>().onClick.AddListener(() => {
                if (opt.nextDialogue != null) StartDialogue(opt.nextDialogue, currentNPC);
                else CloseDialogue();
            });
        }
    }

    public void CloseDialogue()
    {
        dialoguePanel.SetActive(false);
        portraitDisplay.gameObject.SetActive(false);
        currentNPC.ExitFocus();
    }
}