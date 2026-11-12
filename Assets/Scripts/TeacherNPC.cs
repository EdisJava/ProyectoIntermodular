using UnityEngine;
using UnityEngine.UI;

/*
* Script para manejar el comportamiento del profesor.
* 
* Metodos:
*   - Start(): Metodo que se ejecuta al iniciar el script.
*   - SetupTeacherForToday(): Metodo que configura el dialogo del profesor para el dia actual.
*   - Interact(): Metodo que se ejecuta al interactuar con el profesor.
*   - SetAccusedFlag(): Metodo que marca al profesor como acusado.
*   - BuildProgressData(): Metodo que construye los datos de progreso.
*   - ApplyProgressData(): Metodo que aplica los datos de progreso.
*   - ResetMemory(): Metodo que reinicia la memoria del profesor.
*   - EnterFocus(): Metodo que entra en estado de enfoque.
*   - ExitFocus(): Metodo que sale del estado de enfoque.
*
*   Variables:
*   - teacherName: Nombre del profesor.
*   - idleSprite: Sprite en estado de reposo.
*   - centerPoint: Punto central para el enfoque.
*   - myImage: Imagen del profesor.
*   - startPos: Posicion inicial.
*   - startScale: Escala inicial.
*   - currentCasual: Dialogo casual.
*   - currentDecision: Dialogo de decision.
*   - casualRead: Si se ha leido el dialogo casual.
*   - hasAccusedToday: Si se ha acusado al profesor hoy.
*   - postDecisionDialogue: Dialogo despues de la decision.
*
*   Funcionamiento:
*   - Controla el dialogo que se muestra segun la fase del dia y si se ha leido el dialogo casual.
*
*   Flujo:
*   1. El jugador interactua con el profesor.
*   2. Se determina la fase actual del dia.
*   3. Se llama al metodo correspondiente segun la fase.
*   4. Se muestra el dialogo del profesor.
*   5. El jugador puede interactuar con otro alumno.
*/

public class TeacherNPC : MonoBehaviour
{
    public string teacherName = "Profesor";

    [Header("Sprites y Foco")]
    public Sprite idleSprite;
    public Transform centerPoint;
    private Image myImage;
    private Vector3 startPos;
    private Vector3 startScale;

  
    private DialogueData currentCasual;
    private DialogueData currentDecision;
    private bool casualRead = false;
    
    private bool hasAccusedToday = false;
    public DialogueData postDecisionDialogue;



    void Start()
    {
        myImage = GetComponent<Image>();
        startPos = transform.position;
        startScale = transform.localScale;
        if (idleSprite) myImage.sprite = idleSprite;

        SetupTeacherForToday();
    }

    public void SetupTeacherForToday()
    {
        DayScenario today = GameManager.Instance.GetCurrentDayScenario();
        if (today == null || today.teacherName != this.teacherName) return;

        this.currentCasual = today.teacherCasualDialogue;
        this.currentDecision = today.teacherDecisionDialogue;
        this.postDecisionDialogue = today.postDecisionDialogue;
    }

    public void Interact()
    {
        if (Time.timeScale == 0f) return;

        if (DialogueManager.Instance.IsDialogueActive) return;
        if (!GameManager.Instance.IsIn2D()) return;

        // 1. PRIORIDAD maxima: Preguntar al GameManager si YA se ha acusado hoy
        // Usamos GameManager.Instance.hasAccusedThisDay en lugar de la variable local
        if (GameManager.Instance.hasAccusedThisDay)
        {
            Debug.Log("El profesor sabe que ya acusaste. Mostrando post-dialogue.");
            DialogueManager.Instance.StartDialogue(postDecisionDialogue, this);
            return;
        }


        if (!casualRead)
        {
            casualRead = true;
            DialogueManager.Instance.StartDialogue(currentCasual, this);
            return;
        }

 
        if (GameManager.Instance.currentDayPhase != DayPhase.Decision)
        {
            DialogueManager.Instance.StartDialogue(currentCasual, this);
        }

        else
        {
            DialogueManager.Instance.StartDialogue(currentDecision, this);
        }
    }

    public void SetAccusedFlag()
    {
        hasAccusedToday = true;
    }

    public TeacherDialogueProgressData BuildProgressData()
    {
        return new TeacherDialogueProgressData
        {
            teacherName = teacherName,
            casualRead = casualRead,
            hasAccusedToday = hasAccusedToday
        };
    }

    public void ApplyProgressData(TeacherDialogueProgressData data)
    {
        if (data == null || data.teacherName != teacherName)
        {
            return;
        }

        casualRead = data.casualRead;
        hasAccusedToday = data.hasAccusedToday;
    }

    public void ResetMemory()
    {
        casualRead = false;
        hasAccusedToday = false; 
    }

    public void EnterFocus()
    {
        transform.position = centerPoint.position;
        transform.localScale = startScale * 1.3f;
        myImage.enabled = false;
    }

    public void ExitFocus()
    {
        transform.position = startPos;
        transform.localScale = startScale;
        myImage.enabled = true;
    }
}
