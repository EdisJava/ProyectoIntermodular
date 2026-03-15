using System;

[Serializable]
public class SaveData
{
    public int currentDay;
    public int currentDayPhase;
    public int remainingQuestions;
    public int goodDecisions;
    public int badDecisions;
    public bool hasAccusedThisDay;
}
