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

        if (movementScript != null)
            movementScript.enabled = false; 
            movementScript.canLook = false;
    }

    void CloseCutscene()
    {
        cutsceneImage.SetActive(false);
        cutsceneActive = false;

        if (movementScript != null)
            movementScript.enabled = true; 
            movementScript.canLook = true;
    }
}
