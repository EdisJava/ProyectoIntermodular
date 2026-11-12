using UnityEngine;
using UnityEngine.InputSystem;
using TMPro;

/*
* Script para detectar objetos interactivos.
* 
* Metodos:
*   - Start(): Metodo que se ejecuta al iniciar el script.
*   - Update(): Metodo que se ejecuta en cada frame.
*
*   Variables:
*   - crosshairAnimator: Animador de la mira.
*   - playerInput: Input del jugador.
*   - mask: Mascara de capas.
*   - distancia: Distancia de deteccion.
*   - interactAction: Accion de interaccion.
*   - interactLabel: Etiqueta de interaccion.
*   - crosshair: Mira.
*   - cuadroInteract: Cuadro de interaccion.
*   - interactText: Texto de interaccion.
*   - isLookingAtDoor: Si esta mirando a una puerta.
*   - wasLookingAtDoorLastFrame: Si estaba mirando a una puerta el frame anterior.
*
*   Funcionamiento:
*   - Detecta objetos interactivos y muestra un mensaje de interaccion.
*
*   Flujo:
*   1. El jugador mira a un objeto interactivo.
*   2. Se muestra un mensaje de interaccion.
*   3. El jugador interactua con el objeto.
*/

public class Selected : MonoBehaviour
{
    public CrosshairAnimator crosshairAnimator;
    public PlayerInput playerInput;
    private LayerMask mask;
    public float distancia = 2f;

    private InputAction interactAction;

    public TextMeshProUGUI interactLabel;

    public GameObject crosshair;
    public GameObject cuadroInteract;
    public GameObject interactText;
    private bool isLookingAtDoor;
    private bool wasLookingAtDoorLastFrame;

    /*
    * Metodo que se ejecuta al iniciar el script.
    */
    void Start()
    {
        mask = LayerMask.GetMask("Raycast Detect");
        if (playerInput != null)
            interactAction = playerInput.actions["Interact"];

        crosshair.SetActive(false);
        interactText.SetActive(false);
        cuadroInteract.SetActive(false);
        interactLabel.text = "E para interactuar";
    }

    /*
    * Metodo que se ejecuta en cada frame.
    */
    void Update()
    {
        if (Time.timeScale == 0f)
        {
            crosshair.SetActive(false);
            interactText.SetActive(false);
            cuadroInteract.SetActive(false);
            wasLookingAtDoorLastFrame = false;
            return;
        }
        // Si esta en 2D
        if (GameManager.Instance.IsIn2D())
        {
            bool talking = false;
            // Si esta hablando
            if (DialogueManager.Instance != null)
            {
                talking = DialogueManager.Instance.IsDialogueActive;
            }
            if (talking)
            {
                crosshair.SetActive(false);
                interactText.SetActive(false);
                cuadroInteract.SetActive(false);
            }
            // Si no esta hablando
            else
            {
                crosshair.SetActive(false);
                interactText.SetActive(true);
                cuadroInteract.SetActive(true);
                interactLabel.text = "ESPACIO para salir";
            }

            wasLookingAtDoorLastFrame = false;
            return;
        }
        // Si esta en 3D
        // Raycast para detectar objetos interactivos
        RaycastHit hit;
        isLookingAtDoor = false;
        // Si el raycast golpea algo
        if (Physics.Raycast(transform.position,
            transform.TransformDirection(Vector3.forward),
            out hit, distancia, mask))
        {
            // Si el objeto golpeado es una puerta interactiva
            if (hit.collider.CompareTag("PuertaInteractiva"))
            {
                isLookingAtDoor = true;

                DoorButtonInteraction interaction = hit.collider.GetComponent<DoorButtonInteraction>();

                if (interaction != null && interaction.isPrologueExitDoor)
                {
                    interactLabel.text = "Pulsa E para ir a clases";
                }
                else if (interaction != null && interaction.isExitDoor)
                {
                    interactLabel.text = "E para ir a casa";
                }
                else
                {
                    interactLabel.text = "E para interactuar";
                }

                if (!wasLookingAtDoorLastFrame)
                {
                    crosshair.SetActive(true);
                    interactText.SetActive(true);
                    cuadroInteract.SetActive(true);
                    if (crosshairAnimator != null)
                    {
                        crosshairAnimator.PlayAppear();
                    }
                }

                if (interactAction != null && interactAction.triggered && interaction != null)
                {
                    interaction.Interact();
                }
            }
        }
        // Si no esta mirando a una puerta
        if (!isLookingAtDoor)
        {
            crosshair.SetActive(false);
            interactText.SetActive(false);
            cuadroInteract.SetActive(false);

            if (crosshairAnimator != null)
                crosshairAnimator.StopIdle();
        }

        wasLookingAtDoorLastFrame = isLookingAtDoor;
    }
}
