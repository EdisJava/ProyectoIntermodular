using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class DoorButtonInteraction : MonoBehaviour
{
    private const string LoadingBackgroundResourcePath = "PantallaCarga";
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

    public bool isExitDoor = false;

    [Header("Prologo")]
    public bool marksPrologueLetterAsRead = false;
    public bool isPrologueExitDoor = false;
    public string prologueDayOneSceneName = "SampleScene";
    public bool preventExit = false;

    [Header("UI de Transicion")]
    public GameObject fadePanel;
    public TextMeshProUGUI dayText;

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

    void Update()
    {
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

    public void Interact()
    {
        PlayDoorSound();

        if (isExitDoor)
        {
            if (isPrologueExitDoor)
            {
                if (GameManager.Instance != null && !GameManager.Instance.hasReadPrologueLetter)
                {
                    Debug.Log("Primero debes leer la carta.");
                    return;
                }

                HideInteractionUI();
                StartCoroutine(TransitionFromPrologueToDayOne());
                return;
            }

            if (GameManager.Instance.hasAccusedThisDay)
            {
                HideInteractionUI();
                StartCoroutine(TransitionToNextDay());
            }
            else
            {
                Debug.Log("No puedo irme sin acusar a alguien.");
            }
            return;
        }

        if (marksPrologueLetterAsRead && GameManager.Instance != null)
        {
            GameManager.Instance.hasReadPrologueLetter = true;
        }

        OpenCutscene();
    }

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
    }

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

    private Sprite GetLoadingBackgroundSprite()
    {
        if (cachedLoadingBackgroundSprite != null)
        {
            return cachedLoadingBackgroundSprite;
        }

        cachedLoadingBackgroundSprite = Resources.Load<Sprite>(LoadingBackgroundResourcePath);
        return cachedLoadingBackgroundSprite;
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

    void HideInteractionUI()
    {
        if (crosshair != null) crosshair.SetActive(false);
        if (interactText != null) interactText.SetActive(false);
        if (cuadroInteract != null) cuadroInteract.SetActive(false);
    }
}
