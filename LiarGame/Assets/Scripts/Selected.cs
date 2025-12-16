using UnityEngine;
using UnityEngine.InputSystem;

public class Selected : MonoBehaviour
{
    public PlayerInput playerInput;
    LayerMask mask;
    public float distancia  = 2f;

    private InputAction interactAction;

    public GameObject crosshair;
    public GameObject interactText;
    private bool isLookingAtDoor;

    void Start()
    {
        mask = LayerMask.GetMask("Raycast Detect");
        if (playerInput != null)
            interactAction = playerInput.actions["Interact"];
        crosshair.SetActive(false);
        interactText.SetActive(false);
    }

    // Update is called once per frame
    void Update()
    {
        RaycastHit hit;
        isLookingAtDoor = false;

        if (Physics.Raycast(transform.position, transform.TransformDirection(Vector3.forward), out hit, distancia, mask))
        {
            if(hit.collider.tag == "PuertaInteractiva")
            {
                isLookingAtDoor = true;
                Debug.Log("Objeto Seleccionado: " + hit.collider.name);


                if (interactAction != null && interactAction.triggered)
                {
                    Debug.Log("Interacción con: " + hit.collider.name);
                    hit.collider.GetComponent<DoorButtonInteraction>().Interact();
                }
            }
        }

        crosshair.SetActive(isLookingAtDoor);
        interactText.SetActive(isLookingAtDoor);
    }
}
