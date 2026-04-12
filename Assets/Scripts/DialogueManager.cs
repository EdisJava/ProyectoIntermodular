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
    public AudioSource fxSource;      // aqui va el audiosource para que se escuchen los audios 
    public Image backgroundDisplay;   // imagen de fondo por cada linea para que hagamos el final 


    public bool IsDialogueActive => dialoguePanel != null && dialoguePanel.activeSelf;

    void Awake() => Instance = this;

    public void StartDialogue(DialogueData data, StudentNPC npc)
    {
        CleanPreviousFocus(); // limpiamos lo que hubiera antes

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
        currentTeacher = teacher; // guardamos al profe
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
        portraitDisplay.sprite = line.expression; //expresion del persoaje

        // Logica de audio
        if (fxSource != null)
        {
            // paramos cualquier sonido que estuviera sonando de la frase anterior
            fxSource.Stop();

            //si la nueva frase tiene un sonido asignado-
            if (line.lineSound != null)
            {
                // -asignamos el clip y lo reproducimos
                fxSource.clip = line.lineSound;
                fxSource.Play();
            }
        }

        if (backgroundDisplay != null)
        {
            if (line.backgroundOverride != null)
            {
                // si la linea tiene imagen, activamos el objeto y la ponemos
                backgroundDisplay.gameObject.SetActive(true);
                backgroundDisplay.sprite = line.backgroundOverride;
            }
            else
            {
                // si la linea no tiene imagen, lo apagamos para ver el 3D 
               
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

        // si hay opciones, las mostramos al terminar el texto
        if (currentData.lines[lineIndex].hasOptions) ShowOptions();
    }

    public void OnClickPanel() // llamar desde un boton invisible en el panel. 
    //si le das click cuadno el personaje escribe, para la corrutina y muestra toda la linea. 
    //si le das click cuando el personaje ya termino de escribir y no hay opciones, pasa a la siguiente linea.
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


    void NextLine() // pasamos a la siguiente linea (si es que hay) o cerramos el dialogo si no hay mas lineas
    {
        lineIndex++;
        if (lineIndex < currentData.lines.Count) DisplayLine();
        else CloseDialogue();
    }

    void ShowOptions() // mostramos las opciones de la linea actual, creando un boton por cada una y asignandole su texto y funcionalidad
    {
        optionsParent.SetActive(true);
        // limpiamos cualquier opcion que hubiera antes
        foreach (Transform child in optionsParent.transform) Destroy(child.gameObject);
        
        // por cada opcion, creamos un boton y le asignamos su texto y funcionalidad
        foreach (var opt in currentData.lines[lineIndex].options)
        {
            // creamos el boton y le ponemos el texto de la opcion
            GameObject btn = Instantiate(optionButtonPrefab, optionsParent.transform);
            btn.GetComponentInChildren<TextMeshProUGUI>().text = opt.optionText;
            DialogueOption currentOpt = opt;
            // asignamos la funcionalidad del boton segun la opcion
            btn.GetComponent<Button>().onClick.AddListener(() => {
                
                string optionTextNormalized = currentOpt.optionText != null ? currentOpt.optionText.ToLowerInvariant() : string.Empty;
                bool noMoreQuestionsOption = optionTextNormalized.Contains("no tengo más preguntas")
                    || optionTextNormalized.Contains("no tengo mas preguntas");
                // si es una decision final del profesor, registramos la decision y avisamos al profesor para que cambie su dialogo
                if (currentOpt.isFinalDecision)
                {
                    GameManager.Instance.RegisterDecision(currentOpt.isCorrectAccusation);
                    TeacherNPC teacher = Object.FindFirstObjectByType<TeacherNPC>();
                    if (teacher != null) teacher.SetAccusedFlag();
                }
                // si es una opcion de interrogatorio, consumimos un uso de interrogatorio y marcamos al npc como interrogado para que cambie su dialogo
                if (currentOpt.isInterrogation)
                {
                    GameManager.Instance.UseQuestion();
                    if (currentNPC != null) currentNPC.MarkAsInterrogated();
                }
                // cerramos las opciones
                optionsParent.SetActive(false);
                /* si la opcion es "no tengo mas preguntas", y tenemos un npc hablando, mostramos su dialogo de fallback (si lo tiene) en vez de seguir con la siguiente linea del dialogo
                esto es para que el npc deje de hablar de la investigacion y vuelva a su dialogo casual si el jugador decide no seguir interrogando
                aunque seguramente no lo utilicemos en el juego final, lo dejo por si queremos hacer algo parecido en alguna parte*/
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
                // si la opcion tiene un siguiente dialogo asignado, seguimos con ese dialogo. Si no, cerramos el dialogo
                if (currentOpt.nextDialogue != null)
                {
                    // si tenemos un profesor hablando, seguimos con el profesor (esto es para arreglar el bug de que salga un alumno al hablar con el profesor)
                    if (currentTeacher != null)
                    {
                        StartDialogue(currentOpt.nextDialogue, currentTeacher);
                    }
                    // si no, seguimos con el alumno
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
    // cerramos el dialogo, apagando el panel y la imagen del personaje, y reseteando los npcs para que dejen de estar en focus
    public void CloseDialogue()
    {
        dialoguePanel.SetActive(false);
        portraitDisplay.gameObject.SetActive(false);

        if (currentNPC != null) currentNPC.ExitFocus(); // apagamos al npc si estaba hablando
        if (currentTeacher != null) currentTeacher.ExitFocus(); // apagamos al profe si estaba hablando

        currentNPC = null;
        currentTeacher = null;
    }
}