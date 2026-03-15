using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class MainMenu : MonoBehaviour
{
  
    public GameObject mainMenu;
    public Button playButton;
    public Button newGameButton;
    public Button exitButton;
    public GameObject newGameConfirmationText;
    public Button confirmYesButton;
    public Button confirmNoButton;

    void Awake()
    {
        if (playButton == null)
        {
            playButton = FindButtonByName("JugarButton");
        }

        if (newGameButton == null)
        {
            newGameButton = FindButtonByName("NuevaPartidaButton");
        }
        
        if (exitButton == null)
        {
            exitButton = FindButtonByName("SalirButton");
        }

        if (newGameConfirmationText == null)
        {
            TMPro.TextMeshProUGUI[] texts = FindObjectsByType<TMPro.TextMeshProUGUI>(FindObjectsInactive.Include, FindObjectsSortMode.None);
            foreach (TMPro.TextMeshProUGUI text in texts)
            {
                if (text.text.Contains("SEGURO"))
                {
                    newGameConfirmationText = text.gameObject;
                    break;
                }
            }
        }

        if (confirmYesButton == null)
        {
            confirmYesButton = FindButtonByName("SiButton");
        }

        if (confirmNoButton == null)
        {
            confirmNoButton = FindButtonByName("noButton");
        }
    }

    void Start()
    {
        ShowMainOptions();
    }

    public void OpenMainMenuPanel()
    {
        mainMenu.SetActive(true);
        ShowMainOptions();
    }

    public void QuitGame()
    {
        Application.Quit();
    }

    public void PlayGame()
    {
        if (!SaveSystem.HasSave())
        {
            Debug.Log("No hay partida guardada para continuar.");
            RefreshButtons();
            return;
        }

        SceneManager.LoadScene("SampleScene");
    }

    public void NewGame()
    {
        ShowNewGameConfirmation();
    }

    public void ConfirmNewGame()
    {
        SaveSystem.DeleteSave();
        ShowMainOptions();
        SceneManager.LoadScene("SampleScene");
    }

    public void CancelNewGame()
    {
        ShowMainOptions();
    }

    void RefreshButtons()
    {
        if (playButton != null)
        {
            playButton.interactable = SaveSystem.HasSave();
        }
    }

    void ShowNewGameConfirmation()
    {
        SetActiveIfAssigned(playButton, false);
        SetActiveIfAssigned(newGameButton, false);
        SetActiveIfAssigned(exitButton, false);
        SetActiveIfAssigned(newGameConfirmationText, true);
        SetActiveIfAssigned(confirmYesButton, true);
        SetActiveIfAssigned(confirmNoButton, true);
        RefreshButtons();
    }

    void ShowMainOptions()
    {
        SetActiveIfAssigned(playButton, true);
        SetActiveIfAssigned(newGameButton, true);
        SetActiveIfAssigned(exitButton, true);
        SetActiveIfAssigned(newGameConfirmationText, false);
        SetActiveIfAssigned(confirmYesButton, false);
        SetActiveIfAssigned(confirmNoButton, false);
        RefreshButtons();
    }

    void SetActiveIfAssigned(Component component, bool active)
    {
        if (component != null)
        {
            component.gameObject.SetActive(active);
        }
    }

    void SetActiveIfAssigned(GameObject target, bool active)
    {
        if (target != null)
        {
            target.SetActive(active);
        }
    }

    Button FindButtonByName(string buttonName)
    {
        Button[] buttons = FindObjectsByType<Button>(FindObjectsInactive.Include, FindObjectsSortMode.None);
        foreach (Button button in buttons)
        {
            if (button.gameObject.name.Trim() == buttonName)
            {
                return button;
            }
        }

        return null;
    }
}
