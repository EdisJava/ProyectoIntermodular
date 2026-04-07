using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class MainMenu : MonoBehaviour
{
    public GameObject mainMenu;
    public Button playButton;
    public Button newGameButton;
    public Button settingsButton;
    public Button exitButton;
    public GameObject newGameConfirmationText;
    public Button confirmYesButton;
    public Button confirmNoButton;
    public GameObject settingsMenuUI;
    public Button settingsBackButton;

    public Slider volumeSlider;
    public Slider brightnessSlider;
    public Toggle fullscreenToggle;
    public TMP_Dropdown resolutionDropdown;
    public Image brightnessOverlay;

    private PlayerSettingsData playerSettings;
    private bool isApplyingUIState = false;
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

    void Awake()
    {
        FindMenuReferences();
        FindSettingsReferences();
        WireMenuButtons();
        EnsureBrightnessOverlay();
        CacheColorAdjustments();
        CacheSettingsMenuBackground();
        LoadAndApplyPlayerSettings();
    }

    void Start()
    {
        ShowMainOptions();
    }

    public void OpenMainMenuPanel()
    {
        if (mainMenu != null)
        {
            mainMenu.SetActive(true);
        }
        ShowMainOptions();
    }

    public void OpenSettingsMenu()
    {
        SetActiveIfAssigned(playButton, false);
        SetActiveIfAssigned(newGameButton, false);
        SetActiveIfAssigned(settingsButton, false);
        SetActiveIfAssigned(exitButton, false);
        SetActiveIfAssigned(newGameConfirmationText, false);
        SetActiveIfAssigned(confirmYesButton, false);
        SetActiveIfAssigned(confirmNoButton, false);
        SetActiveIfAssigned(settingsMenuUI, true);
        SetActiveIfAssigned(settingsBackButton, true);
    }

    public void BackFromSettings()
    {
        ShowMainOptions();
    }

    public void QuitGame()
    {
        Application.Quit();
    }

    public void PlayGame()
    {
        if (!SaveSystem.HasSave())
        {
            Debug.Log("No hay partida guardada para continuar.");
            RefreshButtons();
            return;
        }

        SaveData saveData = SaveSystem.LoadGame();
        if (saveData == null)
        {
            Debug.LogWarning("No se pudo leer la partida guardada. Cargando SampleScene por defecto.");
            SceneManager.LoadScene("SampleScene");
            return;
        }

        string targetScene = ResolveSceneForSavedGame(saveData);
        SceneManager.LoadScene(targetScene);
    }

    public void NewGame()
    {
        ShowNewGameConfirmation();
    }

    public void ConfirmNewGame()
    {
        SaveSystem.DeleteSave();
        ShowMainOptions();
        SceneManager.LoadScene("HouseScene");
    }

    public void CancelNewGame()
    {
        ShowMainOptions();
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

        ApplySettings(false);
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
        ApplyBrightness(Mathf.Clamp01(playerSettings.brightness));

        Vector2Int targetRes = commonResolutions[playerSettings.resolutionIndex];
        FullScreenMode screenMode = playerSettings.fullscreen ? FullScreenMode.ExclusiveFullScreen : FullScreenMode.Windowed;
        Screen.SetResolution(targetRes.x, targetRes.y, screenMode);
        Screen.fullScreen = playerSettings.fullscreen;

        if (save)
        {
            PlayerSettingsSystem.Save(playerSettings);
        }
    }

    void ApplyBrightness(float brightnessT)
    {
        if (brightnessOverlay == null)
        {
            EnsureBrightnessOverlay();
        }

        float previewT = Mathf.Clamp01(brightnessT);
        float effectiveT = previewT * 0.75f;

        float postExposure = 0f;
        if (brightnessOverlay != null)
        {
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
        }

        if (colorAdjustments != null)
        {
            colorAdjustments.postExposure.Override(postExposure);
        }

        ApplySettingsMenuBackgroundBrightness(previewT);
    }

    void EnsureBrightnessOverlay()
    {
        Canvas rootCanvas = null;
        if (mainMenu != null)
        {
            rootCanvas = mainMenu.GetComponentInParent<Canvas>();
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
            result = new Color(baseColor.r * darkFactor, baseColor.g * darkFactor, baseColor.b * darkFactor, baseColor.a);
        }
        else
        {
            float lightT = (brightnessT - 0.5f) * 2f;
            float previewCurve = Mathf.Pow(lightT, 0.8f);
            Color whiteTint = new Color(1f, 1f, 1f, baseColor.a);
            result = Color.Lerp(baseColor, whiteTint, previewCurve * 0.85f);
            result.a = baseColor.a;
        }

        settingsMenuBackgroundImage.color = result;
    }

    void OnVolumeChanged(float value)
    {
        if (isApplyingUIState)
        {
            return;
        }

        playerSettings.masterVolume = Mathf.Clamp01(value);
        ApplySettings(true);
    }

    void OnBrightnessChanged(float value)
    {
        if (isApplyingUIState)
        {
            return;
        }

        playerSettings.brightness = Mathf.Clamp01(value);
        ApplySettings(true);
    }

    void OnFullscreenChanged(bool value)
    {
        if (isApplyingUIState)
        {
            return;
        }

        playerSettings.fullscreen = value;
        ApplySettings(true);
    }

    void OnResolutionChanged(int value)
    {
        if (isApplyingUIState)
        {
            return;
        }

        playerSettings.resolutionIndex = Mathf.Clamp(value, 0, commonResolutions.Length - 1);
        ApplySettings(true);
    }

    void RefreshButtons()
    {
        if (playButton != null)
        {
            playButton.interactable = SaveSystem.HasSave();
        }
    }

    void ShowNewGameConfirmation()
    {
        SetActiveIfAssigned(playButton, false);
        SetActiveIfAssigned(newGameButton, false);
        SetActiveIfAssigned(settingsButton, false);
        SetActiveIfAssigned(exitButton, false);
        SetActiveIfAssigned(settingsMenuUI, false);
        SetActiveIfAssigned(settingsBackButton, false);
        SetActiveIfAssigned(newGameConfirmationText, true);
        SetActiveIfAssigned(confirmYesButton, true);
        SetActiveIfAssigned(confirmNoButton, true);
        RefreshButtons();
    }

    void ShowMainOptions()
    {
        SetActiveIfAssigned(playButton, true);
        SetActiveIfAssigned(newGameButton, true);
        SetActiveIfAssigned(settingsButton, true);
        SetActiveIfAssigned(exitButton, true);
        SetActiveIfAssigned(settingsMenuUI, false);
        SetActiveIfAssigned(settingsBackButton, false);
        SetActiveIfAssigned(newGameConfirmationText, false);
        SetActiveIfAssigned(confirmYesButton, false);
        SetActiveIfAssigned(confirmNoButton, false);
        RefreshButtons();
    }

    void WireMenuButtons()
    {
        if (settingsButton != null)
        {
            settingsButton.onClick.RemoveListener(OpenSettingsMenu);
            settingsButton.onClick.AddListener(OpenSettingsMenu);
        }

        if (settingsBackButton != null)
        {
            settingsBackButton.onClick.RemoveListener(BackFromSettings);
            settingsBackButton.onClick.AddListener(BackFromSettings);
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

    void FindMenuReferences()
    {
        if (playButton == null)
        {
            playButton = FindButtonByName("JugarButton");
            if (playButton == null)
            {
                playButton = FindButtonByName("JugarButton (1)");
            }
        }

        if (newGameButton == null)
        {
            newGameButton = FindButtonByName("NuevaPartidaButton");
        }

        if (settingsButton == null)
        {
            settingsButton = FindButtonByName("AjustesButton");
        }

        if (exitButton == null)
        {
            exitButton = FindButtonByName("SalirButton");
        }

        if (settingsMenuUI == null)
        {
            settingsMenuUI = FindObjectByName("MenuAjustes");
        }

        if (settingsBackButton == null)
        {
            settingsBackButton = FindButtonByName("AtrasButton");
        }

        if (newGameConfirmationText == null)
        {
            TextMeshProUGUI[] texts = FindObjectsByType<TextMeshProUGUI>(FindObjectsInactive.Include, FindObjectsSortMode.None);
            foreach (TextMeshProUGUI text in texts)
            {
                if (text.text.Contains("SEGURO"))
                {
                    newGameConfirmationText = text.gameObject;
                    break;
                }
            }
        }

        if (confirmYesButton == null)
        {
            confirmYesButton = FindButtonByName("SiButton");
        }

        if (confirmNoButton == null)
        {
            confirmNoButton = FindButtonByName("noButton");
        }
    }

    void FindSettingsReferences()
    {
        Transform settingsRoot = settingsMenuUI != null ? settingsMenuUI.transform : null;

        if (volumeSlider == null)
        {
            volumeSlider = FindSliderByName("SliderVolumen", settingsRoot);
        }

        if (brightnessSlider == null)
        {
            brightnessSlider = FindSliderByName("SliderBrillo", settingsRoot);
        }

        if (fullscreenToggle == null)
        {
            fullscreenToggle = FindToggleByName("ToggleFullScreen", settingsRoot);
            if (fullscreenToggle == null)
            {
                fullscreenToggle = FindToggleByLabel("Pantalla", settingsRoot);
            }
        }

        if (resolutionDropdown == null)
        {
            resolutionDropdown = FindDropdownByName("DropdownResolucion", settingsRoot);
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

    Button FindButtonByName(string buttonName)
    {
        Button[] buttons = FindObjectsByType<Button>(FindObjectsInactive.Include, FindObjectsSortMode.None);
        foreach (Button button in buttons)
        {
            if (button.gameObject.name.Trim() == buttonName)
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

    GameObject FindObjectByName(string objectName)
    {
        Transform[] allTransforms = FindObjectsByType<Transform>(FindObjectsInactive.Include, FindObjectsSortMode.None);
        foreach (Transform t in allTransforms)
        {
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

    string ResolveSceneForSavedGame(SaveData saveData)
    {
        if (!string.IsNullOrWhiteSpace(saveData.sceneName) && Application.CanStreamedLevelBeLoaded(saveData.sceneName))
        {
            return saveData.sceneName;
        }

        // Compatibilidad con partidas antiguas que no guardaban escena.
        if (!saveData.hasReadPrologueLetter && Application.CanStreamedLevelBeLoaded("HouseScene"))
        {
            return "HouseScene";
        }

        return "SampleScene";
    }
}
