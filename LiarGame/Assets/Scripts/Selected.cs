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

    if (GameManager.Instance.IsIn2D())
    {
        crosshair.SetActive(false);
        interactText.SetActive(false);
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

            if (interactAction != null && interactAction.triggered)
            {
                hit.collider.GetComponent<DoorButtonInteraction>().Interact();
            }
        }
    }

    crosshair.SetActive(isLookingAtDoor);
    interactText.SetActive(isLookingAtDoor);
}
}
