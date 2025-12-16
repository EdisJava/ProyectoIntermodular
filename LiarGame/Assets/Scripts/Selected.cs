using UnityEngine;
using UnityEngine.InputSystem;

public class Selected : MonoBehaviour
{
    public PlayerInput playerInput;
    LayerMask mask;
    public float distancia  = 2f;

    private InputAction interactAction;

    void Start()
    {
        mask = LayerMask.GetMask("Raycast Detect");
        if (playerInput != null)
            interactAction = playerInput.actions["Interact"];
    }

    // Update is called once per frame
    void Update()
    {
        RaycastHit hit;

        if (Physics.Raycast(transform.position, transform.TransformDirection(Vector3.forward), out hit, distancia, mask))
        {
            if(hit.collider.tag == "PuertaInteractiva")
            {
                Debug.Log("Objeto Seleccionado: " + hit.collider.name);

                if (interactAction != null && interactAction.triggered)
                {
                    Debug.Log("Interacción con: " + hit.collider.name);
                    hit.collider.GetComponent<DoorButtonInteraction>().Interact();
                }
            }
        }
    }
}
