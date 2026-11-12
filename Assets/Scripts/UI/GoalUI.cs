using UnityEngine;
using TMPro;
using UnityEngine.SceneManagement;

/*
* Script para manejar la interfaz de usuario de objetivos.
* 
* Metodos:
*   - Update(): Metodo que se llama cada frame.
*   - UpdateGoalDisplay(): Metodo que actualiza la visualizacion de objetivos.
*   - ApplyUppercase(): Metodo que aplica mayusculas al texto.
*
*   Variables:
*   - goalText: Texto que muestra los objetivos.
*   - fixedFontAsset: Fuente fija para el texto.
*   - lastPhase: Fase anterior.
*   - lastDay: Dia anterior.
*   - lastAccusedState: Estado de acusacion anterior.
*   - lastPrologueReadState: Estado de lectura del prologo anterior.
*   - lastSceneName: Nombre de la escena anterior.
*
*   Funcionamiento:
*   - Al actualizar, verifica si la fase, el dia, el estado de acusacion o el nombre de la escena han cambiado.
*   - Si han cambiado, actualiza la visualizacion de objetivos.
*   - Aplica mayusculas al texto.
*
*   Flujo:
*   1. El jugador interactua con el alumno.
*   2. Se llama al metodo Interact().
*   3. Se determina la fase actual del dia.
*   4. Se llama al metodo correspondiente segun la fase.
*   5. Se muestra el dialogo del alumno.
*   6. El jugador puede interactuar con otro alumno.
*/

public class GoalUI : MonoBehaviour
{
    private const string PrologueSceneName = "HouseScenePrologue";
    private const string GoodEndingSceneName = "HouseSceneGood";
    private const string BadEndingSceneName = "HouseSceneBad";

    private TextMeshProUGUI goalText;
    private TMP_FontAsset fixedFontAsset;
    private DayPhase lastPhase;
    private int lastDay;
    private bool lastAccusedState;
    private bool lastPrologueReadState;
    private string lastSceneName = string.Empty;

    /*
    * Metodo que se llama al inicio.
    */
    void Awake()
    {
        goalText = GetComponent<TextMeshProUGUI>();
        if (goalText != null)
        {
            fixedFontAsset = goalText.font;
        }
    }

    /*
    * Metodo que se llama cada frame.
    */
    void Update()
    {
        if (goalText == null || GameManager.Instance == null)
        {
            return;
        }

        string sceneName = SceneManager.GetActiveScene().name;
        bool isPrologue = sceneName == PrologueSceneName;
        bool isEndingScene = sceneName == GoodEndingSceneName || sceneName == BadEndingSceneName;

        if (isEndingScene)
        {
            if (sceneName != lastSceneName)
            {
                goalText.text = "Vuelve a casa";
                lastSceneName = sceneName;
            }

            ApplyUppercase();
            return;
        }

        if (isPrologue)
        {
            bool readLetter = GameManager.Instance.hasReadPrologueLetter;
            //Actualiza la visualizacion de objetivos si la fase, el dia, el estado de acusacion o el nombre de la escena han cambiado.
            if (sceneName != lastSceneName || readLetter != lastPrologueReadState)
            {
                goalText.text = readLetter ? "Ve a clases" : "Lee la carta";
                lastSceneName = sceneName;
                lastPrologueReadState = readLetter;
            }
            return;
        }
        //Actualiza la visualizacion de objetivos si la fase, el dia, el estado de acusacion o el nombre de la escena han cambiado.
        if (sceneName != lastSceneName ||
            GameManager.Instance.currentDayPhase != lastPhase ||
            GameManager.Instance.currentDay != lastDay ||
            GameManager.Instance.hasAccusedThisDay != lastAccusedState)
        {
            UpdateGoalDisplay(sceneName);
        }
    }

    /*
    * Metodo que actualiza la visualizacion de objetivos.
    */
    void UpdateGoalDisplay(string sceneName)
    {
        lastSceneName = sceneName;
        lastPhase = GameManager.Instance.currentDayPhase;
        lastDay = GameManager.Instance.currentDay;
        lastAccusedState = GameManager.Instance.hasAccusedThisDay;

        DayScenario currentScenario = GameManager.Instance.GetCurrentDayScenario();
        if (currentScenario == null)
        {
            return;
        }
        //Si el jugador ha acusado a alguien, muestra el mensaje de "Ya has terminado aqui".
        if (GameManager.Instance.hasAccusedThisDay)
        {
            goalText.text = "Ya has terminado aqui. Sal por la puerta.";
            return;
        }
        //Actualiza la visualizacion de objetivos segun la fase actual del dia.
        switch (lastPhase)
        {
            case DayPhase.CasualTalk:
                goalText.text = currentScenario.goalFindVictim;
                break;
            case DayPhase.Investigation:
                goalText.text = currentScenario.goalInvestigate;
                break;
            case DayPhase.Decision:
                goalText.text = currentScenario.goalReport;
                break;
        }
        //Aplica mayusculas al texto.
        ApplyUppercase();
    }

    /*
    * Metodo que se llama cada frame.
    */
    private void LateUpdate()
    {
        ApplyUppercase();
    }

    /*
    * Metodo que aplica mayusculas al texto.
    */
    private void ApplyUppercase()
    {
        if (goalText == null || string.IsNullOrEmpty(goalText.text))
        {
            return;
        }

        if (fixedFontAsset != null && goalText.font != fixedFontAsset)
        {
            goalText.font = fixedFontAsset;
        }

        goalText.text = goalText.text.ToUpperInvariant();
    }
}
