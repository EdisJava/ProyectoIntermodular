using UnityEngine;
using TMPro;

public class GoalUI : MonoBehaviour
{
    private TextMeshProUGUI goalText;
    private DayPhase lastPhase;
    private int lastDay;

    void Awake()
    {
        goalText = GetComponent<TextMeshProUGUI>();
    }

    private bool lastAccusedState; // Para detectar el cambio

    void Update()
    {
        // Añadimos la comprobación de hasAccusedThisDay al IF
        if (GameManager.Instance.currentDayPhase != lastPhase ||
            GameManager.Instance.currentDay != lastDay ||
            GameManager.Instance.hasAccusedThisDay != lastAccusedState)
        {
            UpdateGoalDisplay();
        }
    }

    void UpdateGoalDisplay()
    {
        lastPhase = GameManager.Instance.currentDayPhase;
        lastDay = GameManager.Instance.currentDay;
        lastAccusedState = GameManager.Instance.hasAccusedThisDay; // Guardamos el estado

        DayScenario currentScenario = GameManager.Instance.GetCurrentDayScenario();
        if (currentScenario == null) return;

        // Prioridad: Si ya acusó, mostrar el texto de salida
        if (GameManager.Instance.hasAccusedThisDay)
        {
            // Si no tienes 'goalExit' en tu scriptable object, puedes poner "Sal por la puerta"
            goalText.text = "Ya has terminado aquí. Sal por la puerta.";
            return;
        }

        // Si no ha acusado, seguimos con el switch normal
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
    }
}