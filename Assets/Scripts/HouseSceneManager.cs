using UnityEngine;

/*
* Script para manejar la escena de la casa.
* 
* Metodos:
*   - Start(): Metodo que se llama al iniciar la escena.
*   - SetupPrologue(): Metodo que configura la escena para el prologo.
*   - SetupEnding(): Metodo que configura la escena para el final.
*   - SetObjectsActive(): Metodo que activa o desactiva los objetos.
*
*   Variables:
*   - prologueObjects: Objetos que se muestran solo en el prologo.
*   - goodEndingObjects: Objetos que se muestran solo para el final bueno.
*   - badEndingObjects: Objetos que se muestran solo para el final malo.
*   - puertaPlayer: Interaccion con la puerta del jugador.
*   - puertaVecino: Interaccion con la puerta del vecino.
*
*   Funcionamiento:
*   - Al iniciar, verifica si es la fase final.
*   - Si es la fase final, configura la escena para el final.
*   - Si no es la fase final, configura la escena para el prologo.
*
*   Flujo:
*   1. El jugador interactua con el alumno.
*   2. Se llama al metodo Interact().
*   3. Se determina la fase actual del dia.
*   4. Se llama al metodo correspondiente segun la fase.
*   5. Se muestra el dialogo del alumno.
*   6. El jugador puede interactuar con otro alumno.
*/

public class HouseSceneManager : MonoBehaviour
{
    [Header("Escena por defecto (Prologo)")]
    public GameObject[] prologueObjects; // Objetos que solo se muestran en el prologo (ej. detalles u objetos interactivos especificos)

    [Header("Escenarios Finales")]
    public GameObject[] goodEndingObjects; // Objetos que se muestran solo para el final bueno
    public GameObject[] badEndingObjects; // Objetos que se muestran solo para el final malo

    [Header("Interacciones")]
    public DoorButtonInteraction puertaPlayer;
    public DoorButtonInteraction puertaVecino;

    void Start()
    {
        if (GameManager.Instance != null && GameManager.Instance.isEndingPhase)
        {
            SetupEnding();
        }
        else
        {
            SetupPrologue();
        }
    }

    void SetupPrologue()
    {
        SetObjectsActive(prologueObjects, true);
        SetObjectsActive(goodEndingObjects, false);
        SetObjectsActive(badEndingObjects, false);

        // En el prologo configura las puertas a su estado por defecto
        if (puertaPlayer != null)
        {
            puertaPlayer.preventExit = false;
        }

        if (puertaVecino != null)
        {
            puertaVecino.preventExit = false;
        }
    }

    void SetupEnding()
    {
        SetObjectsActive(prologueObjects, false); // Ocultar cosas que no deben estar en el final
        
        bool isGoodEnding = GameManager.Instance.isGoodEnding;
        // Dependiendo de si es buen final o mal final, se activan los objetos correspondientes
        SetObjectsActive(goodEndingObjects, isGoodEnding);
        SetObjectsActive(badEndingObjects, !isGoodEnding);

        if (puertaPlayer != null)
        {
            // El jugador no debe poder salir del dialogo al interactuar con puerta player en el final
            puertaPlayer.preventExit = true;
        }

        if (puertaVecino != null)
        {
            puertaVecino.preventExit = false; // Puerta vecino si permite salir si se interactua
        }
    }

    void SetObjectsActive(GameObject[] list, bool state)
    {
        if (list == null) return;
        foreach (GameObject obj in list)
        {
            if (obj != null)
            {
                obj.SetActive(state);
            }
        }
    }
}
