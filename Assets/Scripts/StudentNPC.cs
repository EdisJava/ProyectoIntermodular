using UnityEngine;
using UnityEngine.UI;

public class StudentNPC : MonoBehaviour
{
    public string studentName;
    public DialogueData casualDialogue;
    public DialogueData investigationDialogue;

    [Header("Sprites")]
    public Sprite idleSprite;
    public Transform centerPoint;

    private Image myImage;
    private RectTransform myRectTransform;
    private Vector2 startAnchoredPos;
    private Vector3 startScale;

    [Header("Diálogos Especiales")]
    public DialogueData alreadyInterrogatedDialogue;

    [Header("Diálogos de Víctima")]
    public DialogueData victimStateDialogue;

    private bool victimFound = false;
    private bool alreadyInterrogated = false; // Control de si ya soltó la pista
    public bool isVictim;

    void Start()
    {
        myImage = GetComponent<Image>();
        myRectTransform = GetComponent<RectTransform>();
        startAnchoredPos = myRectTransform.anchoredPosition;
        startScale = transform.localScale;
        if (idleSprite) myImage.sprite = idleSprite;
    }

    private bool casualRead = false;
    public void Interact()
    {
        if (Time.timeScale == 0f) return;

        if (DialogueManager.Instance.IsDialogueActive) return;

        if (!GameManager.Instance.IsIn2D()) return;

        // --- LÓGICA DE VÍCTIMA ---
        if (isVictim)
        {
            if (!victimFound)
            {
                victimFound = true;
                DialogueManager.Instance.StartDialogue(casualDialogue, this);
                GameManager.Instance.FoundVictim();
            }
            else
            {
                DialogueData nextD = victimStateDialogue != null ? victimStateDialogue : casualDialogue;
                DialogueManager.Instance.StartDialogue(nextD, this);
            }
            return;
        }

        // --- LÓGICA DE ALUMNOS ---

        // CASO A: Aún no has leído su diálogo casual (Prioridad máxima)
        // Saldrá este diálogo tanto en fase CasualTalk como en Investigation
        if (!casualRead)
        {
            casualRead = true; // La próxima vez ya pasará a la siguiente lógica
            DialogueManager.Instance.StartDialogue(casualDialogue, this);
            return;
        }

        // CASO B: Ya leíste el casual, pero aún no estamos en investigación
        if (GameManager.Instance.currentDayPhase == DayPhase.CasualTalk)
        {
            DialogueManager.Instance.StartDialogue(casualDialogue, this);
            return;
        }

        // CASO C: Ya leíste el casual y estamos en investigación
        if (GameManager.Instance.currentDayPhase == DayPhase.Investigation ||
            GameManager.Instance.currentDayPhase == DayPhase.Decision)
        {
            if (alreadyInterrogated)
            {
                DialogueData nextD = alreadyInterrogatedDialogue != null ? alreadyInterrogatedDialogue : casualDialogue;
                DialogueManager.Instance.StartDialogue(nextD, this);
            }
            else
            {
                // Ahora sí, después de haber leído el casual una vez, sale la investigación
                DialogueManager.Instance.StartDialogue(investigationDialogue, this);
            }
        }
    }

    // Esta función la llama el DialogueManager cuando eliges una opción con isInterrogation = true
    public void MarkAsInterrogated()
    {
        alreadyInterrogated = true;
    }

    public StudentDialogueProgressData BuildProgressData()
    {
        return new StudentDialogueProgressData
        {
            studentName = studentName,
            casualRead = casualRead,
            alreadyInterrogated = alreadyInterrogated,
            victimFound = victimFound
        };
    }

    public void ApplyProgressData(StudentDialogueProgressData data)
    {
        if (data == null || data.studentName != studentName)
        {
            return;
        }

        casualRead = data.casualRead;
        alreadyInterrogated = data.alreadyInterrogated;
        victimFound = data.victimFound;
    }

    public void EnterFocus()
    {
        // Si el centerPoint es un objeto de la UI, usamos su posición anclada
        RectTransform centerRect = centerPoint.GetComponent<RectTransform>();

        if (centerRect != null)
        {
            myRectTransform.anchoredPosition = centerRect.anchoredPosition;
        }
        else
        {
            // Si por alguna razón no es UI, seguimos usando position pero es menos estable
            transform.position = centerPoint.position;
        }

        transform.localScale = startScale * 1.3f;
        // Ocultar todo el objeto en lugar de solo la imagen previene que queden hijos o textos visibles flotando (como el bug de Aroy que se duplica visualmente)
        gameObject.SetActive(false);
    }
    public void ExitFocus()
    {
        // Volvemos a la posición anclada original (que no cambia con la resolución)
        myRectTransform.anchoredPosition = startAnchoredPos; // <--- CAMBIADO

        transform.localScale = startScale;
        gameObject.SetActive(true);
    }

    public void ResetMemory()
    {
        alreadyInterrogated = false;
        victimFound = false;
        casualRead = false;
    }

    public void SetupCharacterForToday()
    {
        

        DayScenario today = GameManager.Instance.GetCurrentDayScenario();
        if (today == null) return;

        isVictim = (studentName == today.victimName);
        if (isVictim) Debug.Log(studentName + " es la víctima hoy.");

        foreach (var config in today.characterConfigs)
        {
            if (config.characterName == studentName)
            {
                this.casualDialogue = config.casualDialogue;
                if (!isVictim)
                {
                    this.investigationDialogue = config.isLiarToday ? config.lieDialogue : config.truthDialogue;
                }
                break;
            }
        }
    }
}
