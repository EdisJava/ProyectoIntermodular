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
    private TeacherNPC currentTeacher;

    [Header("Final y Efectos")]
    public AudioSource fxSource;      // Arrastra un AudioSource aquí
    public Image backgroundDisplay;   // La imagen de fondo de tu escena final


    public bool IsDialogueActive => dialoguePanel != null && dialoguePanel.activeSelf;

    void Awake() => Instance = this;

    public void StartDialogue(DialogueData data, StudentNPC npc)
    {
        CleanPreviousFocus(); // Limpiamos lo que hubiera antes

        currentData = data;
        currentNPC = npc;
        lineIndex = 0;

        dialoguePanel.SetActive(true);
        portraitDisplay.gameObject.SetActive(true);
        optionsParent.SetActive(false);

        if (currentNPC != null) currentNPC.EnterFocus();

        DisplayLine();
    }

    public void StartDialogue(DialogueData data, TeacherNPC teacher)
    {
        CleanPreviousFocus();

        currentData = data;
        currentTeacher = teacher; // Guardamos al profe
        lineIndex = 0;

        dialoguePanel.SetActive(true);
        portraitDisplay.gameObject.SetActive(true);
        optionsParent.SetActive(false);

        if (currentTeacher != null) currentTeacher.EnterFocus(); 

        DisplayLine();
    }

   
    private void CleanPreviousFocus()
    {
        if (currentNPC != null) currentNPC.ExitFocus();
        if (currentTeacher != null) currentTeacher.ExitFocus();

        currentNPC = null;
        currentTeacher = null;
    }

    void DisplayLine()
    {
        StopAllCoroutines();
        var line = currentData.lines[lineIndex];

        nameDisplay.text = line.characterName;
        portraitDisplay.sprite = line.expression; // Cambia la expresión de la cara

        // --- LÓGICA DE AUDIO CORREGIDA ---
        if (fxSource != null)
        {
            // 1. Paramos cualquier sonido que estuviera sonando de la frase anterior
            fxSource.Stop();

            // 2. Si la nueva frase tiene un sonido asignado...
            if (line.lineSound != null)
            {
                // Asignamos el clip y lo reproducimos
                fxSource.clip = line.lineSound;
                fxSource.Play();
            }
        }

        if (backgroundDisplay != null)
        {
            if (line.backgroundOverride != null)
            {
                // Si la línea tiene imagen, activamos el objeto y la ponemos
                backgroundDisplay.gameObject.SetActive(true);
                backgroundDisplay.sprite = line.backgroundOverride;
            }
            else
            {
                // Si la línea NO tiene imagen, lo apagamos para ver el 3D 
                // (En la escena de la casa, pon imagen a todas las líneas para que no parpadee)
                backgroundDisplay.gameObject.SetActive(false);
            }
        }


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
        if (Time.timeScale == 0f)
        {
            return;
        }

        if (optionsParent.activeSelf) return;

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
            DialogueOption currentOpt = opt;

            btn.GetComponent<Button>().onClick.AddListener(() => {

                string optionTextNormalized = currentOpt.optionText != null ? currentOpt.optionText.ToLowerInvariant() : string.Empty;
                bool noMoreQuestionsOption = optionTextNormalized.Contains("no tengo más preguntas")
                    || optionTextNormalized.Contains("no tengo mas preguntas");

                if (currentOpt.isFinalDecision)
                {
                    GameManager.Instance.RegisterDecision(currentOpt.isCorrectAccusation);
                    TeacherNPC teacher = Object.FindFirstObjectByType<TeacherNPC>();
                    if (teacher != null) teacher.SetAccusedFlag();
                }

                if (currentOpt.isInterrogation)
                {
                    GameManager.Instance.UseQuestion();
                    if (currentNPC != null) currentNPC.MarkAsInterrogated();
                }

                optionsParent.SetActive(false);

                if (currentNPC != null && noMoreQuestionsOption)
                {
                    DialogueData fallback = currentNPC.alreadyInterrogatedDialogue != null
                        ? currentNPC.alreadyInterrogatedDialogue
                        : currentNPC.casualDialogue;

                    if (fallback != null)
                    {
                        StartDialogue(fallback, currentNPC);
                        return;
                    }
                }

                if (currentOpt.nextDialogue != null)
                {
                    // Si hay un profesor hablando, seguimos con el profesor
                    if (currentTeacher != null)
                    {
                        StartDialogue(currentOpt.nextDialogue, currentTeacher);
                    }
                    // Si no, seguimos con el alumno
                    else
                    {
                        StartDialogue(currentOpt.nextDialogue, currentNPC);
                    }
                }
                else
                {
                    CloseDialogue();
                }
            });
        }
    }

    public void CloseDialogue()
    {
        dialoguePanel.SetActive(false);
        portraitDisplay.gameObject.SetActive(false);

        if (currentNPC != null) currentNPC.ExitFocus();
        if (currentTeacher != null) currentTeacher.ExitFocus(); // Apagamos al profe si estaba hablando

        currentNPC = null;
        currentTeacher = null;
    }
}