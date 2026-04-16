using UnityEngine;
using UnityEngine.SceneManagement;

/*
* Script para manejar el estado del juego.
* 
* Metodos:
*   - Awake(): Metodo que se llama al crear el objeto.
*   - StartNewGameState(): Metodo que inicia un nuevo estado del juego.
*   - RefreshCurrentDay(): Metodo que actualiza el dia actual.
*   - SetCurrentDayPhase(): Metodo que establece la fase actual del dia.
*   - GetCurrentDayScenario(): Metodo que obtiene el escenario actual del dia.
*   - ResetAllNPCs(): Metodo que reinicia todos los NPCs.
*   - SetRemainingQuestions(): Metodo que establece las preguntas restantes.
*   - BuildSaveData(): Metodo que construye los datos de guardado.
*   - ApplyDialogueProgress(): Metodo que aplica el progreso del dialogo.
*   - ApplySaveData(): Metodo que aplica los datos de guardado.
*   - GetPlayableDayCount(): Metodo que obtiene el numero de dias jugables.
*   - RegisterDecision(): Metodo que registra una decision.
*   - NextDay(): Metodo que pasa al siguiente dia.
*   - EndGame(): Metodo que termina el juego.
*   - IsIn2D(): Metodo que verifica si el juego esta en 2D.
*   - HasAccusedThisDay(): Metodo que verifica si se ha acusado hoy.
*   - HasReadPrologueLetter(): Metodo que verifica si se ha leido el prologo.
*   - HasLoadedPlayerPosition(): Metodo que verifica si se ha cargado la posicion del jugador.
*   - HasLoadedPlayerRotation(): Metodo que verifica si se ha cargado la rotacion del jugador.
*   - LoadedPlayerPosition(): Metodo que obtiene la posicion del jugador.
*   - LoadedPlayerRotation(): Metodo que obtiene la rotacion del jugador.
*
*   Variables:
*   - currentState: Estado actual del juego.
*   - currentDayPhase: Fase actual del dia.
*   - currentDay: Dia actual.
*   - maxDays: Numero maximo de dias.
*   - maxQuestionsPerDay: Numero maximo de preguntas por dia.
*   - questionsUsed: Numero de preguntas usadas.
*   - goodDecisions: Numero de decisiones correctas.
*   - badDecisions: Numero de decisiones incorrectas.
*   - isEndingPhase: Si el juego esta en fase de final.
*   - isGoodEnding: Si el juego esta en fase de final bueno.
*   - allDays: Array de escenarios del dia.
*   - hasAccusedThisDay: Si se ha acusado hoy.
*   - hasReadPrologueLetter: Si se ha leido el prologo.
*   - playerSpawnPoint: Punto de aparicion del jugador.
*   - playerObject: Objeto del jugador.
*   - hasLoadedPlayerPosition: Si se ha cargado la posicion del jugador.
*   - loadedPlayerPosition: Posicion del jugador.
*   - hasLoadedPlayerRotation: Si se ha cargado la rotacion del jugador.
*   - loadedPlayerRotation: Rotacion del jugador.
*
*   Funcionamiento:
*   - Al crear el objeto, verifica si existe una partida guardada.
*   - Si existe, aplica los datos de guardado.
*   - Si no existe, inicia un nuevo estado del juego.
*   - Al presionar el boton de siguiente dia, pasa al siguiente dia.
*   - Al presionar el boton de acusar, registra la decision.
*   - Al presionar el boton de leer prologo, registra la lectura del prologo.
*
*   Flujo:
*   1. El jugador presiona el boton de siguiente dia.
*   2. Se llama al metodo NextDay().
*   3. Se incrementa el dia actual.
*   4. Se reinicia el estado diario.
*   5. Se actualiza la visualizacion de objetivos.
*   6. El jugador puede interactuar con otro alumno.
*/

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

    [Header("Dias")]
    public int currentDay = 1;
    public int maxDays = 5;

    [Header("Investigacion")]
    public int maxQuestionsPerDay = 2;
    private int questionsUsed;

    /*
    * Metodo que obtiene el numero maximo de preguntas.
    */
    public int GetMaxQuestions()
    {
        return currentDay == 3 ? 5 : maxQuestionsPerDay;
    }

    /*
    * Metodo que obtiene el numero de preguntas restantes.
    */
    public int GetRemainingQuestions()
    {
        return GetMaxQuestions() - questionsUsed;
    }

    [Header("Resultados")]
    public int goodDecisions = 0;
    public int badDecisions = 0;
    public bool isEndingPhase = false;
    public bool isGoodEnding = false;

    [Header("Configuracion de la Historia")]
    public DayScenario[] allDays;

    /*
    * Metodo que se llama al crear el objeto.
    */
    private void Awake()
    {
        //Si no existe una instancia, crea una.
        if (Instance == null)
            Instance = this;
        else
        {
            //Si existe una instancia, destruye el objeto.
            Destroy(gameObject);
            return;
        }

        //Si existe una partida guardada, aplica los datos de guardado.
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

    /*
    * Metodo que verifica si el juego esta en 2D.
    */
    public bool IsIn2D()
    {
        return currentState == GameState.Interaction2D;
    }

    // ---------------- DIAS ----------------

    public bool hasAccusedThisDay = false; // Nueva variable
    public bool hasReadPrologueLetter = false;

    [Header("Posicion Inicial")]
    public Transform playerSpawnPoint;
    public GameObject playerObject;
    private bool hasLoadedPlayerPosition = false;
    private Vector3 loadedPlayerPosition;
    private bool hasLoadedPlayerRotation = false;
    private Quaternion loadedPlayerRotation;

    /*
    * Metodo que inicia un nuevo estado del juego.
    */
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

    /*
    * Metodo que aplica los datos de guardado.
    */
    void ApplyLoadedGameState(SaveData data)
    {
        ApplySaveData(data);
        RefreshCurrentDay(false);
        ApplyDialogueProgress(data);
    }

    /*
    * Metodo que reinicia el estado diario.
    */
    void ResetDailyState()
    {
        currentDayPhase = DayPhase.CasualTalk;
        questionsUsed = 0;
        hasAccusedThisDay = false;
    }

    /*
    * Metodo que refresca el dia actual.
    */
    void RefreshCurrentDay(bool resetDailyMemory)
    {
        DayScenario scenario = GetCurrentDayScenario();
        // si no hay escenario, muestra un error.
        if (scenario != null)
        {
            Debug.Log($"Configurando Dia {currentDay}: {scenario.name}");
            // busca todos los alumnos.
            StudentNPC[] allNPCs = FindObjectsOfType<StudentNPC>();
            foreach (StudentNPC npc in allNPCs)
            {
                if (resetDailyMemory)
                {
                    npc.ResetMemory();
                }

                npc.SetupCharacterForToday();
            }
            // si no hay profesor, muestra un error.
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
            Debug.LogError("No hay un DayScenario configurado para el dia " + currentDay);
        }

        Debug.Log($"Dia {currentDay} comienza oficialmente");
        // si el jugador existe, carga su posicion.
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
    /*
    * Metodo que se llama cuando se encuentra al acosado.
    */
    public void FoundVictim()
    {
        if (currentDayPhase != DayPhase.CasualTalk)
            return;

        currentDayPhase = DayPhase.Investigation;
        questionsUsed = 0;

        Debug.Log("Has encontrado al acosado. Investigacin desbloqueada.");
    }

    // ---------------- PREGUNTAS ----------------
    /*
    * Metodo que verifica si se puede hacer una pregunta.
    */
    public bool CanAskQuestion()
    {
        return currentDayPhase == DayPhase.Investigation
            && questionsUsed < GetMaxQuestions();
    }

    /*
    * Metodo que registra el uso de una pregunta.
    */
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
    /*
    * Metodo que registra la decision.
    */
    public void RegisterDecision(bool correct)
    {
        if (correct) goodDecisions++;
        else badDecisions++;

        hasAccusedThisDay = true; // <--- Marca que ya se hizo la acusacion
        Debug.Log("Decisin registrada. Ahora puedes irte.");
    }

    /*
    * Metodo que pasa al siguiente dia.
    */
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

    /*
    * Metodo que termina el juego.
    */
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
            LoadEndingScene("HouseSceneGood", "Assets/HouseSceneGood.unity");
        }
        else
        {
            LoadEndingScene("HouseSceneBad", "Assets/Scenes/HouseSceneBad.unity");
        }
    }

    void LoadEndingScene(string sceneName, string scenePath)
    {
        if (Application.CanStreamedLevelBeLoaded(sceneName))
        {
            SceneManager.LoadScene(sceneName);
            return;
        }

        if (Application.CanStreamedLevelBeLoaded(scenePath))
        {
            SceneManager.LoadScene(scenePath);
            return;
        }

        Debug.LogError($"No se pudo cargar la escena de final '{sceneName}'.");
    }


    /*
    * Metodo auxiliar para que los alumnos sepan que dia es.
    */
    public DayScenario GetCurrentDayScenario()
    {
        // Restamos 1 porque el array empieza en 0 pero tus das en 1
        int index = currentDay - 1;

        if (index < allDays.Length)
            return allDays[index];

        return null;
    }

    /*
    * Metodo que reinicia todos los NPCs.
    */
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

    /*
    * Metodo que establece el numero de preguntas restantes.
    */
    public void SetRemainingQuestions(int remaining)
    {
        int clampedRemaining = Mathf.Clamp(remaining, 0, GetMaxQuestions());
        questionsUsed = GetMaxQuestions() - clampedRemaining;
    }

    /*
    * Metodo que construye los datos de guardado.
    */
    public SaveData BuildSaveData()
    {
        // Crea una nueva instancia de SaveData.
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
    /*
    * Metodo que aplica los datos de guardado.
    */
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
    /*
    * Metodo que aplica los datos de guardado.
    */
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
    /*
    * Metodo que obtiene el numero de dias jugables.
    */
    public int GetPlayableDayCount()
    {
        if (allDays == null || allDays.Length == 0)
        {
            return 1;
        }

        return Mathf.Min(maxDays, allDays.Length);
    }


}
