using UnityEngine;
using UnityEngine.InputSystem;
using TMPro;



public class Selected : MonoBehaviour
{
    public CrosshairAnimator crosshairAnimator;
    public PlayerInput playerInput;
    LayerMask mask;
    public float distancia  = 2f;

    private InputAction interactAction;

    public TextMeshProUGUI interactLabel;

    public GameObject crosshair;
    public GameObject cuadroInteract;
    public GameObject interactText;
    private bool isLookingAtDoor;
    private bool wasLookingAtDoorLastFrame;

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



    // Update is called once per frame
void Update()
{

    if (GameManager.Instance.IsIn2D())
    {
            // 1. Verificamos si existe la instancia para evitar el error
            bool talking = false;
            if (DialogueManager.Instance != null)
            {
                talking = DialogueManager.Instance.IsDialogueActive;
            }

            // 2. Usamos esa variable para decidir qué mostrar
            if (talking)
            {
                crosshair.SetActive(false);
                interactText.SetActive(false);
                cuadroInteract.SetActive(false);
            }
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

        RaycastHit hit;
    isLookingAtDoor = false;

    if (Physics.Raycast(transform.position,
        transform.TransformDirection(Vector3.forward),
        out hit, distancia, mask))
    {
        if (hit.collider.CompareTag("PuertaInteractiva"))
        {
            isLookingAtDoor = true;

            interactLabel.text = "E para interactuar";

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

                if (interactAction != null && interactAction.triggered)
            {
                hit.collider.GetComponent<DoorButtonInteraction>().Interact();
            }
        }
    }

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
