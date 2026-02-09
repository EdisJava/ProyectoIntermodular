using UnityEngine;
using UnityEngine.InputSystem;

public class DoorButtonInteraction : MonoBehaviour
{
    public GameObject cutsceneImage;
    public GameObject goran;
    
    public FirstPlayerController movementScript;  

    private bool cutsceneActive = false;
    private bool goranActive = false;

    void Update()
    {
        if (cutsceneActive)
        {
            if (Keyboard.current.spaceKey.wasPressedThisFrame || Keyboard.current.enterKey.wasPressedThisFrame)
            {
                CloseCutscene();
            }
        }
    }

    public void Interact()
    {
        OpenCutscene();
    }

    void OpenCutscene()
{
        Cursor.visible = true;
        Cursor.lockState = CursorLockMode.None;
        cutsceneImage.SetActive(true);
        cutsceneActive = true;
        goran.SetActive(true);
        goranActive = true;

        GameManager.Instance.currentState = GameState.Interaction2D;

    if (movementScript != null)
    {
        movementScript.enabled = false;
        movementScript.canLook = false;
    }
}

void CloseCutscene()
{
    cutsceneImage.SetActive(false);
    cutsceneActive = false;
        goran.SetActive(false);
        goranActive = false;
        Cursor.visible = false;
        Cursor.lockState = CursorLockMode.Locked;

        GameManager.Instance.currentState = GameState.Exploration3D;

    if (movementScript != null)
    {
        movementScript.enabled = true;
        movementScript.canLook = true;
    }
}
}
