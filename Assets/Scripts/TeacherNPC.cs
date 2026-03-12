using UnityEngine;
using UnityEngine.UI;

public class TeacherNPC : MonoBehaviour
{
    public string teacherName = "Profesor";

    [Header("Sprites y Foco")]
    public Sprite idleSprite;
    public Transform centerPoint;
    private Image myImage;
    private Vector3 startPos;
    private Vector3 startScale;

    // Diálogos que se cargarán dinámicamente
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
        if (DialogueManager.Instance.IsDialogueActive) return;
        if (!GameManager.Instance.IsIn2D()) return;

        // 1. PRIORIDAD MÁXIMA: Preguntar al GameManager si YA se ha acusado hoy
        // Usamos GameManager.Instance.hasAccusedThisDay en lugar de la variable local
        if (GameManager.Instance.hasAccusedThisDay)
        {
            Debug.Log("El profesor sabe que ya acusaste. Mostrando post-dialogue.");
            DialogueManager.Instance.StartDialogue(postDecisionDialogue, this);
            return;
        }

        // 2. Si NO se ha acusado, miramos si es la primera vez que hablamos con él hoy
        if (!casualRead)
        {
            casualRead = true;
            DialogueManager.Instance.StartDialogue(currentCasual, this);
            return;
        }

        // 3. Si ya hablamos pero NO estamos en fase de decisión
        if (GameManager.Instance.currentDayPhase != DayPhase.Decision)
        {
            DialogueManager.Instance.StartDialogue(currentCasual, this);
        }
        // 4. Si estamos en fase de decisión (aquí es donde se abre el menú de acusar)
        else
        {
            DialogueManager.Instance.StartDialogue(currentDecision, this);
        }
    }

    public void SetAccusedFlag()
    {
        hasAccusedToday = true;
    }

    public void ResetMemory()
    {
        casualRead = false;
        hasAccusedToday = false; // Importante resetear esto también al cambiar de día
    }

    // Usamos la misma lógica de foco que tus alumnos para que no de error
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