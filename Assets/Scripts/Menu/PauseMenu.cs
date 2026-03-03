using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.InputSystem; // ¡IMPORTANTE: Añade esta línea!

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

        // Bloqueamos el cursor si volvemos a la exploración
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

        // Liberamos el ratón
        Cursor.visible = true;
        Cursor.lockState = CursorLockMode.None;
        movementScript.enabled = false;
        movementScript.canLook = false;
    }

    public void QuitGame()
    {
        Debug.Log("Saliendo del juego...");
        Application.Quit();
    }
}