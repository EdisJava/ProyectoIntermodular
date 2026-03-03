using UnityEngine;

public class StudentData : MonoBehaviour
{
    public string studentName;

    [TextArea]
    public string casualDialogue;

    [TextArea]
    public string investigationDialogue;

    public bool isVictim;
    public bool isBully;
    public bool lies;

    public int appearsOnDay;
}