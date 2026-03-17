using System;
using System.Collections.Generic;

[Serializable]
public class SaveData
{
    public int currentDay;
    public int currentDayPhase;
    public int remainingQuestions;
    public int goodDecisions;
    public int badDecisions;
    public bool hasAccusedThisDay;
    public List<StudentDialogueProgressData> studentProgress = new List<StudentDialogueProgressData>();
    public TeacherDialogueProgressData teacherProgress;
    public bool hasPlayerPosition;
    public float playerPosX;
    public float playerPosY;
    public float playerPosZ;
    public bool hasPlayerRotation;
    public float playerRotX;
    public float playerRotY;
    public float playerRotZ;
    public float playerRotW;
}

[Serializable]
public class StudentDialogueProgressData
{
    public string studentName;
    public bool casualRead;
    public bool alreadyInterrogated;
    public bool victimFound;
}

[Serializable]
public class TeacherDialogueProgressData
{
    public string teacherName;
    public bool casualRead;
    public bool hasAccusedToday;
}
