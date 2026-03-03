using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;

public class QuestionUI : MonoBehaviour
{
    [Header("Configuración")]
    public GameObject iconPrefab;

    private List<GameObject> activeIcons = new List<GameObject>();

    void Start()
    {
        UpdateUI();
    }

   
    void Update()
    {
        // Para que sea automático, comparamos el conteo
        int remaining = GameManager.Instance.GetRemainingQuestions();
        if (remaining != activeIcons.Count)
        {
            UpdateUI();
        }
    }

    public void UpdateUI()
    {
        // Limpiar iconos viejos
        foreach (GameObject icon in activeIcons)
        {
            Destroy(icon);
        }
        activeIcons.Clear();

        // Crear iconos nuevos según las preguntas restantes
        int remaining = GameManager.Instance.GetRemainingQuestions();

        // Solo mostramos iconos si estamos en fase de investigación o decisión
        if (GameManager.Instance.currentDayPhase != DayPhase.CasualTalk)
        {
            for (int i = 0; i < remaining; i++)
            {
                GameObject newIcon = Instantiate(iconPrefab, transform);
                activeIcons.Add(newIcon);
            }
        }
    }
}