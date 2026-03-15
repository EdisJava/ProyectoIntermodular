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

        if (SaveSystem.HasSave())
        {
            SaveData saveData = SaveSystem.LoadGame();
            if (saveData != null)
            {
                ApplyLoadedGameState(saveData);
                return;
            }
        }

        StartNewGameState();
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
    void StartNewGameState()
    {
        currentDay = 1;
        currentState = GameState.Exploration3D;
        goodDecisions = 0;
        badDecisions = 0;
        ResetDailyState();
        RefreshCurrentDay(true);
    }

    void ApplyLoadedGameState(SaveData data)
    {
        ApplySaveData(data);
        RefreshCurrentDay(false);
    }

    void ResetDailyState()
    {
        currentDayPhase = DayPhase.CasualTalk;
        questionsUsed = 0;
        hasAccusedThisDay = false;
    }

    void RefreshCurrentDay(bool resetDailyMemory)
    {
        DayScenario scenario = GetCurrentDayScenario();

        if (scenario != null)
        {
            Debug.Log($"Configurando Día {currentDay}: {scenario.name}");

            StudentNPC[] allNPCs = FindObjectsOfType<StudentNPC>();
            foreach (StudentNPC npc in allNPCs)
            {
                if (resetDailyMemory)
                {
                    npc.ResetMemory();
                }

                npc.SetupCharacterForToday();
            }

            TeacherNPC teacher = FindFirstObjectByType<TeacherNPC>();
            if (teacher != null)
            {
                if (resetDailyMemory)
                {
                    teacher.ResetMemory();
                }

                teacher.SetupTeacherForToday();
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
        if (currentDay >= GetPlayableDayCount())
        {
            EndGame();
            return;
        }

        currentDay++;
        ResetDailyState();
        RefreshCurrentDay(true);
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

    public void SetRemainingQuestions(int remaining)
    {
        int clampedRemaining = Mathf.Clamp(remaining, 0, maxQuestionsPerDay);
        questionsUsed = maxQuestionsPerDay - clampedRemaining;
    }

    public SaveData BuildSaveData()
    {
        return new SaveData
        {
            currentDay = currentDay,
            currentDayPhase = (int)currentDayPhase,
            remainingQuestions = GetRemainingQuestions(),
            goodDecisions = goodDecisions,
            badDecisions = badDecisions,
            hasAccusedThisDay = hasAccusedThisDay
        };
    }

    public void ApplySaveData(SaveData data)
    {
        if (data == null)
        {
            Debug.LogWarning("No se pudo aplicar la partida guardada porque los datos son nulos.");
            StartNewGameState();
            return;
        }

        currentDay = Mathf.Clamp(data.currentDay, 1, GetPlayableDayCount());
        currentState = GameState.Exploration3D;
        goodDecisions = Mathf.Max(0, data.goodDecisions);
        badDecisions = Mathf.Max(0, data.badDecisions);
        hasAccusedThisDay = data.hasAccusedThisDay;
        SetRemainingQuestions(data.remainingQuestions);

        if (System.Enum.IsDefined(typeof(DayPhase), data.currentDayPhase))
        {
            currentDayPhase = (DayPhase)data.currentDayPhase;
        }
        else
        {
            currentDayPhase = DayPhase.CasualTalk;
        }
    }

    int GetPlayableDayCount()
    {
        if (allDays == null || allDays.Length == 0)
        {
            return 1;
        }

        return Mathf.Min(maxDays, allDays.Length);
    }


}

