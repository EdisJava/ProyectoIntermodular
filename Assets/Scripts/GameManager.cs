using UnityEngine;

public enum GameState
{
    Exploration3D,
    Interaction2D
}

public enum DayPhase
{
    CasualTalk,     // Antes de encontrar al acosado
    Investigation,  // Acosado encontrado, puedes preguntar
    Decision        // Chivarse al profesor
}

public class GameManager : MonoBehaviour
{
    public static GameManager Instance;

    [Header("Estado del juego")]
    public GameState currentState = GameState.Exploration3D;
    public DayPhase currentDayPhase = DayPhase.CasualTalk;

    [Header("Días")]
    public int currentDay = 1;
    public int maxDays = 7;

    [Header("Investigación")]
    public int maxQuestionsPerDay = 2;
    private int questionsUsed;

    public int GetRemainingQuestions()
    {
        return maxQuestionsPerDay - questionsUsed;
    }


    [Header("Resultados")]
    public int goodDecisions = 0;
    public int badDecisions = 0;

    [Header("Configuración de la Historia")]
    public DayScenario[] allDays;

    private void Awake()
    {
        if (Instance == null)
            Instance = this;
        else
        {
            Destroy(gameObject);
            return;
        }

        StartNewDay();
    }

    // ---------------- ESTADOS ----------------

    public bool IsIn2D()
    {
        return currentState == GameState.Interaction2D;
    }

    // ---------------- DÍAS ----------------

    public bool hasAccusedThisDay = false; // Nueva variable

    [Header("Posicion Inicial")]
    public Transform playerSpawnPoint;
    public GameObject playerObject;
    void StartNewDay()
    {
        currentDayPhase = DayPhase.CasualTalk;
        questionsUsed = 0;
        hasAccusedThisDay = false;

        // 1. Obtenemos el escenario del nuevo día
        DayScenario scenario = GetCurrentDayScenario();

        if (scenario != null)
        {
            Debug.Log($"Configurando Día {currentDay}: {scenario.name}");

            // 2. Buscamos a todos los NPCs y les damos su nueva "hoja de ruta"
            StudentNPC[] allNPCs = FindObjectsOfType<StudentNPC>();
            foreach (StudentNPC npc in allNPCs)
            {
                npc.ResetMemory();
                npc.SetupCharacterForToday(); // Esta función ya la tienes en StudentNPC
            }

            TeacherNPC teacher = FindFirstObjectByType<TeacherNPC>();
            if (teacher != null)
            {
                teacher.ResetMemory();
                teacher.SetupTeacherForToday(); // Esta función ya la tienes en TeacherNPC
            }
        }
        else
        {
            Debug.LogError("No hay un DayScenario configurado para el día " + currentDay);
        }

        Debug.Log($"Día {currentDay} comienza oficialmente");

        if (playerSpawnPoint != null && playerObject != null)
        {
            playerObject.transform.position = playerSpawnPoint.position;
            playerObject.transform.rotation = playerSpawnPoint.rotation;
        }
    }

    // ---------------- ACOSADO ----------------

    public void FoundVictim()
    {
        if (currentDayPhase != DayPhase.CasualTalk)
            return;

        currentDayPhase = DayPhase.Investigation;
        questionsUsed = 0;

        Debug.Log("Has encontrado al acosado. Investigación desbloqueada.");
    }

    // ---------------- PREGUNTAS ----------------

    public bool CanAskQuestion()
    {
        return currentDayPhase == DayPhase.Investigation
            && questionsUsed < maxQuestionsPerDay;
    }

    public void UseQuestion()
    {
        questionsUsed++;
        Debug.Log($"Pregunta usada ({questionsUsed}/{maxQuestionsPerDay})");

        if (questionsUsed >= maxQuestionsPerDay)
        {
            currentDayPhase = DayPhase.Decision;
            Debug.Log("Ya puedes chivarte al profesor.");
        }
    }

    // ---------------- DECISIONES ----------------

    public void RegisterDecision(bool correct)
    {
        if (correct) goodDecisions++;
        else badDecisions++;

        hasAccusedThisDay = true; // <--- Marcamos que ya se hizo la acusación
        Debug.Log("Decisión registrada. Ahora puedes irte.");
    }


    public void NextDay()
    {
        if (currentDay >= maxDays)
        {
            EndGame();
            return;
        }

        currentDay++;
        StartNewDay();
    }

    void EndGame()
    {
        if (badDecisions > 3)
            Debug.Log("FINAL MALO");
        else
            Debug.Log("FINAL BUENO");
    }


    // Función auxiliar para que los alumnos sepan qué día es
    public DayScenario GetCurrentDayScenario()
    {
        // Restamos 1 porque el array empieza en 0 pero tus días en 1
        int index = currentDay - 1;

        if (index < allDays.Length)
            return allDays[index];

        return null;
    }

    public void ResetAllNPCs()
    {
        StudentNPC[] allNPCs = FindObjectsOfType<StudentNPC>();
        foreach (StudentNPC npc in allNPCs)
        {
            npc.ResetMemory();
        }
        TeacherNPC teacher = FindFirstObjectByType<TeacherNPC>();
        if (teacher != null)
        {
            teacher.ResetMemory();
        }
    }


}
