using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;

/*
* Script para manjear el menu de pausa
* Metodos:
*   - Resume(): Metodo que reanuda el juego.
*   - SaveAndExit(): Metodo que guarda y sale del juego.
*   - ExitWithoutSaving(): Metodo que sale del juego sin guardar.
*   - ConfirmExitYes(): Metodo que confirma la salida del juego.
*   - ConfirmExitNo(): Metodo que cancela la salida del juego.
*   - OpenSettingsMenu(): Metodo que abre el menu de configuraciones.
*   - CloseSettingsMenu(): Metodo que cierra el menu de configuraciones.
*   - QuitGame(): Metodo que sale del juego.
*   - Pause(): Metodo que pausa el juego.
*   - DoSaveAndExit(): Metodo que guarda y sale del juego.
*   - DoExitWithoutSaving(): Metodo que sale del juego sin guardar.
*   - ShowConfirmation(): Metodo que muestra la confirmacion de salida.
*   - ShowPauseButtons(): Metodo que muestra los botones del menu de pausa.
*   - SetSettingsElementsActive(): Metodo que activa los elementos del menu de configuraciones.
*   - FindPauseMenuReferences(): Metodo que encuentra las referencias del menu de pausa.
*   - FindSettingsReferences(): Metodo que encuentra las referencias del menu de configuraciones.
*   - CacheSettingsMenuBackground(): Metodo que cachea la imagen de fondo del menu de configuraciones.
*   - CollectSettingsUiElements(): Metodo que recolecta los elementos del menu de configuraciones.
*   - WireButtons(): Metodo que conecta los botones del menu de pausa.
*   - EnsureBrightnessOverlay(): Metodo que asegura que el overlay de brillo este presente.
*   - CacheColorAdjustments(): Metodo que cachea los ajustes de color.
*   - LoadAndApplyPlayerSettings(): Metodo que carga y aplica la configuracion del jugador.
*   - ConfigureResolutionDropdown(): Metodo que configura el dropdown de resolucion.
*   - ConfigureResolutionDropdownScroll(): Metodo que configura el scroll del dropdown de resolucion.
*   - ApplySettings(): Metodo que aplica la configuracion del jugador.
*   - SetActiveIfAssigned(): Metodo que activa un objeto si esta asignado.
*   - ShowMainOptions(): Metodo que muestra las opciones principales.
*   - ShowNewGameConfirmation(): Metodo que muestra la confirmacion de nueva partida.
*   - RefreshButtons(): Metodo que refresca los botones.
*   - ResolveSceneForSavedGame(): Metodo que resuelve la escena para la partida guardada.
*   - GetSceneNameForDay(): Metodo que obtiene el nombre de la escena para el dia.
*   - GetSceneNameForDay(): Metodo que obtiene el nombre de la escena para el dia.
*
*   Variables:
*   - pauseMenuUI: Panel que muestra el menu de pausa.
*   - movementScript: Script de movimiento del jugador.
*   - resumeButton: Boton de reanudar.
*   - saveAndExitButton: Boton de guardar y salir.
*   - exitWithoutSaveButton: Boton de salir sin guardar.
*   - settingsButton: Boton de configuraciones.
*   - confirmationTextObject: Objeto que muestra el texto de confirmacion.
*   - confirmationTextLabel: Etiqueta que muestra el texto de confirmacion.
*   - confirmYesButton: Boton de confirmacion de salida.
*   - confirmNoButton: Boton de cancelacion de salida.
*   - settingsMenuUI: Panel que muestra el menu de configuraciones.
*   - settingsBackButton: Boton de regreso del menu de configuraciones.
*   - volumeSlider: Slider de volumen.
*   - brightnessSlider: Slider de brillo.
*   - fullscreenToggle: Toggle de pantalla completa.
*   - resolutionDropdown: Dropdown de resolucion.
*   - brightnessOverlay: Overlay de brillo.
*   - playerSettings: Datos de configuracion del jugador.
*   - isApplyingUIState: Si se esta aplicando la configuracion del jugador.
*   - colorAdjustments: Ajustes de color.
*   - settingsMenuBackgroundImage: Imagen de fondo del menu de configuraciones.
*   - settingsMenuBackgroundBaseColor: Color base de la imagen de fondo del menu de configuraciones.
*   - commonResolutions: Resoluciones comunes.
*   - pendingExitAction: Accion pendiente de salida.
*
*   Funcionamiento:
*   - Al iniciar el juego, se muestra el menu de pausa.
*   - El jugador puede reanudar el juego, guardar y salir, salir sin guardar, abrir el menu de configuraciones o salir del juego.
*   - El menu de configuraciones permite ajustar el volumen, brillo, pantalla completa y resolucion.
*   - El jugador puede guardar la partida en cualquier momento.
*
*   Flujo:
*   1. El jugador inicia el juego.
*   2. Se muestra el menu de pausa.
*   3. El jugador puede reanudar el juego, guardar y salir, salir sin guardar, abrir el menu de configuraciones o salir del juego.
*   4. El jugador puede guardar la partida en cualquier momento.
*
*   Hay mucho codigo quer se repite, solo estan comentados algunos metodos.
*
*/

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

    /*
    * Metodo que se llama al iniciar el juego.
    */
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

    /*
    * Metodo que se llama en cada frame.
    */
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

    /*
    * Metodo que reanuda el juego.
    */
    public void Resume()
    {
        GameObject selected = EventSystem.current != null ? EventSystem.current.currentSelectedGameObject : null;
        //Si se presiona el boton de configuraciones.
        if (isPaused && settingsButton != null && selected == settingsButton.gameObject)
        {
            OpenSettingsMenu();
            return;
        }
        //Si se presiona el boton de regreso del menu de configuraciones.
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
        //Si el juego esta en modo exploracion 3D.
        if (GameManager.Instance != null && GameManager.Instance.currentState == GameState.Exploration3D)
        {
            Cursor.visible = false;
            Cursor.lockState = CursorLockMode.Locked;
        }
        //Si el script de movimiento no es nulo.
        if (movementScript != null)
        {
            movementScript.enabled = true;
            movementScript.canLook = true;
        }
    }

    /*
    * Metodo que guarda y sale del juego.
    */
    public void SaveAndExit()
    {
        pendingExitAction = PendingExitAction.SaveAndExit;
        ShowConfirmation("QUIERES GUARDAR Y SALIR?");
    }

    /*
    * Metodo que sale del juego sin guardar.
    */
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

    /*
    * Metodo que confirma la salida del juego.
    */
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

    /*
    * Metodo que cancela la salida del juego.
    */
    public void ConfirmExitNo()
    {
        pendingExitAction = PendingExitAction.None;
        ShowPauseButtons();
    }

    /*
    * Metodo que abre el menu de configuraciones.
    */
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

    /*
    * Metodo que cierra el menu de configuraciones.
    */
    public void CloseSettingsMenu()
    {
        SetSettingsElementsActive(false);
        ShowPauseButtons();
    }

    /*
    * Metodo que sale del juego.
    */
    public void QuitGame()
    {
        ExitWithoutSaving();
    }

    /*
    * Metodo que pausa el juego.
    */
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

    /*
    * Metodo que guarda y sale del juego.
    */
    void DoSaveAndExit()
    {
        if (GameManager.Instance != null)
        {
            SaveSystem.SaveGame(GameManager.Instance.BuildSaveData());
        }

        ExitToMainMenu("Guardando partida y volviendo al menu principal...");
    }

    /*
    * Metodo que sale del juego sin guardar.
    */
    void DoExitWithoutSaving()
    {
        ExitToMainMenu("Volviendo al menu principal sin guardar...");
    }

    /*
    * Metodo que sale del juego.
    */
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

    /*
    * Metodo que muestra la confirmacion de salida.
    */
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

    /*
    * Metodo que muestra los botones del menu de pausa.
    */
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

    /*
    * Metodo que carga la configuracion del jugador.
    */
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

    /*
    * Metodo que configura el dropdown de resolucion.
    */
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

    /*
    * Metodo que configura el scroll del dropdown de resolucion.
    */
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

    /*
    * Metodo que aplica la configuracion del jugador.
    */
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

    /*
    * Metodo que asegura el overlay de brillo.
    */
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

    /*
    * Metodo que aplica el overlay de brillo.
    */
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

        // Por debajo de 0.5 oscurecem con overlay negro.
        // Por encima de 0.5 aclara con postExposure (sin overlay blanco grisáceo).
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

    /*
    * Metodo que cachea el fondo del menu de configuracion.
    */
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

    /*
    * Metodo que aplica el brillo al fondo del menu de configuracion.
    */
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

    /*
    * Metodo que cachea los ajustes de color.
    */
    void CacheColorAdjustments()
    {
        colorAdjustments = null;
        Volume[] volumes = FindObjectsByType<Volume>(FindObjectsInactive.Include, FindObjectsSortMode.None);
        // Recorre todos los volumes.
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
                // Clona para no editar el asset original del proyecto en tiempo de juego.
                v.profile = Instantiate(v.profile);
            }
            // Busca si hay ColorAdjustments en el perfil.
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

    /*
    * Metodo que cambia el volumen.
    */
    void OnVolumeChanged(float value)
    {
        if (isApplyingUIState)
        {
            return;
        }

        playerSettings.masterVolume = Mathf.Clamp01(value);
        ApplySettings(save: true);
    }

    /*
    * Metodo que cambia el brillo.
    */
    void OnBrightnessChanged(float value)
    {
        if (isApplyingUIState)
        {
            return;
        }

        playerSettings.brightness = Mathf.Clamp01(value);
        ApplySettings(save: true);
    }

    /*
    * Metodo que cambia la pantalla completa.
    */
    void OnFullscreenChanged(bool value)
    {
        if (isApplyingUIState)
        {
            return;
        }

        playerSettings.fullscreen = value;
        ApplySettings(save: true);
    }

    /*
    * Metodo que cambia la resolucion.
    */
    void OnResolutionChanged(int value)
    {
        if (isApplyingUIState)
        {
            return;
        }

        playerSettings.resolutionIndex = Mathf.Clamp(value, 0, commonResolutions.Length - 1);
        ApplySettings(save: true);
    }

    /*
    * Metodo que conecta los botones.
    */
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

    /*
    * Metodo que busca las referencias del menu de pausa.
    */
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

    /*
    * Metodo que busca las referencias del menu de configuracion.
    */
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

    /*
    * Metodo que recolecta los elementos de la interfaz de usuario de configuracion.
    */
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

    /*
    * Metodo que agrega un elemento al menu de configuracion.
    */
    void AddSettingsElement(Component component)
    {
        if (component != null)
        {
            AddSettingsElement(component.gameObject);
        }
    }

    /*
    * Metodo que agrega un elemento al menu de configuracion.
    */
    void AddSettingsElement(GameObject go)
    {
        if (go != null && !settingsUiElements.Contains(go))
        {
            settingsUiElements.Add(go);
        }
    }

    /*
    * Metodo que activa o desactiva los elementos del menu de configuracion.
    */
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

    /*
    * Metodo que activa o desactiva un componente.
    */
    void SetActiveIfAssigned(Component component, bool active)
    {
        if (component != null)
        {
            component.gameObject.SetActive(active);
        }
    }

    /*
    * Metodo que activa o desactiva un objeto.
    */
    void SetActiveIfAssigned(GameObject target, bool active)
    {
        if (target != null)
        {
            target.SetActive(active);
        }
    }

    /*
    * Metodo que busca un boton por nombre.
    */
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

    /*
    * Metodo que busca un boton por texto.
    */
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

    /*
    * Metodo que busca un slider por nombre.
    */
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

    /*
    * Metodo que busca un toggle por nombre.
    */
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

    /*
    * Metodo que busca un toggle por texto.
    */
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

    /*
    * Metodo que busca un dropdown por nombre.
    */
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

    /*
    * Metodo que busca un objeto por nombre.
    */
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

    /*
    * Metodo que verifica si un objeto esta bajo un root.
    */
    bool IsUnderRoot(Transform target, Transform root)
    {
        if (root == null)
        {
            return true;
        }

        return target == root || target.IsChildOf(root);
    }
}
