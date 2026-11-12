using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class FinalDoorSequence : MonoBehaviour
{
    [Header("Referencias de tu Sistema")]
    public FirstPlayerController movementScript;
    public AudioClip DoorOpenAudio;
    public AudioSource audioSource;

    public GameObject crosshair;
    public GameObject cuadroInteract;
    public GameObject interactText;

    [Header("Configuración del Diálogo Final")]
    public DialogueData finalDialogue;

    private bool sequenceActive = false;

    void Awake()
    {
        // Setup de audio idéntico al tuyo
        if (audioSource == null) audioSource = GetComponent<AudioSource>();
        if (audioSource == null) audioSource = gameObject.AddComponent<AudioSource>();

        if (audioSource != null)
        {
            audioSource.playOnAwake = false;
            audioSource.spatialBlend = 0f;
            audioSource.volume = 1f;
        }
    }

    public void Interact()
    {
        if (Time.timeScale == 0f || sequenceActive) return;

        // 1. Sonido de apertura
        PlayDoorSound();

        // 2. Ocultar UI de interacción (usando tu método)
        HideInteractionUI();

        // 3. Bloqueo de jugador y cámara (idéntico a tu OpenCutscene)
        if (movementScript != null)
        {
            movementScript.enabled = false;
            movementScript.canLook = false;
        }

        // 4. Liberar Cursor para el diálogo
        Cursor.visible = true;
        Cursor.lockState = CursorLockMode.None;

        // 5. Cambiar Estado del Juego
        GameManager.Instance.currentState = GameState.Interaction2D;

        // 6. Lanzar el Diálogo Final
        // Pasamos null en el NPC porque es una secuencia directa
        if (DialogueManager.Instance != null && finalDialogue != null)
        {
            sequenceActive = true;
            DialogueManager.Instance.StartDialogue(finalDialogue, (StudentNPC)null);
        }
    }

    // Métodos de apoyo copiados de tu script original para asegurar compatibilidad
    void PlayDoorSound()
    {
        if (audioSource != null && DoorOpenAudio != null)
        {
            audioSource.PlayOneShot(DoorOpenAudio);
        }
    }

    void HideInteractionUI()
    {
        if (crosshair != null) crosshair.SetActive(false);
        if (interactText != null) interactText.SetActive(false);
        if (cuadroInteract != null) cuadroInteract.SetActive(false);
    }
}