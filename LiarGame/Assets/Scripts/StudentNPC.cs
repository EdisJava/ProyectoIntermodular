using UnityEngine;
using UnityEngine.UI;

public class StudentNPC : MonoBehaviour
{
    public string studentName;
    public DialogueData casualDialogue;
    public DialogueData investigationDialogue;

    [Header("Sprites")]
    public Sprite idleSprite; // De espaldas / sentado
    public Transform centerPoint;

    private Image myImage;
    private Vector3 startPos;
    private Vector3 startScale;

    void Start()
    {
        myImage = GetComponent<Image>();
        startPos = transform.position;
        startScale = transform.localScale;
        if (idleSprite) myImage.sprite = idleSprite;
    }

    public void Interact()
    {
        if (!GameManager.Instance.IsIn2D()) return;

        // Si es fase casual: Habla gratis
        if (GameManager.Instance.currentDayPhase == DayPhase.CasualTalk)
        {
            DialogueManager.Instance.StartDialogue(casualDialogue, this);
        }
        // Si es investigación: ¡Gasta usos!
        else if (GameManager.Instance.currentDayPhase == DayPhase.Investigation)
        {
            if (GameManager.Instance.CanAskQuestion())
            {
                GameManager.Instance.UseQuestion();
                DialogueManager.Instance.StartDialogue(investigationDialogue, this);
            }
            else
            {
                Debug.Log("No te quedan preguntas por hoy.");
            }
        }
    }

    public void EnterFocus()
    {
        // Se mueve al centro y se hace grande
        transform.position = centerPoint.position;
        transform.localScale = startScale * 1.3f;
        // La imagen de la cara la gestiona el DialogueManager a través de las líneas
        // 2. ¡CLAVE!: Desactivamos su imagen de "escena" (espaldas) 
        // porque el DialogueManager ya va a mostrar la cara en el Portrait del Canvas
        myImage.enabled = false;
    }

    public void ExitFocus()
    {
        // Vuelve a su sitio y a su imagen original
        transform.position = startPos;
        transform.localScale = startScale;
        myImage.sprite = idleSprite;
        // 3. Volvemos a activar su imagen normal al terminar
        myImage.enabled = true;
        myImage.sprite = idleSprite;
    }

    public void SetupCharacterForToday()
    {
        // Por ahora lo dejamos vacío para que RoomManager no de error
        // Luego aquí pondremos la lógica de cargar los diálogos del día
    }
}