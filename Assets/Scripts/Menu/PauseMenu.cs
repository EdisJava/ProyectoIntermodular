using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.InputSystem; // Â¡IMPORTANTE: AÃ±ade esta lÃ­nea!

public class PauseMenu : MonoBehaviour
{
    public GameObject pauseMenuUI;
    private bool isPaused = false;
    public FirstPlayerController movementScript;

    void Update()
    {
        // Usamos la misma sintaxis que tu DoorButtonInteraction
        if (Keyboard.current != null && Keyboard.current.escapeKey.wasPressedThisFrame)
        {
            if (isPaused)
                Resume();
            else
                Pause();
        }
    }

    public void Resume()
    {
        pauseMenuUI.SetActive(false);
        Time.timeScale = 1f; 
        isPaused = false;

        // Bloqueamos el cursor si volvemos a la exploraciÃ³n
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

        // Liberamos el ratÃ³n
        Cursor.visible = true;
        Cursor.lockState = CursorLockMode.None;
        movementScript.enabled = false;
        movementScript.canLook = false;
    }

    public void QuitGame()
    {
        if (GameManager.Instance != null)
        {
            SaveSystem.SaveGame(GameManager.Instance.BuildSaveData());
        }

        Time.timeScale = 1f;
        Cursor.visible = true;
        Cursor.lockState = CursorLockMode.None;
        Debug.Log("Guardando partida y volviendo al menú principal...");
        SceneManager.LoadScene("MainMenu");
    }
}
