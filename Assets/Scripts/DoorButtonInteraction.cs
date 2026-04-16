using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

/*
* Script para manejar la interaccion con la puerta.
* 
* Metodos:
*   - Interact(): Metodo principal que se llama al interactuar con la puerta.
*   - CloseCutscene(): Metodo que se llama para cerrar la cutscene.
*   - TransitionToNextDay(): Metodo que se llama para transicionar al siguiente dia.
*   - TransitionFromPrologueToDayOne(): Metodo que se llama para transicionar del prologo al dia 1.
*   - ApplyLoadingBackgroundToFadePanel(): Metodo que aplica el fondo de carga al panel de transicion.
*   - GetLoadingBackgroundSprite(): Metodo que obtiene el sprite de fondo de carga.
*   - CreateRuntimeTransitionOverlay(): Metodo que crea un overlay de transicion en tiempo de ejecucion.
*   - PlayDoorSound(): Metodo que reproduce el sonido de la puerta.
*   - PlayDoorOpenSound(): Metodo que reproduce el sonido de la puerta abriendose.
*   - PlayDoorCloseSound(): Metodo que reproduce el sonido de la puerta cerrandose.
*   - ShowInteractionUI(): Metodo que muestra la UI de interaccion.
*   - HideInteractionUI(): Metodo que oculta la UI de interaccion.
*
*   Variables:
*   - cutsceneImage: Imagen de la cutscene.
*   - movementScript: Script de movimiento del jugador.
*   - DoorOpenAudio: Sonido de la puerta abriendose.
*   - DoorCloseAudio: Sonido de la puerta cerrandose.
*   - audioSource: Fuente de audio.
*   - crosshair: Mira del jugador.
*   - cuadroInteract: Cuadro de interaccion.
*   - interactText: Texto de interaccion.
*   - cutsceneActive: Si la cutscene esta activa.
*   - isDayTransitionInProgress: Si la transicion de dia esta en progreso.
*   - isExitDoor: Si la puerta es una puerta de salida.
*   - marksPrologueLetterAsRead: Si la puerta marca la carta del prologo como leida.
*   - isPrologueExitDoor: Si la puerta es una puerta de salida del prologo.
*   - prologueDayOneSceneName: Nombre de la escena del dia 1 del prologo.
*   - preventExit: Si se previene la salida.
*   - fadePanel: Panel de transicion.
*   - dayText: Texto del dia.
*
*   Funcionamiento:
*   - Al interactuar con la puerta, se llama al metodo Interact().
*   - El metodo Interact() llama al metodo correspondiente segun la fase actual del dia.
*   - En la fase de dialogo casual, se llama al metodo CasualTalk().
*   - En la fase de investigacion, se llama al metodo InvestigationTalk().
*   - En la fase de decision, se llama al metodo Accuse().
*   - El metodo ResetForNewDay() se llama al inicio de cada dia.
*
*   Flujo:
*   1. El jugador interactua con la puerta.
*   2. Se llama al metodo Interact().
*   3. Se determina la fase actual del dia.
*   4. Se llama al metodo correspondiente segun la fase.
*   5. Se muestra el dialogo del alumno.
*   6. El jugador puede interactuar con otro alumno.
*/

public class DoorButtonInteraction : MonoBehaviour
{
    private const string LoadingBackgroundResourcePath = "PantallaCarga";
    private const string PrologueSceneName = "HouseScenePrologue";
    private static Sprite cachedLoadingBackgroundSprite;

    public GameObject cutsceneImage;

    public FirstPlayerController movementScript;
    public AudioClip DoorOpenAudio;
    public AudioClip DoorCloseAudio;
    public AudioSource audioSource;

    public GameObject crosshair;
    public GameObject cuadroInteract;
    public GameObject interactText;

    private bool cutsceneActive = false;
    private bool isDayTransitionInProgress = false;

    public bool isExitDoor = false;

    [Header("Prologo")]
    public bool marksPrologueLetterAsRead = false;
    public bool isPrologueExitDoor = false;
    public string prologueDayOneSceneName = "SampleScene";
    public bool preventExit = false;

    [Header("UI de Transicion")]
    public GameObject fadePanel;
    public TextMeshProUGUI dayText;

    /*
    * Metodo que se llama al iniciar el script.
    */
    void Awake()
    {
        if (audioSource == null)
        {
            audioSource = GetComponent<AudioSource>();
        }

        if (audioSource == null)
        {
            audioSource = gameObject.AddComponent<AudioSource>();
        }

        if (audioSource != null)
        {
            audioSource.playOnAwake = false;
            audioSource.spatialBlend = 0f;
            audioSource.volume = 1f;
        }
    }

    /*
    * Metodo que se llama en cada frame.
    */
    void Update()
    {
        if (Time.timeScale == 0f)
        {
            return;
        }

        if (!cutsceneActive)
        {
            return;
        }

        if (DialogueManager.Instance != null && DialogueManager.Instance.IsDialogueActive)
        {
            return;
        }

        if (Keyboard.current.spaceKey.wasPressedThisFrame)
        {
            if (!preventExit)
            {
                CloseCutscene();
            }
        }
    }

    /*
    * Metodo que se llama al interactuar con la puerta.
    */
    public void Interact()
    {
        if (Time.timeScale == 0f)
        {
            return;
        }

        if (isDayTransitionInProgress)
        {
            return;
        }

        PlayDoorSound();
        /*
        * Si la puerta es una puerta de salida, se llama al metodo TransitionToNextDay().
        * Si la puerta es una puerta de entrada, se llama al metodo CloseCutscene().
        */
        if (isExitDoor)
        {
            bool isInPrologueScene = SceneManager.GetActiveScene().name == PrologueSceneName;

            if (isPrologueExitDoor && isInPrologueScene)
            {
                if (GameManager.Instance != null && !GameManager.Instance.hasReadPrologueLetter)
                {
                    Debug.Log("Primero debes leer la carta.");
                    return;
                }

                HideInteractionUI();
                isDayTransitionInProgress = true;
                StartCoroutine(TransitionFromPrologueToDayOne());
                return;
            }

            if (GameManager.Instance.hasAccusedThisDay)
            {
                HideInteractionUI();
                isDayTransitionInProgress = true;
                StartCoroutine(TransitionToNextDay());
            }
            else
            {
                Debug.Log("No puedo irme sin acusar a alguien.");
            }
            return;
        }
        /*
        * Si la puerta marca la carta del prologo como leida, se llama al metodo MarkPrologueLetterAsRead().
        */
        if (marksPrologueLetterAsRead && GameManager.Instance != null)
        {
            GameManager.Instance.hasReadPrologueLetter = true;
        }
        /*
        * Abre la cutscene.
        */
        OpenCutscene();
    }

    /*
    * Metodo que se llama para transicionar al siguiente dia.
    */
    IEnumerator TransitionToNextDay()
    {
        ApplyLoadingBackgroundToFadePanel();

        if (fadePanel != null)
        {
            fadePanel.SetActive(true);
        }

        PlayDoorCloseSound();

        if (dayText != null)
        {
            if (GameManager.Instance != null && GameManager.Instance.currentDay >= GameManager.Instance.GetPlayableDayCount())
            {
                dayText.text = "FINAL";
            }
            else
            {
                int proximoDia = GameManager.Instance.currentDay + 1;
                dayText.text = "DIA " + proximoDia;
            }
            dayText.gameObject.SetActive(true);
        }
        /*
        * Espera 5 segundos.
        */
        yield return new WaitForSeconds(5f);

        GameManager.Instance.NextDay();

        if (dayText != null)
        {
            dayText.gameObject.SetActive(false);
        }

        if (fadePanel != null)
        {
            fadePanel.SetActive(false);
        }

        isDayTransitionInProgress = false;
    }

    /*
    * Metodo que se llama para transicionar del prologo al dia 1.
    */
    IEnumerator TransitionFromPrologueToDayOne()
    {
        GameObject runtimeOverlay = null;

        ApplyLoadingBackgroundToFadePanel();

        if (fadePanel != null)
        {
            fadePanel.SetActive(true);
        }
        else
        {
            runtimeOverlay = CreateRuntimeTransitionOverlay();
        }

        if (dayText != null)
        {
            dayText.text = "DIA 1";
            dayText.gameObject.SetActive(true);
        }

        yield return new WaitForSeconds(1.2f);
        SceneManager.LoadScene(prologueDayOneSceneName);

        if (runtimeOverlay != null)
        {
            Destroy(runtimeOverlay);
        }
    }

    /*
    * Metodo que se llama para crear un overlay de transicion en tiempo de ejecucion.
    */
    private GameObject CreateRuntimeTransitionOverlay()
    {
        GameObject canvasGO = new GameObject("PrologueTransitionCanvas");
        Canvas canvas = canvasGO.AddComponent<Canvas>();
        canvas.renderMode = RenderMode.ScreenSpaceOverlay;
        canvas.sortingOrder = 500;

        CanvasScaler scaler = canvasGO.AddComponent<CanvasScaler>();
        scaler.uiScaleMode = CanvasScaler.ScaleMode.ScaleWithScreenSize;
        scaler.referenceResolution = new Vector2(1920f, 1080f);

        canvasGO.AddComponent<GraphicRaycaster>();

        GameObject panelGO = new GameObject("FadePanel");
        panelGO.transform.SetParent(canvasGO.transform, false);
        RectTransform panelRect = panelGO.AddComponent<RectTransform>();
        panelRect.anchorMin = Vector2.zero;
        panelRect.anchorMax = Vector2.one;
        panelRect.offsetMin = Vector2.zero;
        panelRect.offsetMax = Vector2.zero;

        Image panelImage = panelGO.AddComponent<Image>();
        Sprite loadingSprite = GetLoadingBackgroundSprite();
        if (loadingSprite != null)
        {
            panelImage.sprite = loadingSprite;
            panelImage.color = Color.white;
            panelImage.preserveAspect = false;
        }
        else
        {
            panelImage.color = Color.black;
        }

        GameObject textGO = new GameObject("DayText");
        textGO.transform.SetParent(panelGO.transform, false);
        RectTransform textRect = textGO.AddComponent<RectTransform>();
        textRect.anchorMin = new Vector2(0.5f, 0.5f);
        textRect.anchorMax = new Vector2(0.5f, 0.5f);
        textRect.pivot = new Vector2(0.5f, 0.5f);
        textRect.anchoredPosition = Vector2.zero;
        textRect.sizeDelta = new Vector2(800f, 180f);

        TextMeshProUGUI tmp = textGO.AddComponent<TextMeshProUGUI>();
        if (TMP_Settings.defaultFontAsset != null)
        {
            tmp.font = TMP_Settings.defaultFontAsset;
        }
        tmp.text = "DIA 1";
        tmp.alignment = TextAlignmentOptions.Center;
        tmp.fontSize = 80f;
        tmp.color = Color.white;

        return canvasGO;
    }

    /*
    * Metodo que se llama para aplicar el fondo de carga al panel de transicion.
    */
    private void ApplyLoadingBackgroundToFadePanel()
    {
        if (fadePanel == null)
        {
            return;
        }

        Image panelImage = fadePanel.GetComponent<Image>();
        if (panelImage == null)
        {
            return;
        }

        Sprite loadingSprite = GetLoadingBackgroundSprite();
        if (loadingSprite == null)
        {
            return;
        }

        panelImage.sprite = loadingSprite;
        panelImage.color = Color.white;
        panelImage.preserveAspect = false;
    }

    /*
    * Metodo que se llama para obtener el sprite de fondo de carga.
    */
    private Sprite GetLoadingBackgroundSprite()
    {
        if (cachedLoadingBackgroundSprite != null)
        {
            return cachedLoadingBackgroundSprite;
        }

        cachedLoadingBackgroundSprite = Resources.Load<Sprite>(LoadingBackgroundResourcePath);
        return cachedLoadingBackgroundSprite;
    }

    /*
    * Metodo que se llama para abrir la cutscene.
    */
    void OpenCutscene()
    {   
        /*
        * Hace visible el cursor y desbloquea el cursor.
        */
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
        /*
        * Obtiene el RoomManager del objeto cutsceneImage.
        */
        RoomManager roomManager = cutsceneImage.GetComponent<RoomManager>();

        if (roomManager != null)
        {
            roomManager.RefreshRoom();
        }
        else
        {
            Debug.LogError("El objeto cutsceneImage no tiene el script RoomManager.");
        }
    }

    /*
    * Metodo que se llama para cerrar la cutscene.
    */
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

    /*
    * Metodo que se llama para reproducir el sonido de la puerta.
    */
    void PlayDoorSound()
    {
        if (audioSource != null && DoorOpenAudio != null)
        {
            audioSource.PlayOneShot(DoorOpenAudio);
        }
        else if (DoorOpenAudio == null)
        {
            Debug.LogWarning("Falta asignar el clip 'Door Open Audio' en el inspector.");
        }
    }

    /*
    * Metodo que se llama para reproducir el sonido de la puerta.
    */
    void PlayDoorCloseSound()
    {
        if (audioSource != null && DoorCloseAudio != null)
        {
            audioSource.PlayOneShot(DoorCloseAudio);
        }
        else if (DoorCloseAudio == null)
        {
            Debug.LogWarning("Falta asignar el clip 'Door Close Audio' en el inspector.");
        }
    }

    /*
    * Metodo que se llama para ocultar la UI de interaccion.
    */
    void HideInteractionUI()
    {
        if (crosshair != null) crosshair.SetActive(false);
        if (interactText != null) interactText.SetActive(false);
        if (cuadroInteract != null) cuadroInteract.SetActive(false);
    }
}
