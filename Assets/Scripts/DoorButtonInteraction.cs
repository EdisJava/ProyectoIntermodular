using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.InputSystem;

public class DoorButtonInteraction : MonoBehaviour
{
    public GameObject cutsceneImage;
    
    public FirstPlayerController movementScript;  
    public AudioClip DoorOpenAudio;
    public AudioClip DoorCloseAudio;
    public AudioSource audioSource;

    public GameObject crosshair;
    public GameObject cuadroInteract;
    public GameObject interactText;

    private bool cutsceneActive = false;

    public bool isExitDoor = false; 
    

    void Awake()
    {
        if (audioSource == null)
        {
            audioSource = GetComponent<AudioSource>();
        }

        // Si la puerta no tiene el componente AudioSource, se lo añadimos por código
        if (audioSource == null)
        {
            audioSource = gameObject.AddComponent<AudioSource>();
        }

        // Nos aseguramos de configurarlo para que se escuche siempre a volumen máximo
        if (audioSource != null)
        {
            audioSource.playOnAwake = false;
            audioSource.spatialBlend = 0f; // 0 = Sonido 2D (se escucha en toda la habitación igual)
            audioSource.volume = 1f;       // Volumen al 100%
        }
    }

    void Update()
    {
        if (cutsceneActive)
        {
            // Importante: Usar .Instance para acceder a la copia que está viva en la escena
            if (DialogueManager.Instance != null && DialogueManager.Instance.IsDialogueActive)
            {
                return; // Si hay diálogo, no hacemos nada más en este Update
            }

            if (Keyboard.current.spaceKey.wasPressedThisFrame ||
                Keyboard.current.enterKey.wasPressedThisFrame ||
                Keyboard.current.escapeKey.wasPressedThisFrame)
            {
                CloseCutscene();
            }
        }
    }

    public void Interact()
    {
        PlayDoorSound();

        if (isExitDoor)
        {
            // Solo dejamos salir si ya acusó
            if (GameManager.Instance.hasAccusedThisDay)
            {
                StartCoroutine(TransitionToNextDay());
                crosshair.SetActive(false);
                interactText.SetActive(false);
                cuadroInteract.SetActive(false);
            }
            else
            {
                Debug.Log("No puedo irme sin acusar a alguien.");
            }
        }
        else
        {
            OpenCutscene();
        }
    }

    void PlayDoorSound()
    {
        if (audioSource != null && DoorOpenAudio != null)
        {
            audioSource.PlayOneShot(DoorOpenAudio);
            Debug.Log("🔊 Reproduciendo sonido de abrir puerta");
        }
        else if (DoorOpenAudio == null)
        {
            Debug.LogWarning("⚠️ Falta asignar el clip 'Door Open Audio' en el Inspector del botón de la puerta.");
        }
    }

    [Header("UI de Transición")]
    public GameObject fadePanel;
    public TextMeshProUGUI dayText;

    IEnumerator TransitionToNextDay()
    {
        // 1. Pantalla en negro
        fadePanel.SetActive(true);

        // Reproducimos el sonido de cerrar al salir
        PlayDoorCloseSound();

        if (dayText != null)
        {
            // Sumamos 1 porque el cambio en el GameManager aún no ha ocurrido
            int proximoDia = GameManager.Instance.currentDay + 1;
            dayText.text = "DÍA " + proximoDia;
            dayText.gameObject.SetActive(true);
        }

        // 2. Esperar 5 segundos
        yield return new WaitForSeconds(5f);

        // 3. Pasar de día
        GameManager.Instance.NextDay();

        if (dayText != null)
        {
            dayText.gameObject.SetActive(false);
        }

        // 4. Quitar pantalla en negro
        fadePanel.SetActive(false);
    }

    void OpenCutscene()
    {
        Cursor.visible = true;
        Cursor.lockState = CursorLockMode.None;
        cutsceneImage.SetActive(true);
        cutsceneActive = true;

        GameManager.Instance.currentState = GameState.Interaction2D;

        if (movementScript != null)
        {
            movementScript.enabled = false;
            movementScript.canLook = false;
        }
        // 2. Buscamos el RoomManager en el objeto que acabamos de activar
        RoomManager roomManager = cutsceneImage.GetComponent<RoomManager>();

        if (roomManager != null)
        {
            // 3. Esta función es la que activará a Goran según el día
            roomManager.RefreshRoom();
        }
        else
        {
            Debug.LogError("¡Ojo! El objeto cutsceneImage no tiene el script RoomManager.");
        }
    }

    void CloseCutscene()
    {
        PlayDoorCloseSound();
        cutsceneImage.SetActive(false);
        cutsceneActive = false;
        Cursor.visible = false;
        Cursor.lockState = CursorLockMode.Locked;

        GameManager.Instance.currentState = GameState.Exploration3D;

        if (movementScript != null)
        {
            movementScript.enabled = true;
            movementScript.canLook = true;
        }
    }

    void PlayDoorCloseSound()
    {
        if (audioSource != null && DoorCloseAudio != null)
        {
            audioSource.PlayOneShot(DoorCloseAudio);
            Debug.Log("🔊 Reproduciendo sonido de cerrar puerta");
        }
        else if (DoorCloseAudio == null)
        {
            Debug.LogWarning("⚠️ Falta asignar el clip 'Door Close Audio' en el Inspector del botón de la puerta.");
        }
    }
}
