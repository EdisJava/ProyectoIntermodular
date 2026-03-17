using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;

public class PauseMenu : MonoBehaviour
{
    public GameObject pauseMenuUI;
    public FirstPlayerController movementScript;

    public Button resumeButton;
    public Button saveAndExitButton;
    public Button exitWithoutSaveButton;
    public Button settingsButton;

    public GameObject confirmationTextObject;
    public TextMeshProUGUI confirmationTextLabel;
    public Button confirmYesButton;
    public Button confirmNoButton;

    public GameObject settingsMenuUI;
    public Button settingsBackButton;
    public Slider volumeSlider;
    public Slider brightnessSlider;
    public Toggle fullscreenToggle;
    public TMP_Dropdown resolutionDropdown;
    public Image brightnessOverlay;

    private bool isPaused = false;
    private bool isExitingToMenu = false;
    private bool isApplyingUIState = false;
    private PlayerSettingsData playerSettings;
    private readonly List<GameObject> settingsUiElements = new List<GameObject>();
    private ColorAdjustments colorAdjustments;
    private Image settingsMenuBackgroundImage;
    private Color settingsMenuBackgroundBaseColor = Color.white;

    private readonly Vector2Int[] commonResolutions =
    {
        new Vector2Int(1920, 1080),
        new Vector2Int(1600, 900),
        new Vector2Int(1366, 768),
        new Vector2Int(1280, 720)
    };

    private enum PendingExitAction
    {
        None,
        SaveAndExit,
        ExitWithoutSaving
    }

    private PendingExitAction pendingExitAction = PendingExitAction.None;

    void Awake()
    {
        FindPauseMenuReferences();
        FindSettingsReferences();
        CacheSettingsMenuBackground();
        CollectSettingsUiElements();
        WireButtons();
        EnsureBrightnessOverlay();
        CacheColorAdjustments();
        LoadAndApplyPlayerSettings();
    }

    void Update()
    {
        if (Keyboard.current == null || !Keyboard.current.escapeKey.wasPressedThisFrame)
        {
            return;
        }

        if (isPaused)
        {
            if (settingsMenuUI != null && settingsMenuUI.activeSelf)
            {
                CloseSettingsMenu();
                return;
            }

            if (pendingExitAction != PendingExitAction.None)
            {
                ConfirmExitNo();
                return;
            }

            Resume();
            return;
        }

        Pause();
    }

    public void Resume()
    {
        GameObject selected = EventSystem.current != null ? EventSystem.current.currentSelectedGameObject : null;
        if (isPaused && settingsButton != null && selected == settingsButton.gameObject)
        {
            OpenSettingsMenu();
            return;
        }

        if (settingsMenuUI != null && settingsMenuUI.activeSelf && settingsBackButton != null && selected == settingsBackButton.gameObject)
        {
            CloseSettingsMenu();
            return;
        }

        pendingExitAction = PendingExitAction.None;
        ShowPauseButtons();
        if (settingsMenuUI != null)
        {
            settingsMenuUI.SetActive(false);
        }

        if (pauseMenuUI != null)
        {
            pauseMenuUI.SetActive(false);
        }

        Time.timeScale = 1f;
        isPaused = false;

        if (GameManager.Instance != null && GameManager.Instance.currentState == GameState.Exploration3D)
        {
            Cursor.visible = false;
            Cursor.lockState = CursorLockMode.Locked;
        }

        if (movementScript != null)
        {
            movementScript.enabled = true;
            movementScript.canLook = true;
        }
    }

    public void SaveAndExit()
    {
        pendingExitAction = PendingExitAction.SaveAndExit;
        ShowConfirmation("QUIERES GUARDAR Y SALIR?");
    }

    public void ExitWithoutSaving()
    {
        GameObject selected = EventSystem.current != null ? EventSystem.current.currentSelectedGameObject : null;
        if (selected != null)
        {
            if (confirmNoButton != null && selected == confirmNoButton.gameObject)
            {
                ConfirmExitNo();
                return;
            }

            if (confirmYesButton != null && selected == confirmYesButton.gameObject)
            {
                ConfirmExitYes();
                return;
            }
        }

        pendingExitAction = PendingExitAction.ExitWithoutSaving;
        ShowConfirmation("QUIERES SALIR SIN GUARDAR?");
    }

    public void ConfirmExitYes()
    {
        if (isExitingToMenu)
        {
            return;
        }

        if (pendingExitAction == PendingExitAction.SaveAndExit)
        {
            DoSaveAndExit();
        }
        else if (pendingExitAction == PendingExitAction.ExitWithoutSaving)
        {
            DoExitWithoutSaving();
        }
        else
        {
            ShowPauseButtons();
        }
    }

    public void ConfirmExitNo()
    {
        pendingExitAction = PendingExitAction.None;
        ShowPauseButtons();
    }

    public void OpenSettingsMenu()
    {
        if (settingsUiElements.Count == 0)
        {
            return;
        }

        pendingExitAction = PendingExitAction.None;
        SetActiveIfAssigned(resumeButton, false);
        SetActiveIfAssigned(saveAndExitButton, false);
        SetActiveIfAssigned(exitWithoutSaveButton, false);
        SetActiveIfAssigned(settingsButton, false);
        SetActiveIfAssigned(confirmationTextObject, false);
        SetActiveIfAssigned(confirmYesButton, false);
        SetActiveIfAssigned(confirmNoButton, false);
        SetSettingsElementsActive(true);
    }

    public void CloseSettingsMenu()
    {
        SetSettingsElementsActive(false);
        ShowPauseButtons();
    }

    public void QuitGame()
    {
        ExitWithoutSaving();
    }

    void Pause()
    {
        if (pauseMenuUI != null)
        {
            pauseMenuUI.SetActive(true);
        }

        Time.timeScale = 0f;
        isPaused = true;
        pendingExitAction = PendingExitAction.None;
        ShowPauseButtons();

        if (settingsMenuUI != null)
        {
            settingsMenuUI.SetActive(false);
        }
        SetSettingsElementsActive(false);

        Cursor.visible = true;
        Cursor.lockState = CursorLockMode.None;

        if (movementScript != null)
        {
            movementScript.enabled = false;
            movementScript.canLook = false;
        }
    }

    void DoSaveAndExit()
    {
        if (GameManager.Instance != null)
        {
            SaveSystem.SaveGame(GameManager.Instance.BuildSaveData());
        }

        ExitToMainMenu("Guardando partida y volviendo al menu principal...");
    }

    void DoExitWithoutSaving()
    {
        ExitToMainMenu("Volviendo al menu principal sin guardar...");
    }

    void ExitToMainMenu(string logMessage)
    {
        isExitingToMenu = true;
        Time.timeScale = 1f;
        isPaused = false;
        pendingExitAction = PendingExitAction.None;

        if (pauseMenuUI != null)
        {
            pauseMenuUI.SetActive(false);
        }

        if (settingsMenuUI != null)
        {
            settingsMenuUI.SetActive(false);
        }
        SetSettingsElementsActive(false);

        Cursor.visible = true;
        Cursor.lockState = CursorLockMode.None;
        Debug.Log(logMessage);
        SceneManager.LoadScene("MainMenu");
    }

    void ShowConfirmation(string message)
    {
        SetActiveIfAssigned(resumeButton, false);
        SetActiveIfAssigned(saveAndExitButton, false);
        SetActiveIfAssigned(exitWithoutSaveButton, false);
        SetActiveIfAssigned(settingsButton, false);
        SetActiveIfAssigned(confirmationTextObject, true);
        SetActiveIfAssigned(confirmYesButton, true);
        SetActiveIfAssigned(confirmNoButton, true);

        if (settingsMenuUI != null)
        {
            settingsMenuUI.SetActive(false);
        }
        SetSettingsElementsActive(false);

        if (confirmationTextLabel != null)
        {
            confirmationTextLabel.text = message;
        }
    }

    void ShowPauseButtons()
    {
        SetActiveIfAssigned(resumeButton, true);
        SetActiveIfAssigned(saveAndExitButton, true);
        SetActiveIfAssigned(exitWithoutSaveButton, true);
        SetActiveIfAssigned(settingsButton, true);
        SetActiveIfAssigned(confirmationTextObject, false);
        SetActiveIfAssigned(confirmYesButton, false);
        SetActiveIfAssigned(confirmNoButton, false);
    }

    void LoadAndApplyPlayerSettings()
    {
        playerSettings = PlayerSettingsSystem.Load();
        if (playerSettings == null)
        {
            playerSettings = new PlayerSettingsData();
        }

        playerSettings.masterVolume = Mathf.Clamp01(playerSettings.masterVolume);
        playerSettings.brightness = Mathf.Clamp01(playerSettings.brightness);
        playerSettings.resolutionIndex = Mathf.Clamp(playerSettings.resolutionIndex, 0, commonResolutions.Length - 1);

        ConfigureResolutionDropdown();

        isApplyingUIState = true;
        if (volumeSlider != null)
        {
            volumeSlider.value = playerSettings.masterVolume;
        }

        if (brightnessSlider != null)
        {
            brightnessSlider.value = playerSettings.brightness;
        }

        if (fullscreenToggle != null)
        {
            fullscreenToggle.isOn = playerSettings.fullscreen;
        }

        if (resolutionDropdown != null)
        {
            resolutionDropdown.value = playerSettings.resolutionIndex;
            resolutionDropdown.RefreshShownValue();
        }
        isApplyingUIState = false;

        ApplySettings(save: false);

        SetSettingsElementsActive(false);
    }

    void ConfigureResolutionDropdown()
    {
        if (resolutionDropdown == null)
        {
            return;
        }

        List<string> options = new List<string>();
        for (int i = 0; i < commonResolutions.Length; i++)
        {
            Vector2Int res = commonResolutions[i];
            options.Add($"{res.x} x {res.y}");
        }

        resolutionDropdown.ClearOptions();
        resolutionDropdown.AddOptions(options);
        ConfigureResolutionDropdownScroll();
    }

    void ConfigureResolutionDropdownScroll()
    {
        if (resolutionDropdown == null || resolutionDropdown.template == null)
        {
            return;
        }

        RectTransform template = resolutionDropdown.template;
        ScrollRect scrollRect = template.GetComponentInChildren<ScrollRect>(true);
        if (scrollRect == null || scrollRect.content == null || scrollRect.viewport == null)
        {
            return;
        }

        if (scrollRect.verticalScrollbar == null)
        {
            scrollRect.verticalScrollbar = template.GetComponentInChildren<Scrollbar>(true);
        }

        if (scrollRect.verticalScrollbar != null)
        {
            Scrollbar scrollbar = scrollRect.verticalScrollbar;
            scrollbar.interactable = true;
            scrollbar.gameObject.SetActive(true);
            scrollbar.direction = Scrollbar.Direction.BottomToTop;

            RectTransform scrollbarRect = scrollbar.transform as RectTransform;
            if (scrollbarRect != null)
            {
                scrollbarRect.anchorMin = new Vector2(1f, 0f);
                scrollbarRect.anchorMax = new Vector2(1f, 1f);
                scrollbarRect.pivot = new Vector2(1f, 0.5f);
                scrollbarRect.sizeDelta = new Vector2(10f, scrollbarRect.sizeDelta.y);
                scrollbarRect.anchoredPosition = new Vector2(-4f, 0f);
            }

            Image scrollbarBackground = scrollbar.GetComponent<Image>();
            if (scrollbarBackground != null)
            {
                scrollbarBackground.color = new Color(0.05f, 0.07f, 0.22f, 0.75f);
            }

            if (scrollbar.targetGraphic != null)
            {
                scrollbar.targetGraphic.color = new Color(0.82f, 0.88f, 1f, 0.96f);
            }
        }
        scrollRect.vertical = true;
        scrollRect.verticalScrollbarVisibility = ScrollRect.ScrollbarVisibility.Permanent;

        Toggle itemToggle = scrollRect.content.GetComponentInChildren<Toggle>(true);
        float itemHeight = 28f;
        if (itemToggle != null)
        {
            RectTransform itemRect = itemToggle.transform as RectTransform;
            if (itemRect != null && itemRect.rect.height > 1f)
            {
                itemHeight = itemRect.rect.height;
            }
        }

        // Forzamos un viewport pequeño para que haya desplazamiento visible con 5 opciones.
        float visibleItems = 2f;
        float viewportHeight = itemHeight * visibleItems;
        Vector2 viewportSize = scrollRect.viewport.sizeDelta;
        scrollRect.viewport.sizeDelta = new Vector2(viewportSize.x, viewportHeight);

        Vector2 templateSize = template.sizeDelta;
        template.sizeDelta = new Vector2(templateSize.x, viewportHeight + itemHeight);
    }

    void ApplySettings(bool save)
    {
        AudioListener.volume = playerSettings.masterVolume;
        ApplyBrightnessOverlay(Mathf.Clamp01(playerSettings.brightness));

        Vector2Int targetRes = commonResolutions[playerSettings.resolutionIndex];
        FullScreenMode screenMode = playerSettings.fullscreen ? FullScreenMode.ExclusiveFullScreen : FullScreenMode.Windowed;
        Screen.SetResolution(targetRes.x, targetRes.y, screenMode);
        Screen.fullScreen = playerSettings.fullscreen;

        if (save)
        {
            PlayerSettingsSystem.Save(playerSettings);
        }
    }

    void EnsureBrightnessOverlay()
    {
        if (brightnessOverlay != null)
        {
            return;
        }

        Canvas rootCanvas = null;
        if (pauseMenuUI != null)
        {
            rootCanvas = pauseMenuUI.GetComponentInParent<Canvas>();
        }

        if (rootCanvas == null)
        {
            rootCanvas = FindFirstObjectByType<Canvas>();
        }

        if (rootCanvas == null)
        {
            return;
        }

        Transform existing = rootCanvas.transform.Find("BrightnessOverlay");
        if (existing != null)
        {
            brightnessOverlay = existing.GetComponent<Image>();
            if (brightnessOverlay != null)
            {
                brightnessOverlay.raycastTarget = false;
            }
            return;
        }

        GameObject overlayGO = new GameObject("BrightnessOverlay");
        overlayGO.transform.SetParent(rootCanvas.transform, false);
        RectTransform rt = overlayGO.AddComponent<RectTransform>();
        rt.anchorMin = Vector2.zero;
        rt.anchorMax = Vector2.one;
        rt.offsetMin = Vector2.zero;
        rt.offsetMax = Vector2.zero;

        brightnessOverlay = overlayGO.AddComponent<Image>();
        brightnessOverlay.color = new Color(0f, 0f, 0f, 0f);
        brightnessOverlay.raycastTarget = false;
        overlayGO.transform.SetAsFirstSibling();
    }

    void ApplyBrightnessOverlay(float brightnessT)
    {
        if (brightnessOverlay == null)
        {
            EnsureBrightnessOverlay();
        }

        if (brightnessOverlay == null)
        {
            return;
        }

        // Por debajo de 0.5 oscurecemos con overlay negro.
        // Por encima de 0.5 aclaramos con postExposure (sin overlay blanco grisáceo).
        float previewT = Mathf.Clamp01(brightnessT);
        float effectiveT = previewT * 0.75f;

        float postExposure = 0f;
        if (effectiveT < 0.5f)
        {
            float darkAlpha = Mathf.Lerp(0.85f, 0f, effectiveT * 2f);
            brightnessOverlay.color = new Color(0f, 0f, 0f, darkAlpha);
        }
        else
        {
            brightnessOverlay.color = new Color(0f, 0f, 0f, 0f);
            float t = (effectiveT - 0.5f) * 2f;
            float curvedT = Mathf.Pow(t, 1.8f);
            postExposure = Mathf.Lerp(0f, 7.5f, curvedT);
        }

        if (colorAdjustments != null)
        {
            colorAdjustments.postExposure.Override(postExposure);
        }

        ApplySettingsMenuBackgroundBrightness(previewT);
    }

    void CacheSettingsMenuBackground()
    {
        settingsMenuBackgroundImage = null;
        if (settingsMenuUI == null)
        {
            return;
        }

        settingsMenuBackgroundImage = settingsMenuUI.GetComponent<Image>();
        if (settingsMenuBackgroundImage != null)
        {
            settingsMenuBackgroundBaseColor = settingsMenuBackgroundImage.color;
        }
    }

    void ApplySettingsMenuBackgroundBrightness(float brightnessT)
    {
        if (settingsMenuBackgroundImage == null)
        {
            return;
        }

        Color baseColor = settingsMenuBackgroundBaseColor;
        Color result;
        if (brightnessT < 0.5f)
        {
            float darkFactor = Mathf.Lerp(0.2f, 1f, brightnessT * 2f);
            result = new Color(
                baseColor.r * darkFactor,
                baseColor.g * darkFactor,
                baseColor.b * darkFactor,
                baseColor.a);
        }
        else
        {
            float lightT = (brightnessT - 0.5f) * 2f;
            Color whiteTint = new Color(1f, 1f, 1f, baseColor.a);
            float previewCurve = Mathf.Pow(lightT, 0.8f);
            result = Color.Lerp(baseColor, whiteTint, previewCurve * 0.85f);
            result.a = baseColor.a;
        }

        settingsMenuBackgroundImage.color = result;
    }

    void CacheColorAdjustments()
    {
        colorAdjustments = null;
        Volume[] volumes = FindObjectsByType<Volume>(FindObjectsInactive.Include, FindObjectsSortMode.None);
        foreach (Volume v in volumes)
        {
            if (v == null || !v.isGlobal)
            {
                continue;
            }

            if (v.profile == null)
            {
                v.profile = ScriptableObject.CreateInstance<VolumeProfile>();
            }
            else
            {
                // Clonamos para no editar el asset original del proyecto en tiempo de juego.
                v.profile = Instantiate(v.profile);
            }

            if (!v.profile.TryGet(out ColorAdjustments found))
            {
                found = v.profile.Add<ColorAdjustments>(true);
            }

            if (found != null)
            {
                colorAdjustments = found;
                colorAdjustments.active = true;
                colorAdjustments.postExposure.Override(0f);
                return;
            }
        }
    }

    void OnVolumeChanged(float value)
    {
        if (isApplyingUIState)
        {
            return;
        }

        playerSettings.masterVolume = Mathf.Clamp01(value);
        ApplySettings(save: true);
    }

    void OnBrightnessChanged(float value)
    {
        if (isApplyingUIState)
        {
            return;
        }

        playerSettings.brightness = Mathf.Clamp01(value);
        ApplySettings(save: true);
    }

    void OnFullscreenChanged(bool value)
    {
        if (isApplyingUIState)
        {
            return;
        }

        playerSettings.fullscreen = value;
        ApplySettings(save: true);
    }

    void OnResolutionChanged(int value)
    {
        if (isApplyingUIState)
        {
            return;
        }

        playerSettings.resolutionIndex = Mathf.Clamp(value, 0, commonResolutions.Length - 1);
        ApplySettings(save: true);
    }

    void WireButtons()
    {
        if (confirmYesButton != null)
        {
            confirmYesButton.onClick.RemoveListener(ConfirmExitYes);
            confirmYesButton.onClick.AddListener(ConfirmExitYes);
        }

        if (confirmNoButton != null)
        {
            confirmNoButton.onClick.RemoveListener(ConfirmExitNo);
            confirmNoButton.onClick.AddListener(ConfirmExitNo);
        }

        if (settingsButton != null)
        {
            settingsButton.onClick.RemoveListener(OpenSettingsMenu);
            settingsButton.onClick.AddListener(OpenSettingsMenu);
        }

        if (settingsBackButton != null)
        {
            settingsBackButton.onClick.RemoveListener(CloseSettingsMenu);
            settingsBackButton.onClick.AddListener(CloseSettingsMenu);
        }

        if (volumeSlider != null)
        {
            volumeSlider.onValueChanged.RemoveListener(OnVolumeChanged);
            volumeSlider.onValueChanged.AddListener(OnVolumeChanged);
        }

        if (brightnessSlider != null)
        {
            brightnessSlider.onValueChanged.RemoveListener(OnBrightnessChanged);
            brightnessSlider.onValueChanged.AddListener(OnBrightnessChanged);
        }

        if (fullscreenToggle != null)
        {
            fullscreenToggle.onValueChanged.RemoveListener(OnFullscreenChanged);
            fullscreenToggle.onValueChanged.AddListener(OnFullscreenChanged);
        }

        if (resolutionDropdown != null)
        {
            resolutionDropdown.onValueChanged.RemoveListener(OnResolutionChanged);
            resolutionDropdown.onValueChanged.AddListener(OnResolutionChanged);
        }
    }

    void FindPauseMenuReferences()
    {
        Transform pauseRoot = pauseMenuUI != null ? pauseMenuUI.transform : null;

        if (resumeButton == null)
        {
            resumeButton = FindButtonByName("ReanudarButton", pauseRoot);
        }

        if (saveAndExitButton == null)
        {
            saveAndExitButton = FindButtonByName("SalirSaveButton", pauseRoot);
        }

        if (exitWithoutSaveButton == null)
        {
            exitWithoutSaveButton = FindButtonByName("SalirNoSaveButton (1)", pauseRoot);
        }

        if (settingsButton == null)
        {
            settingsButton = FindButtonByName("AjustesButton", pauseRoot);
            if (settingsButton == null)
            {
                settingsButton = FindButtonByText("Ajustes", pauseRoot);
            }
        }

        if (confirmYesButton == null)
        {
            confirmYesButton = FindButtonByName("Si", pauseRoot);
        }

        if (confirmNoButton == null)
        {
            confirmNoButton = FindButtonByName("No", pauseRoot);
        }

        if (confirmationTextObject == null)
        {
            confirmationTextObject = FindObjectByName("ConfirmText", pauseRoot);
        }

        if (confirmationTextLabel == null && confirmationTextObject != null)
        {
            confirmationTextLabel = confirmationTextObject.GetComponentInChildren<TextMeshProUGUI>(true);
        }
    }

    void FindSettingsReferences()
    {
        if (settingsMenuUI == null)
        {
            settingsMenuUI = FindObjectByName("MenuAjustes");
        }

        Transform settingsRoot = settingsMenuUI != null ? settingsMenuUI.transform : null;

        if (settingsBackButton == null)
        {
            settingsBackButton = FindButtonByName("AtrasButton", settingsRoot);
            if (settingsBackButton == null)
            {
                settingsBackButton = FindButtonByText("Atras", settingsRoot);
            }
            if (settingsBackButton == null)
            {
                settingsBackButton = FindButtonByText("Atr", settingsRoot);
            }
            if (settingsBackButton == null)
            {
                settingsBackButton = FindButtonByText("Volver", settingsRoot);
            }
            if (settingsBackButton == null)
            {
                settingsBackButton = FindButtonByName("ReanudarButton", settingsRoot);
            }
        }

        if (volumeSlider == null)
        {
            volumeSlider = FindSliderByName("SliderVolumen", settingsRoot);
            if (volumeSlider == null)
            {
                volumeSlider = FindSliderByName("SliderVolumen");
            }
        }

        if (brightnessSlider == null)
        {
            brightnessSlider = FindSliderByName("SliderBrillo", settingsRoot);
            if (brightnessSlider == null)
            {
                brightnessSlider = FindSliderByName("SliderBrillo");
            }
        }

        if (fullscreenToggle == null)
        {
            fullscreenToggle = FindToggleByLabel("Pantalla", settingsRoot);
            if (fullscreenToggle == null)
            {
                fullscreenToggle = FindToggleByLabel("Completa", settingsRoot);
            }
            if (fullscreenToggle == null)
            {
                fullscreenToggle = FindToggleByName("Toggle", settingsRoot);
            }
            if (fullscreenToggle == null)
            {
                fullscreenToggle = FindToggleByName("Toggle");
            }
        }

        if (resolutionDropdown == null)
        {
            resolutionDropdown = FindDropdownByName("DropdownResolucion", settingsRoot);
            if (resolutionDropdown == null)
            {
                resolutionDropdown = FindDropdownByName("DropdownResolucion");
            }
        }
    }

    void CollectSettingsUiElements()
    {
        settingsUiElements.Clear();
        AddSettingsElement(settingsMenuUI);
        AddSettingsElement(settingsBackButton);
        AddSettingsElement(volumeSlider);
        AddSettingsElement(brightnessSlider);
        AddSettingsElement(fullscreenToggle);
        AddSettingsElement(resolutionDropdown);
    }

    void AddSettingsElement(Component component)
    {
        if (component != null)
        {
            AddSettingsElement(component.gameObject);
        }
    }

    void AddSettingsElement(GameObject go)
    {
        if (go != null && !settingsUiElements.Contains(go))
        {
            settingsUiElements.Add(go);
        }
    }

    void SetSettingsElementsActive(bool active)
    {
        for (int i = 0; i < settingsUiElements.Count; i++)
        {
            if (settingsUiElements[i] != null)
            {
                settingsUiElements[i].SetActive(active);
            }
        }
    }

    void SetActiveIfAssigned(Component component, bool active)
    {
        if (component != null)
        {
            component.gameObject.SetActive(active);
        }
    }

    void SetActiveIfAssigned(GameObject target, bool active)
    {
        if (target != null)
        {
            target.SetActive(active);
        }
    }

    Button FindButtonByName(string buttonName, Transform root = null)
    {
        Button[] buttons = FindObjectsByType<Button>(FindObjectsInactive.Include, FindObjectsSortMode.None);
        foreach (Button button in buttons)
        {
            if (!IsUnderRoot(button.transform, root))
            {
                continue;
            }

            if (button.gameObject.name.Trim() == buttonName)
            {
                return button;
            }
        }

        return null;
    }

    Button FindButtonByText(string buttonTextContains, Transform root = null)
    {
        Button[] buttons = FindObjectsByType<Button>(FindObjectsInactive.Include, FindObjectsSortMode.None);
        foreach (Button button in buttons)
        {
            if (!IsUnderRoot(button.transform, root))
            {
                continue;
            }

            TextMeshProUGUI tmp = button.GetComponentInChildren<TextMeshProUGUI>(true);
            if (tmp != null && tmp.text.ToLowerInvariant().Contains(buttonTextContains.ToLowerInvariant()))
            {
                return button;
            }
        }

        return null;
    }

    Slider FindSliderByName(string sliderName, Transform root = null)
    {
        Slider[] sliders = FindObjectsByType<Slider>(FindObjectsInactive.Include, FindObjectsSortMode.None);
        foreach (Slider slider in sliders)
        {
            if (!IsUnderRoot(slider.transform, root))
            {
                continue;
            }

            if (slider.gameObject.name.Trim() == sliderName)
            {
                return slider;
            }
        }

        return null;
    }

    Toggle FindToggleByName(string toggleName, Transform root = null)
    {
        Toggle[] toggles = FindObjectsByType<Toggle>(FindObjectsInactive.Include, FindObjectsSortMode.None);
        foreach (Toggle toggle in toggles)
        {
            if (!IsUnderRoot(toggle.transform, root))
            {
                continue;
            }

            if (toggle.gameObject.name.Trim() == toggleName)
            {
                return toggle;
            }
        }

        return null;
    }

    Toggle FindToggleByLabel(string textContains, Transform root = null)
    {
        Toggle[] toggles = FindObjectsByType<Toggle>(FindObjectsInactive.Include, FindObjectsSortMode.None);
        foreach (Toggle toggle in toggles)
        {
            if (!IsUnderRoot(toggle.transform, root))
            {
                continue;
            }

            TextMeshProUGUI tmp = toggle.GetComponentInChildren<TextMeshProUGUI>(true);
            if (tmp != null && tmp.text.ToLowerInvariant().Contains(textContains.ToLowerInvariant()))
            {
                return toggle;
            }
        }

        return null;
    }

    TMP_Dropdown FindDropdownByName(string dropdownName, Transform root = null)
    {
        TMP_Dropdown[] dropdowns = FindObjectsByType<TMP_Dropdown>(FindObjectsInactive.Include, FindObjectsSortMode.None);
        foreach (TMP_Dropdown dropdown in dropdowns)
        {
            if (!IsUnderRoot(dropdown.transform, root))
            {
                continue;
            }

            if (dropdown.gameObject.name.Trim() == dropdownName)
            {
                return dropdown;
            }
        }

        return null;
    }

    GameObject FindObjectByName(string objectName, Transform root = null)
    {
        Transform[] allTransforms = FindObjectsByType<Transform>(FindObjectsInactive.Include, FindObjectsSortMode.None);
        foreach (Transform t in allTransforms)
        {
            if (!IsUnderRoot(t, root))
            {
                continue;
            }

            if (t.gameObject.name.Trim() == objectName)
            {
                return t.gameObject;
            }
        }

        return null;
    }

    bool IsUnderRoot(Transform target, Transform root)
    {
        if (root == null)
        {
            return true;
        }

        return target == root || target.IsChildOf(root);
    }
}
