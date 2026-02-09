using System;
using System.Collections.Generic;
using UnityEngine;

// Orquesta diálogos, decisiones y áreas disponibles por día.
// Integrado con el GameManager existente (cambia GameState cuando se entra/sale de área).
public class DayManager : MonoBehaviour
{
    public DayData[] days;
    public bool autoStart = true;

    public event Action<DayData.DialogueLine> OnShowDialogue;
    public event Action<DayData.DecisionPoint> OnShowDecision;
    public event Action<List<string>> OnAvailableAreasChanged; // lista de area names
    public event Action<string> OnEnterArea;
    public event Action<string> OnExitArea;
    public event Action<int, bool> OnDayFinished; // index, success

    int currentDayIndex = -1;
    DayData currentDay;
    int currentDialogueIndex;
    HashSet<int> decisionsMadeIndices = new HashSet<int>();
    List<int> badDecisionsChosen = new List<int>();
    string currentAreaName;

    void Start()
    {
        if (autoStart && days != null && days.Length > 0)
            StartDay(0);
    }

    public void StartDay(int index)
    {
        if (index < 0 || index >= (days?.Length ?? 0)) { Debug.LogWarning("StartDay: índice fuera de rango."); return; }
        currentDayIndex = index;
        currentDay = days[index];
        currentDialogueIndex = 0;
        decisionsMadeIndices.Clear();
        badDecisionsChosen.Clear();
        currentAreaName = null;

        Debug.Log($"DayManager: iniciando día {currentDay.dayNumber} - {currentDay.displayName}");
        var areas = new List<string>(currentDay.availableAreaNames ?? new string[0]);
        OnAvailableAreasChanged?.Invoke(areas);
        PlayNextLine();
    }

    void PlayNextLine()
    {
        if (currentDay == null) return;
        if (currentDialogueIndex >= currentDay.dialogues.Length) { EndDay(); return; }
        var line = currentDay.dialogues[currentDialogueIndex];
        OnShowDialogue?.Invoke(line);
        var dp = FindDecisionAfterDialogue(currentDialogueIndex);
        if (dp != null) OnShowDecision?.Invoke(dp);
        else { currentDialogueIndex++; PlayNextLine(); }
    }

    public void MakeDecision(DayData.DecisionPoint decisionPoint, int selectedOptionIndex)
    {
        if (currentDay == null) return;
        int decisionIndex = Array.IndexOf(currentDay.decisions, decisionPoint);
        if (decisionIndex < 0 || decisionsMadeIndices.Contains(decisionIndex)) { Debug.LogWarning("Decision inválida o ya resuelta."); return; }
        decisionsMadeIndices.Add(decisionIndex);
        bool choseBad = selectedOptionIndex == decisionPoint.badOptionIndex;
        if (choseBad) badDecisionsChosen.Add(decisionIndex);
        currentDialogueIndex++;
        PlayNextLine();
    }

    DayData.DecisionPoint FindDecisionAfterDialogue(int idx)
    {
        if (currentDay == null || currentDay.decisions == null) return null;
        foreach (var d in currentDay.decisions) if (d.afterDialogueIndex == idx) return d;
        return null;
    }

    void EndDay()
    {
        bool success = badDecisionsChosen.Count == 0;
        Debug.Log($"DayManager: Día {currentDay?.dayNumber} terminado. Éxito: {success}");
        OnDayFinished?.Invoke(currentDayIndex, success);
    }

    // Áreas
    public bool IsAreaAvailable(string areaName)
    {
        if (currentDay == null || currentDay.availableAreaNames == null) return false;
        foreach (var a in currentDay.availableAreaNames) if (a == areaName) return true;
        return false;
    }

    public void RequestEnterArea(string areaName)
    {
        if (!IsAreaAvailable(areaName)) { Debug.LogWarning($"Área '{areaName}' no disponible hoy."); return; }
        if (currentAreaName != null) { OnExitArea?.Invoke(currentAreaName); }
        currentAreaName = areaName;
        // Cambiamos el estado global para usar el sistema ya implementado:
        GameManager.Instance.currentState = GameState.Interaction2D;
        OnEnterArea?.Invoke(areaName);
    }

    public void ExitCurrentArea()
    {
        if (currentAreaName == null) return;
        OnExitArea?.Invoke(currentAreaName);
        currentAreaName = null;
        GameManager.Instance.currentState = GameState.Exploration3D;
    }

    // Helpers
    public List<string> GetAvailableAreasForCurrentDay() => new List<string>(currentDay?.availableAreaNames ?? new string[0]);
    public int GetCurrentDayIndex() => currentDayIndex;
    public void ContinueAfterDialogue() { currentDialogueIndex++; PlayNextLine(); }
}