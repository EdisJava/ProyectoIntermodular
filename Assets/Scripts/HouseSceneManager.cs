using UnityEngine;

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

        // En el prologo configuramos las puertas a su estado por defecto
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
