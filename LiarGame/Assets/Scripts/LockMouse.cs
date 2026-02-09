using UnityEngine;

public class LockMouse : MonoBehaviour
{

    void Start()
    {
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;

    }

    void Update()
    {
        
    }
}
