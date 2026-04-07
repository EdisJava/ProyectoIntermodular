using UnityEngine;
using TMPro;
using UnityEngine.SceneManagement;

public class GoalUI : MonoBehaviour
{
    private const string PrologueSceneName = "HouseScene";

    private TextMeshProUGUI goalText;
    private TMP_FontAsset fixedFontAsset;
    private DayPhase lastPhase;
    private int lastDay;
    private bool lastAccusedState;
    private bool lastPrologueReadState;
    private string lastSceneName = string.Empty;

    void Awake()
    {
        goalText = GetComponent<TextMeshProUGUI>();
        if (goalText != null)
        {
            fixedFontAsset = goalText.font;
        }
    }

    void Update()
    {
        if (goalText == null || GameManager.Instance == null)
        {
            return;
        }

        string sceneName = SceneManager.GetActiveScene().name;
        bool isPrologue = sceneName == PrologueSceneName;

        if (isPrologue)
        {
            bool readLetter = GameManager.Instance.hasReadPrologueLetter;
            if (sceneName != lastSceneName || readLetter != lastPrologueReadState)
            {
                goalText.text = readLetter ? "Ve a clases" : "Lee la carta";
                lastSceneName = sceneName;
                lastPrologueReadState = readLetter;
            }
            return;
        }

        if (sceneName != lastSceneName ||
            GameManager.Instance.currentDayPhase != lastPhase ||
            GameManager.Instance.currentDay != lastDay ||
            GameManager.Instance.hasAccusedThisDay != lastAccusedState)
        {
            UpdateGoalDisplay(sceneName);
        }
    }

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

        if (GameManager.Instance.hasAccusedThisDay)
        {
            goalText.text = "Ya has terminado aqui. Sal por la puerta.";
            return;
        }

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

        ApplyUppercase();
    }

    private void LateUpdate()
    {
        ApplyUppercase();
    }

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
