using UnityEngine;

public class StudentNPC : MonoBehaviour
{
    public string studentName; // Ej: "Juan", "Maria"

    // Variables privadas que se llenarán automáticamente
    private string currentCasualText;
    private string currentInvestigationText;
    private bool isVictim;

    // Referencia al Manager para avisar de clicks
    private GameManager gameManager;

    private void Start()
    {
        gameManager = GameManager.Instance;
        SetupCharacterForToday();
    }

    // Esta función configura al personaje según el día actual
    public void SetupCharacterForToday()
    {
        // 1. Obtenemos los datos del día actual del GameManager (veremos esto en el paso 3)
        DayScenario today = gameManager.GetCurrentDayScenario();

        if (today == null) return;

        // 2. Comprobar si soy la víctima
        isVictim = (studentName == today.victimName);

        // 3. Buscar mis diálogos para hoy
        foreach (var dialogue in today.characterDialogues)
        {
            if (dialogue.characterName == studentName)
            {
                currentCasualText = dialogue.casualText;

                // Si el personaje miente hoy, su texto de investigación será la mentira
                if (dialogue.isLiarToday)
                {
                    currentInvestigationText = dialogue.lieText;
                }
                else
                {
                    currentInvestigationText = dialogue.truthText;
                }
                break;
            }
        }
    }

    // Detectar click (necesitas un Collider2D en el personaje)
    private void OnMouseDown()
    {
        // Si el juego no está en modo 2D, ignorar
        if (!gameManager.IsIn2D()) return;

        Interact();
    }

    // Cambia el método Interact a public para que el botón lo vea
    public void Interact()
    {
        // Si el juego no está en modo 2D, no hacer nada
        if (!GameManager.Instance.IsIn2D()) return;

        // CASO 1: Fase de Charla Casual
        if (GameManager.Instance.currentDayPhase == DayPhase.CasualTalk)
        {
            Debug.Log(studentName + ": " + currentCasualText);

            if (isVictim)
            {
                GameManager.Instance.FoundVictim();
            }
        }
        // CASO 2: Fase de Investigación
        else if (GameManager.Instance.currentDayPhase == DayPhase.Investigation)
        {
            if (isVictim) return;

            if (GameManager.Instance.CanAskQuestion())
            {
                GameManager.Instance.UseQuestion();
                Debug.Log(studentName + " responde: " + currentInvestigationText);
            }
        }
    }
}