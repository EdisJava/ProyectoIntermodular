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

    // DiÃ¡logos que se cargarÃ¡n dinÃ¡micamente
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

        // 1. PRIORIDAD MÃXIMA: Preguntar al GameManager si YA se ha acusado hoy
        // Usamos GameManager.Instance.hasAccusedThisDay en lugar de la variable local
        if (GameManager.Instance.hasAccusedThisDay)
        {
            Debug.Log("El profesor sabe que ya acusaste. Mostrando post-dialogue.");
            DialogueManager.Instance.StartDialogue(postDecisionDialogue, this);
            return;
        }

        // 2. Si NO se ha acusado, miramos si es la primera vez que hablamos con Ã©l hoy
        if (!casualRead)
        {
            casualRead = true;
            DialogueManager.Instance.StartDialogue(currentCasual, this);
            return;
        }

        // 3. Si ya hablamos pero NO estamos en fase de decisiÃ³n
        if (GameManager.Instance.currentDayPhase != DayPhase.Decision)
        {
            DialogueManager.Instance.StartDialogue(currentCasual, this);
        }
        // 4. Si estamos en fase de decisiÃ³n (aquÃ­ es donde se abre el menÃº de acusar)
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
        hasAccusedToday = false; // Importante resetear esto tambiÃ©n al cambiar de dÃ­a
    }

    // Usamos la misma lÃ³gica de foco que tus alumnos para que no de error
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
