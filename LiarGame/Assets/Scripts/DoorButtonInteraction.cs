using UnityEngine;
using UnityEngine.InputSystem;

public class DoorButtonInteraction : MonoBehaviour
{
    public GameObject cutsceneImage;
    public FirstPlayerController movementScript;  

    private bool cutsceneActive = false;

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
    cutsceneImage.SetActive(true);
    cutsceneActive = true;

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

    GameManager.Instance.currentState = GameState.Exploration3D;

    if (movementScript != null)
    {
        movementScript.enabled = true;
        movementScript.canLook = true;
    }
}
}
