using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class PauseMenu : MonoBehaviour
{
    public GameObject pauseMenuUI;
    public FirstPlayerController movementScript;

    public Button resumeButton;
    public Button saveAndExitButton;
    public Button exitWithoutSaveButton;
    public GameObject confirmationTextObject;
    public TextMeshProUGUI confirmationTextLabel;
    public Button confirmYesButton;
    public Button confirmNoButton;

    private bool isPaused = false;

    private enum PendingExitAction
    {
        None,
        SaveAndExit,
        ExitWithoutSaving
    }

    private PendingExitAction pendingExitAction = PendingExitAction.None;
    private bool isExitingToMenu = false;

    void Awake()
    {
        if (resumeButton == null)
        {
            resumeButton = FindButtonByName("ReanudarButton");
        }

        if (saveAndExitButton == null)
        {
            saveAndExitButton = FindButtonByName("SalirSaveButton");
        }

        if (exitWithoutSaveButton == null)
        {
            exitWithoutSaveButton = FindButtonByName("SalirNoSaveButton (1)");
        }

        if (confirmYesButton == null)
        {
            confirmYesButton = FindButtonByName("Si");
        }

        if (confirmNoButton == null)
        {
            confirmNoButton = FindButtonByName("No");
        }

        if (confirmationTextObject == null)
        {
            confirmationTextObject = FindObjectByName("ConfirmText");
        }

        if (confirmationTextLabel == null && confirmationTextObject != null)
        {
            confirmationTextLabel = confirmationTextObject.GetComponentInChildren<TextMeshProUGUI>(true);
        }

        if (confirmYesButton != null)
        {
            confirmYesButton.onClick.RemoveListener(ConfirmExitYes);
            confirmYesButton.onClick.AddListener(ConfirmExitYes);
        }

        if (confirmNoButton != null)
        {
            confirmNoButton.onClick.RemoveListener(ConfirmExitNo);
            confirmNoButton.onClick.AddListener(ConfirmExitNo);
        }
    }

    void Update()
    {
        if (Keyboard.current != null && Keyboard.current.escapeKey.wasPressedThisFrame)
        {
            if (isPaused)
            {
                if (pendingExitAction != PendingExitAction.None)
                {
                    ConfirmExitNo();
                }
                else
                {
                    Resume();
                }
            }
            else
            {
                Pause();
            }
        }
    }

    public void Resume()
    {
        pendingExitAction = PendingExitAction.None;
        ShowPauseButtons();

        pauseMenuUI.SetActive(false);
        Time.timeScale = 1f;
        isPaused = false;

        if (GameManager.Instance.currentState == GameState.Exploration3D)
        {
            Cursor.visible = false;
            Cursor.lockState = CursorLockMode.Locked;
        }

        movementScript.enabled = true;
        movementScript.canLook = true;
    }

    void Pause()
    {
        pauseMenuUI.SetActive(true);
        Time.timeScale = 0f;
        isPaused = true;
        pendingExitAction = PendingExitAction.None;
        ShowPauseButtons();

        Cursor.visible = true;
        Cursor.lockState = CursorLockMode.None;
        movementScript.enabled = false;
        movementScript.canLook = false;
    }

    public void SaveAndExit()
    {
        pendingExitAction = PendingExitAction.SaveAndExit;
        ShowConfirmation("¿QUIERES GUARDAR Y SALIR?");
    }

    public void ExitWithoutSaving()
    {
        GameObject selected = EventSystem.current != null ? EventSystem.current.currentSelectedGameObject : null;
        if (selected != null)
        {
            if (confirmNoButton != null && selected == confirmNoButton.gameObject)
            {
                ConfirmExitNo();
                return;
            }

            if (confirmYesButton != null && selected == confirmYesButton.gameObject)
            {
                ConfirmExitYes();
                return;
            }
        }

        pendingExitAction = PendingExitAction.ExitWithoutSaving;
        ShowConfirmation("¿QUIERES SALIR SIN GUARDAR?");
    }

    public void ConfirmExitYes()
    {
        if (isExitingToMenu)
        {
            return;
        }

        if (pendingExitAction == PendingExitAction.SaveAndExit)
        {
            DoSaveAndExit();
        }
        else if (pendingExitAction == PendingExitAction.ExitWithoutSaving)
        {
            DoExitWithoutSaving();
        }
        else
        {
            ShowPauseButtons();
        }
    }

    public void ConfirmExitNo()
    {
        pendingExitAction = PendingExitAction.None;
        ShowPauseButtons();
    }

    public void QuitGame()
    {
        ExitWithoutSaving();
    }

    void DoSaveAndExit()
    {
        if (GameManager.Instance != null)
        {
            SaveSystem.SaveGame(GameManager.Instance.BuildSaveData());
        }

        ExitToMainMenu("Guardando partida y volviendo al menu principal...");
    }

    void DoExitWithoutSaving()
    {
        ExitToMainMenu("Volviendo al menu principal sin guardar...");
    }

    void ExitToMainMenu(string logMessage)
    {
        isExitingToMenu = true;
        Time.timeScale = 1f;
        isPaused = false;
        pendingExitAction = PendingExitAction.None;
        pauseMenuUI.SetActive(false);
        Cursor.visible = true;
        Cursor.lockState = CursorLockMode.None;

        Debug.Log(logMessage);
        SceneManager.LoadScene("MainMenu");
    }

    void ShowConfirmation(string message)
    {
        SetActiveIfAssigned(resumeButton, false);
        SetActiveIfAssigned(saveAndExitButton, false);
        SetActiveIfAssigned(exitWithoutSaveButton, false);
        SetActiveIfAssigned(confirmationTextObject, true);
        SetActiveIfAssigned(confirmYesButton, true);
        SetActiveIfAssigned(confirmNoButton, true);

        if (confirmationTextLabel != null)
        {
            confirmationTextLabel.text = message;
        }
    }

    void ShowPauseButtons()
    {
        SetActiveIfAssigned(resumeButton, true);
        SetActiveIfAssigned(saveAndExitButton, true);
        SetActiveIfAssigned(exitWithoutSaveButton, true);
        SetActiveIfAssigned(confirmationTextObject, false);
        SetActiveIfAssigned(confirmYesButton, false);
        SetActiveIfAssigned(confirmNoButton, false);
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

    GameObject FindObjectByName(string objectName)
    {
        Transform[] allTransforms = FindObjectsByType<Transform>(FindObjectsInactive.Include, FindObjectsSortMode.None);
        foreach (Transform t in allTransforms)
        {
            if (t.gameObject.name.Trim() == objectName)
            {
                return t.gameObject;
            }
        }

        return null;
    }
}
