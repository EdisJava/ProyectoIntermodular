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

    void Update()
    {
        // actualizar no cada frame, sino cuando cambie la fase o el dia
        if (GameManager.Instance.currentDayPhase != lastPhase || GameManager.Instance.currentDay != lastDay)
        {
            UpdateGoalDisplay();
        }
    }

    void UpdateGoalDisplay()
    {
        lastPhase = GameManager.Instance.currentDayPhase;
        lastDay = GameManager.Instance.currentDay;

        DayScenario currentScenario = GameManager.Instance.GetCurrentDayScenario();

        if (currentScenario == null) return;

        // cambiar el texto segun la fase actual
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