using UnityEngine;

/*
* Script para bloquear el mouse.
* 
* Metodos:
*   - Start(): Metodo que se llama al iniciar la escena.
*
*   Variables:
*   - None
*
*   Funcionamiento:
*   - Al iniciar, bloquea el mouse.
*
*   Flujo:
*   1. El jugador inicia el juego.
*   2. Se bloquea el mouse.
*/

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
