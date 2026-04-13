using UnityEngine;
using TMPro;
using UnityEngine.UI;
using System.Collections;

/*
* Script para manejar los dialogos.
* 
* Metodos:
*   - StartDialogue(): Metodo que inicia el dialogo.
*   - CleanPreviousFocus(): Metodo que limpia el foco anterior.
*   - DisplayLine(): Metodo que muestra la linea actual.
*   - TypeLine(): Metodo que escribe la linea actual.
*   - OnClickPanel(): Metodo que se llama al hacer clic en el panel.
*   - NextLine(): Metodo que pasa a la siguiente linea.
*   - ShowOptions(): Metodo que muestra las opciones.
*   - CloseDialogue(): Metodo que cierra el dialogo.
*
*   Variables:
*   - dialoguePanel: Panel que muestra el dialogo.
*   - textDisplay: Texto que muestra la linea actual.
*   - nameDisplay: Texto que muestra el nombre del personaje.
*   - portraitDisplay: Imagen que muestra la expresion del personaje.
*   - optionsParent: Panel que muestra las opciones.
*   - optionButtonPrefab: Prefab que crea los botones de las opciones.
*   - currentData: Datos del dialogo actual.
*   - lineIndex: Indice de la linea actual.
*   - isTyping: Indica si se esta escribiendo la linea actual.
*   - currentNPC: NPC actual.
*   - currentTeacher: Profesor actual.
*   - fxSource: Fuente de sonido.
*   - backgroundDisplay: Imagen de fondo.
*
*   Funcionamiento:
*   - Al iniciar, limpia el foco anterior y muestra la linea actual.
*   - Al hacer clic en el panel, pasa a la siguiente linea o muestra las opciones.
*   - Al cerrar el dialogo, limpia el foco anterior.
*
*   Flujo:
*   1. El jugador interactua con el alumno.
*   2. Se llama al metodo StartDialogue().
*   3. Se determina la fase actual del dia.
*   4. Se llama al metodo correspondiente segun la fase.
*   5. Se muestra el dialogo del alumno.
*   6. El jugador puede interactuar con otro alumno.
*/

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
    public Image backgroundDisplay;   // imagen de fondo por cada linea 


    public bool IsDialogueActive => dialoguePanel != null && dialoguePanel.activeSelf;

    void Awake() => Instance = this;

    /*
    * Metodo que inicia el dialogo con un alumno.
    */
    public void StartDialogue(DialogueData data, StudentNPC npc)
    {
        CleanPreviousFocus(); // limpia lo que hubiera antes

        currentData = data;
        currentNPC = npc;
        lineIndex = 0;

        dialoguePanel.SetActive(true);
        portraitDisplay.gameObject.SetActive(true);
        optionsParent.SetActive(false);

        if (currentNPC != null) currentNPC.EnterFocus();

        DisplayLine();
    }

    /*
    * Metodo que inicia el dialogo con un profesor.
    */
    public void StartDialogue(DialogueData data, TeacherNPC teacher)
    {
        CleanPreviousFocus();

        currentData = data;
        currentTeacher = teacher; // guarda al profe
        lineIndex = 0;

        dialoguePanel.SetActive(true);
        portraitDisplay.gameObject.SetActive(true);
        optionsParent.SetActive(false);

        if (currentTeacher != null) currentTeacher.EnterFocus(); 

        DisplayLine();
    }

    /*
    * Metodo que limpia el foco anterior.
    */
    private void CleanPreviousFocus()
    {
        if (currentNPC != null) currentNPC.ExitFocus();
        if (currentTeacher != null) currentTeacher.ExitFocus();

        currentNPC = null;
        currentTeacher = null;
    }

    /*
    * Metodo que muestra la linea actual.
    */
    void DisplayLine()
    {
        StopAllCoroutines();
        var line = currentData.lines[lineIndex];

        nameDisplay.text = line.characterName;
        portraitDisplay.sprite = line.expression; //expresion del persoaje

        // Logica de audio
        if (fxSource != null)
        {
            // para cualquier sonido que estuviera sonando de la frase anterior
            fxSource.Stop();

            //si la nueva frase tiene un sonido asignado
            if (line.lineSound != null)
            {
                // asigna el clip y lo reproduce
                fxSource.clip = line.lineSound;
                fxSource.Play();
            }
        }

        if (backgroundDisplay != null)
        {
            if (line.backgroundOverride != null)
            {
                // si la linea tiene imagen, activa el objeto y la pone
                backgroundDisplay.gameObject.SetActive(true);
                backgroundDisplay.sprite = line.backgroundOverride;
            }
            else
            {
                // si la linea no tiene imagen, lo apaga para ver el 3D 
               
                backgroundDisplay.gameObject.SetActive(false);
            }
        }

        StartCoroutine(TypeLine(line.text));
    }

    /*
    * Metodo que escribe la linea actual.
    */
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

        // si hay opciones, las muestra al terminar el texto
        if (currentData.lines[lineIndex].hasOptions) ShowOptions();
    }

    /*
    * Metodo que se llama al hacer clic en el panel.
    */
    public void OnClickPanel() // llamar desde un boton invisible en el panel. 
    //si le da click cuadno el personaje escribe, para la corrutina y muestra toda la linea. 
    //si le da click cuando el personaje ya termino de escribir y no hay opciones, pasa a la siguiente linea.
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

    /*
    * Metodo que pasa a la siguiente linea.
    */
    void NextLine() // pasa a la siguiente linea (si es que hay) o cierra el dialogo si no hay mas lineas
    {
        lineIndex++;
        if (lineIndex < currentData.lines.Count) DisplayLine();
        else CloseDialogue();
    }

    /*
    * Metodo que muestra las opciones.
    */
    void ShowOptions() // muestra las opciones de la linea actual, creando un boton por cada una y asignandole su texto y funcionalidad
    {
        optionsParent.SetActive(true);
        // limpia cualquier opcion que hubiera antes
        foreach (Transform child in optionsParent.transform) Destroy(child.gameObject);
        
        // por cada opcion, crea un boton y le asigna su texto y funcionalidad
        foreach (var opt in currentData.lines[lineIndex].options)
        {
            // crea el boton y le pone el texto de la opcion
            GameObject btn = Instantiate(optionButtonPrefab, optionsParent.transform);
            btn.GetComponentInChildren<TextMeshProUGUI>().text = opt.optionText;
            DialogueOption currentOpt = opt;
            // asigna la funcionalidad del boton segun la opcion
            btn.GetComponent<Button>().onClick.AddListener(() => {
                
                string optionTextNormalized = currentOpt.optionText != null ? currentOpt.optionText.ToLowerInvariant() : string.Empty;
                bool noMoreQuestionsOption = optionTextNormalized.Contains("no tengo mas preguntas")
                    || optionTextNormalized.Contains("no tengo mas preguntas");
                // si es una decision final del profesor, registra la decision y avisa al profesor para que cambie su dialogo
                if (currentOpt.isFinalDecision)
                {
                    GameManager.Instance.RegisterDecision(currentOpt.isCorrectAccusation);
                    TeacherNPC teacher = Object.FindFirstObjectByType<TeacherNPC>();
                    if (teacher != null) teacher.SetAccusedFlag();
                }
                // si es una opcion de interrogatorio, consume un uso de interrogatorio y marca al npc como interrogado para que cambie su dialogo
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
                /*
                Vale
                */
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
                // si la opcion tiene un siguiente dialogo asignado, seguimos con ese dialogo. Si no, termina el dialogo
                if (currentOpt.nextDialogue != null)
                {
                    // si tiene un profesor hablando, sigue con el profesor (esto es para arreglar el bug de que salga un alumno al hablar con el profesor)
                    if (currentTeacher != null)
                    {
                        StartDialogue(currentOpt.nextDialogue, currentTeacher);
                    }
                    // si no, sigue con el alumno
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
    // cierra el dialogo, apagando el panel y la imagen del personaje, y reseteando los npcs para que dejen de estar en focus
    public void CloseDialogue()
    {
        dialoguePanel.SetActive(false);
        portraitDisplay.gameObject.SetActive(false);

        if (currentNPC != null) currentNPC.ExitFocus(); // apaga al npc si estaba hablando
        if (currentTeacher != null) currentTeacher.ExitFocus(); // apaga al profe si estaba hablando

        currentNPC = null;
        currentTeacher = null;
    }
}