using UnityEngine;
using UnityEngine.SceneManagement;

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

    [Header("D�as")]
    public int currentDay = 1;
    public int maxDays = 5;

    [Header("Investigaci�n")]
    public int maxQuestionsPerDay = 2;
    private int questionsUsed;

    public int GetMaxQuestions()
    {
        return currentDay == 3 ? 5 : maxQuestionsPerDay;
    }

    public int GetRemainingQuestions()
    {
        return GetMaxQuestions() - questionsUsed;
    }

    [Header("Resultados")]
    public int goodDecisions = 0;
    public int badDecisions = 0;
    public bool isEndingPhase = false;
    public bool isGoodEnding = false;

    [Header("Configuracin de la Historia")]
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

    // ---------------- DAS ----------------

    public bool hasAccusedThisDay = false; // Nueva variable
    public bool hasReadPrologueLetter = false;

    [Header("Posicion Inicial")]
    public Transform playerSpawnPoint;
    public GameObject playerObject;
    private bool hasLoadedPlayerPosition = false;
    private Vector3 loadedPlayerPosition;
    private bool hasLoadedPlayerRotation = false;
    private Quaternion loadedPlayerRotation;
    void StartNewGameState()
    {
        currentDay = 1;
        currentState = GameState.Exploration3D;
        goodDecisions = 0;
        badDecisions = 0;
        isEndingPhase = false;
        isGoodEnding = false;
        hasReadPrologueLetter = false;
        hasLoadedPlayerPosition = false;
        hasLoadedPlayerRotation = false;
        ResetDailyState();
        RefreshCurrentDay(true);
    }

    void ApplyLoadedGameState(SaveData data)
    {
        ApplySaveData(data);
        RefreshCurrentDay(false);
        ApplyDialogueProgress(data);
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
            Debug.Log($"Configurando Da {currentDay}: {scenario.name}");

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
            Debug.LogError("No hay un DayScenario configurado para el da " + currentDay);
        }

        Debug.Log($"Da {currentDay} comienza oficialmente");

        if (playerObject != null)
        {
            if (hasLoadedPlayerPosition)
            {
                playerObject.transform.position = loadedPlayerPosition;
                if (hasLoadedPlayerRotation)
                {
                    playerObject.transform.rotation = loadedPlayerRotation;
                }
                hasLoadedPlayerPosition = false;
                hasLoadedPlayerRotation = false;
            }
            else if (playerSpawnPoint != null)
            {
                playerObject.transform.position = playerSpawnPoint.position;
                playerObject.transform.rotation = playerSpawnPoint.rotation;
            }
        }
    }

    // ---------------- ACOSADO ----------------

    public void FoundVictim()
    {
        if (currentDayPhase != DayPhase.CasualTalk)
            return;

        currentDayPhase = DayPhase.Investigation;
        questionsUsed = 0;

        Debug.Log("Has encontrado al acosado. Investigacin desbloqueada.");
    }

    // ---------------- PREGUNTAS ----------------

    public bool CanAskQuestion()
    {
        return currentDayPhase == DayPhase.Investigation
            && questionsUsed < GetMaxQuestions();
    }

    public void UseQuestion()
    {
        questionsUsed++;
        Debug.Log($"Pregunta usada ({questionsUsed}/{GetMaxQuestions()})");

        if (questionsUsed >= GetMaxQuestions())
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

        hasAccusedThisDay = true; // <--- Marcamos que ya se hizo la acusacin
        Debug.Log("Decisin registrada. Ahora puedes irte.");
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
        isEndingPhase = true;
        if (goodDecisions >= 5)
        {
            isGoodEnding = true;
            Debug.Log("FINAL BUENO");
        }
        else
        {
            isGoodEnding = false;
            Debug.Log("FINAL MALO");
        }

        SaveSystem.SaveGame(BuildSaveData());

        if (isGoodEnding)
        {
            SceneManager.LoadScene("HouseSceneGood");
        }
        else
        {
            SceneManager.LoadScene("HouseSceneBad");
        }
    }


    // Funcin auxiliar para que los alumnos sepan qu da es
    public DayScenario GetCurrentDayScenario()
    {
        // Restamos 1 porque el array empieza en 0 pero tus das en 1
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
        int clampedRemaining = Mathf.Clamp(remaining, 0, GetMaxQuestions());
        questionsUsed = GetMaxQuestions() - clampedRemaining;
    }

    public SaveData BuildSaveData()
    {
        SaveData saveData = new SaveData
        {
            sceneName = SceneManager.GetActiveScene().name,
            currentDay = currentDay,
            currentDayPhase = (int)currentDayPhase,
            remainingQuestions = GetRemainingQuestions(),
            goodDecisions = goodDecisions,
            badDecisions = badDecisions,
            hasAccusedThisDay = hasAccusedThisDay,
            hasReadPrologueLetter = hasReadPrologueLetter,
            isEndingPhase = isEndingPhase,
            isGoodEnding = isGoodEnding
        };

        if (playerObject != null)
        {
            Vector3 playerPosition = playerObject.transform.position;
            saveData.hasPlayerPosition = true;
            saveData.playerPosX = playerPosition.x;
            saveData.playerPosY = playerPosition.y;
            saveData.playerPosZ = playerPosition.z;

            Quaternion playerRotation = playerObject.transform.rotation;
            saveData.hasPlayerRotation = true;
            saveData.playerRotX = playerRotation.x;
            saveData.playerRotY = playerRotation.y;
            saveData.playerRotZ = playerRotation.z;
            saveData.playerRotW = playerRotation.w;
        }

        StudentNPC[] allStudents = FindObjectsByType<StudentNPC>(FindObjectsInactive.Include, FindObjectsSortMode.None);
        foreach (StudentNPC student in allStudents)
        {
            saveData.studentProgress.Add(student.BuildProgressData());
        }

        TeacherNPC teacher = FindFirstObjectByType<TeacherNPC>(FindObjectsInactive.Include);
        if (teacher != null)
        {
            saveData.teacherProgress = teacher.BuildProgressData();
        }

        return saveData;
    }

    void ApplyDialogueProgress(SaveData data)
    {
        if (data == null || data.studentProgress == null)
        {
            return;
        }

        StudentNPC[] allStudents = FindObjectsByType<StudentNPC>(FindObjectsInactive.Include, FindObjectsSortMode.None);
        foreach (StudentNPC student in allStudents)
        {
            StudentDialogueProgressData savedStudent = data.studentProgress.Find(progress => progress.studentName == student.studentName);
            if (savedStudent != null)
            {
                student.ApplyProgressData(savedStudent);
            }
        }

        TeacherNPC teacher = FindFirstObjectByType<TeacherNPC>(FindObjectsInactive.Include);
        if (teacher != null && data.teacherProgress != null)
        {
            teacher.ApplyProgressData(data.teacherProgress);
        }
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
        hasReadPrologueLetter = data.hasReadPrologueLetter;
        isEndingPhase = data.isEndingPhase;
        isGoodEnding = data.isGoodEnding;
        SetRemainingQuestions(data.remainingQuestions);

        string activeSceneName = SceneManager.GetActiveScene().name;
        bool canRestorePlayerTransform =
            !string.IsNullOrWhiteSpace(data.sceneName) &&
            data.sceneName == activeSceneName;

        hasLoadedPlayerPosition = canRestorePlayerTransform && data.hasPlayerPosition;
        if (hasLoadedPlayerPosition)
        {
            loadedPlayerPosition = new Vector3(data.playerPosX, data.playerPosY, data.playerPosZ);
        }

        hasLoadedPlayerRotation = canRestorePlayerTransform && data.hasPlayerRotation;
        if (hasLoadedPlayerRotation)
        {
            loadedPlayerRotation = new Quaternion(data.playerRotX, data.playerRotY, data.playerRotZ, data.playerRotW);
        }

        if (!canRestorePlayerTransform && (data.hasPlayerPosition || data.hasPlayerRotation))
        {
            Debug.Log($"Posicion/rotacion guardada ignorada (save: '{data.sceneName}', actual: '{activeSceneName}'). Se usara playerSpawnPoint.");
        }

        if (System.Enum.IsDefined(typeof(DayPhase), data.currentDayPhase))
        {
            currentDayPhase = (DayPhase)data.currentDayPhase;
        }
        else
        {
            currentDayPhase = DayPhase.CasualTalk;
        }
    }

    public int GetPlayableDayCount()
    {
        if (allDays == null || allDays.Length == 0)
        {
            return 1;
        }

        return Mathf.Min(maxDays, allDays.Length);
    }


}
