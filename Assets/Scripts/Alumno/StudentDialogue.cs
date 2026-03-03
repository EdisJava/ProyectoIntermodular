using UnityEngine;

public enum StudentRole
{
    Normal,
    Victim,
    Bully
}
public class StudentDialogue : MonoBehaviour
{
    [Header("Identidad")]
    public string studentName;
    public StudentRole role;

    [Header("Comportamiento")]
    public bool liesAboutBully; // si miente al señalar

    [Header("Diálogos")]
    [TextArea] public string casualDialogue;
    [TextArea] public string victimDialogue;
    [TextArea] public string truthDialogue;
    [TextArea] public string lieDialogue;

    private bool alreadyAsked = false;

    // ESTE MÉTODO LO LLAMAS AL INTERACTUAR
    public void Talk()
    {
        var gm = GameManager.Instance;

        switch (gm.currentDayPhase)
        {
            case DayPhase.CasualTalk:
                CasualTalk();
                break;

            case DayPhase.Investigation:
                InvestigationTalk();
                break;

            case DayPhase.Decision:
                DecisionTalk();
                break;
        }
    }

    // ---------------- FASE 1 ----------------

    void CasualTalk()
    {
        if (role == StudentRole.Victim)
        {
            Debug.Log($"{studentName}: {victimDialogue}");
            GameManager.Instance.FoundVictim();
        }
        else
        {
            Debug.Log($"{studentName}: {casualDialogue}");
        }
    }

    // ---------------- FASE 2 ----------------

    void InvestigationTalk()
    {
        if (alreadyAsked)
        {
            Debug.Log($"{studentName}: Ya te dije todo lo que sabía.");
            return;
        }

        if (!GameManager.Instance.CanAskQuestion())
        {
            Debug.Log("No puedes hacer más preguntas hoy.");
            return;
        }

        alreadyAsked = true;
        GameManager.Instance.UseQuestion();

        if (liesAboutBully)
            Debug.Log($"{studentName}: {lieDialogue}");
        else
            Debug.Log($"{studentName}: {truthDialogue}");
    }

    // ---------------- FASE 3 ----------------

    void DecisionTalk()
    {
        Debug.Log($"{studentName}: Deberías hablar con el profesor.");
    }

    // ---------------- RESET POR DÍA ----------------

    public void ResetForNewDay()
    {
        alreadyAsked = false;
    }
}
