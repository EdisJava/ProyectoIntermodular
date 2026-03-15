using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class MainMenu : MonoBehaviour
{
  
    public GameObject mainMenu;
    public Button playButton;

    void Awake()
    {
        if (playButton == null)
        {
            GameObject playButtonObject = GameObject.Find("JugarButton");
            if (playButtonObject != null)
            {
                playButton = playButtonObject.GetComponent<Button>();
            }
        }
    }

    void Start()
    {
        RefreshButtons();
    }

    public void OpenMainMenuPanel()
    {
        mainMenu.SetActive(true);
        RefreshButtons();
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
        SaveSystem.DeleteSave();
        RefreshButtons();
        SceneManager.LoadScene("SampleScene");
    }

    void RefreshButtons()
    {
        if (playButton != null)
        {
            playButton.interactable = SaveSystem.HasSave();
        }
    }
}
