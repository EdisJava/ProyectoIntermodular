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
        if (today == null) return;

        // Estos campos los añadiremos ahora al DayScenario
        this.currentCasual = today.teacherCasualDialogue;
        this.currentDecision = today.teacherDecisionDialogue;
    }

    public void Interact()
    {
        if (DialogueManager.Instance.IsDialogueActive) return;
        if (!GameManager.Instance.IsIn2D()) return;

        if (hasAccusedToday)
        {
            // CAMBIA EL NULL POR "this"
            DialogueManager.Instance.StartDialogue(postDecisionDialogue, this);
            return;
        }

        if (!casualRead)
        {
            casualRead = true;
            DialogueManager.Instance.StartDialogue(currentCasual, this); // CAMBIA EL NULL POR "this"
            return;
        }

        if (GameManager.Instance.currentDayPhase != DayPhase.Decision)
        {
            DialogueManager.Instance.StartDialogue(currentCasual, this); // CAMBIA EL NULL POR "this"
        }
        else
        {
            DialogueManager.Instance.StartDialogue(currentDecision, this); // CAMBIA EL NULL POR "this"
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