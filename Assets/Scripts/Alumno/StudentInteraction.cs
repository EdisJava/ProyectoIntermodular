using UnityEngine;

public class StudentInteraction : MonoBehaviour
{
    public StudentData data;

    private bool alreadyAsked = false;

    public void Interact()
    {
        if (GameManager.Instance.currentDay != data.appearsOnDay)
            return;

        switch (GameManager.Instance.currentDayPhase)
        {
            case DayPhase.CasualTalk:
                CasualTalk();
                break;

            case DayPhase.Investigation:
                InvestigationTalk();
                break;
        }
    }

    void CasualTalk()
    {
        Debug.Log(data.studentName + ": " + data.casualDialogue);

        if (data.isVictim)
        {
            GameManager.Instance.FoundVictim();
        }
    }

    void InvestigationTalk()
    {
        if (alreadyAsked)
        {
            Debug.Log("Ya hablaste con esta persona.");
            return;
        }

        if (!GameManager.Instance.CanAskQuestion())
        {
            Debug.Log("No te quedan preguntas hoy.");
            return;
        }

        GameManager.Instance.UseQuestion();
        alreadyAsked = true;

        if (data.lies)
            Debug.Log(data.studentName + ": Creo que fue X (mentira)");
        else
            Debug.Log(data.studentName + ": Creo que fue el acosador real");
    }

    public void Accuse()
    {
        GameManager.Instance.RegisterDecision(data.isBully);
    }
}
