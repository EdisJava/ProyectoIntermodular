using UnityEngine;
using TMPro;

public class DayDisplayUI : MonoBehaviour
{
    [Header("Configuracion")]
    [SerializeField] private TextMeshProUGUI dayText;

    private int lastDay = -1;

    private void Awake()
    {
        if (dayText == null)
        {
            dayText = GetComponent<TextMeshProUGUI>();
        }
    }

    private void Start()
    {
        UpdateDayText();
    }

    private void Update()
    {
        if (GameManager.Instance == null || dayText == null)
        {
            return;
        }

        if (GameManager.Instance.currentDay != lastDay)
        {
            UpdateDayText();
        }
    }

    private void UpdateDayText()
    {
        if (GameManager.Instance == null || dayText == null)
        {
            return;
        }

        lastDay = GameManager.Instance.currentDay;
        dayText.text = $"DÍA {lastDay}";
    }
}