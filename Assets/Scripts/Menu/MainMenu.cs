using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;
using UnityEngine.SceneManagement;
using UnityEngine.EventSystems;
using UnityEngine.UI;


/*
* Script para manejar el menu principal.
* 
* Metodos:
*   - OpenMainMenuPanel(): Metodo que abre el menu principal.
*   - OpenSettingsMenu(): Metodo que abre el menu de configuraciones.
*   - BackFromSettings(): Metodo que cierra el menu de configuraciones.
*   - QuitGame(): Metodo que cierra el juego.
*   - PlayGame(): Metodo que inicia el juego.
*   - NewGame(): Metodo que inicia una nueva partida.
*   - ConfirmNewGame(): Metodo que confirma la nueva partida.
*   - CancelNewGame(): Metodo que cancela la nueva partida.
*   - LoadAndApplyPlayerSettings(): Metodo que carga la configuracion del jugador.
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
*   - mainMenu: Panel que muestra el menu principal.
*   - playButton: Boton de jugar.
*   - newGameButton: Boton de nueva partida.
*   - settingsButton: Boton de configuraciones.
*   - exitButton: Boton de salir.
*   - newGameConfirmationText: Texto de confirmacion de nueva partida.
*   - confirmYesButton: Boton de confirmacion de nueva partida.
*   - confirmNoButton: Boton de cancelacion de nueva partida.
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
*
*   Funcionamiento:
*   - Al iniciar el juego, se muestra el menu principal.
*   - El jugador puede iniciar una nueva partida, continuar la partida guardada, abrir el menu de configuraciones o salir del juego.
*   - El menu de configuraciones permite ajustar el volumen, brillo, pantalla completa y resolucion.
*   - El jugador puede guardar la partida en cualquier momento.
*
*   Flujo:
*   1. El jugador inicia el juego.
*   2. Se muestra el menu principal.
*   3. El jugador puede iniciar una nueva partida, continuar la partida guardada, abrir el menu de configuraciones o salir del juego.
*   4. El jugador puede guardar la partida en cualquier momento.
*
*   Hay mucho codigo quer se repite, solo estan comentados algunos metodos.
*
*/


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

    //Esto hace que el juego se vea bien en diferentes resoluciones.
    private readonly Vector2Int[] commonResolutions =
    {
        new Vector2Int(1920, 1080),
        new Vector2Int(1600, 900),
        new Vector2Int(1366, 768),
        new Vector2Int(1280, 720)
    };

    /*
    * Metodo que se ejecuta al iniciar el juego.
    */

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

    /*
    * Metodo que se ejecuta al iniciar el juego.
    */
    void Start()
    {
        ShowMainOptions();
    }

    /*
    * Metodo que abre el menu principal.
    */
    public void OpenMainMenuPanel()
    {
        if (mainMenu != null)
        {
            mainMenu.SetActive(true);
        }
        ShowMainOptions();
    }

    /*
    * Metodo que abre el menu de configuraciones.
    */
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

    /*
    * Metodo que regresa al menu principal.
    */
    public void BackFromSettings()
    {
        ShowMainOptions();
    }

    /*
    * Metodo que cierra el juego.
    */
    public void QuitGame()
    {
        Application.Quit();
    }

    /*
    * Metodo que inicia el juego.
    */
    public void PlayGame()
    {
        //Obtiene el objeto seleccionado.
        GameObject selected = EventSystem.current != null ? EventSystem.current.currentSelectedGameObject : null;
        //Si el boton de jugar no esta seleccionado, no se puede iniciar el juego.
        if (playButton != null && selected != null && selected != playButton.gameObject)
        {
            return;
        }

        //Si no hay partida guardada, no se puede iniciar el juego.
        if (!SaveSystem.HasSave())
        {
            Debug.Log("No hay partida guardada para continuar.");
            RefreshButtons();
            return;
        }

        //Carga la partida guardada.
        SaveData saveData = SaveSystem.LoadGame();
        //Si no se pudo cargar la partida guardada, se carga la escena por defecto.
        if (saveData == null)
        {
            Debug.LogWarning("No se pudo leer la partida guardada. Cargando SampleScene por defecto.");
            SceneManager.LoadScene("SampleScene");
            return;
        }

        //Resuelve la escena para la partida guardada.
        string targetScene = ResolveSceneForSavedGame(saveData);
        //Carga la escena para la partida guardada.
        SceneManager.LoadScene(targetScene);
    }

    /*
    * Metodo que inicia una nueva partida.
    */
    public void NewGame()
    {
        ShowNewGameConfirmation();
    }

    /*
    * Metodo que confirma la nueva partida.
    */
    public void ConfirmNewGame()
    {
        SaveSystem.DeleteSave();
        ShowMainOptions();
        SceneManager.LoadScene("HouseScenePrologue");
    }

    /*
    * Metodo que cancela la nueva partida.
    */
    public void CancelNewGame()
    {
        ShowMainOptions();
    }

    /*
    * Metodo que carga y aplica la configuracion del jugador.
    */
    void LoadAndApplyPlayerSettings()
    {
        playerSettings = PlayerSettingsSystem.Load();
        //Si no hay configuracion del jugador, se crea una nueva.
        if (playerSettings == null)
        {
            playerSettings = new PlayerSettingsData();
        }

        //Clampa el volumen y el brillo.
        //clampar es para que el valor no se salga de un rango.
        playerSettings.masterVolume = Mathf.Clamp01(playerSettings.masterVolume);
        playerSettings.brightness = Mathf.Clamp01(playerSettings.brightness);
        playerSettings.resolutionIndex = Mathf.Clamp(playerSettings.resolutionIndex, 0, commonResolutions.Length - 1);

        //Configura el dropdown de resolucion.
        // El dropdown es una lista desplegable.
        ConfigureResolutionDropdown();

        isApplyingUIState = true;
        //Si el slider de volumen no es nulo, se establece el valor del slider.
        if (volumeSlider != null)
        {
            volumeSlider.value = playerSettings.masterVolume;
        }

        //Si el slider de brillo no es nulo, se establece el valor del slider.
        if (brightnessSlider != null)
        {
            brightnessSlider.value = playerSettings.brightness;
        }

        //Si el toggle de pantalla completa no es nulo, se establece el valor del toggle.
        if (fullscreenToggle != null)
        {
            fullscreenToggle.isOn = playerSettings.fullscreen;
        }

        //Si el dropdown de resolucion no es nulo, se establece el valor del dropdown.
        if (resolutionDropdown != null)
        {
            resolutionDropdown.value = playerSettings.resolutionIndex;
            resolutionDropdown.RefreshShownValue();
        }
        isApplyingUIState = false;

        ApplySettings(false);
    }

    /*
    * Metodo que configura el dropdown de resolucion.
    */
    void ConfigureResolutionDropdown()
    {
        //Si el dropdown de resolucion no es nulo, se configura.
        if (resolutionDropdown == null)
        {
            return;
        }

        List<string> options = new List<string>();
        //Recorre las resoluciones comunes.
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
        //Si el dropdown de resolucion o su plantilla no es nulo, se configura.
        if (resolutionDropdown == null || resolutionDropdown.template == null)
        {
            return;
        }

        RectTransform template = resolutionDropdown.template;
        ScrollRect scrollRect = template.GetComponentInChildren<ScrollRect>(true);
        //Si el scrollRect o su contenido o su viewport no es nulo, se configura.
        if (scrollRect == null || scrollRect.content == null || scrollRect.viewport == null)
        {
            return;
        }

        //Si el scrollRect.verticalScrollbar no es nulo, se configura.
        if (scrollRect.verticalScrollbar == null)
        {
            scrollRect.verticalScrollbar = template.GetComponentInChildren<Scrollbar>(true);
        }

        //Si el scrollRect.verticalScrollbar no es nulo, se configura.
        if (scrollRect.verticalScrollbar != null)
        {
            Scrollbar scrollbar = scrollRect.verticalScrollbar;
            scrollbar.interactable = true;
            scrollbar.gameObject.SetActive(true);
            scrollbar.direction = Scrollbar.Direction.BottomToTop;

            RectTransform scrollbarRect = scrollbar.transform as RectTransform;
            //Si el scrollbarRect no es nulo, se configura.
            if (scrollbarRect != null)
            {
                scrollbarRect.anchorMin = new Vector2(1f, 0f);
                scrollbarRect.anchorMax = new Vector2(1f, 1f);
                scrollbarRect.pivot = new Vector2(1f, 0.5f);
                scrollbarRect.sizeDelta = new Vector2(10f, scrollbarRect.sizeDelta.y);
                scrollbarRect.anchoredPosition = new Vector2(-4f, 0f);
            }

            Image scrollbarBackground = scrollbar.GetComponent<Image>();
            //Si el scrollbarBackground no es nulo, se configura.
            if (scrollbarBackground != null)
            {
                scrollbarBackground.color = new Color(0.05f, 0.07f, 0.22f, 0.75f);
            }

            //Si el scrollbar.targetGraphic no es nulo, se configura.
            if (scrollbar.targetGraphic != null)
            {
                scrollbar.targetGraphic.color = new Color(0.82f, 0.88f, 1f, 0.96f);
            }
        }
        scrollRect.vertical = true;
        scrollRect.verticalScrollbarVisibility = ScrollRect.ScrollbarVisibility.Permanent;

        Toggle itemToggle = scrollRect.content.GetComponentInChildren<Toggle>(true);
        float itemHeight = 28f;
        //Si el itemToggle no es nulo, se configura.
        if (itemToggle != null)
        {
            RectTransform itemRect = itemToggle.transform as RectTransform;
            //Si el itemRect no es nulo y su altura es mayor a 1, se establece la altura del item.
            if (itemRect != null && itemRect.rect.height > 1f)
            {
                itemHeight = itemRect.rect.height;
            }
        }

        //Configura el viewport.
        float visibleItems = 2f;
        float viewportHeight = itemHeight * visibleItems;
        Vector2 viewportSize = scrollRect.viewport.sizeDelta;
        scrollRect.viewport.sizeDelta = new Vector2(viewportSize.x, viewportHeight);
        //Configura el template.
        Vector2 templateSize = template.sizeDelta;
        template.sizeDelta = new Vector2(templateSize.x, viewportHeight + itemHeight);
    }

    /*
    * Metodo que aplica la configuracion.
    */
    void ApplySettings(bool save)
    {
        AudioListener.volume = playerSettings.masterVolume;
        ApplyBrightness(Mathf.Clamp01(playerSettings.brightness));

        Vector2Int targetRes = commonResolutions[playerSettings.resolutionIndex];
        FullScreenMode screenMode = playerSettings.fullscreen ? FullScreenMode.ExclusiveFullScreen : FullScreenMode.Windowed;
        Screen.SetResolution(targetRes.x, targetRes.y, screenMode);
        Screen.fullScreen = playerSettings.fullscreen;

        //Si se debe guardar, se guarda.
        if (save)
        {
            PlayerSettingsSystem.Save(playerSettings);
        }
    }

    /*
    * Metodo que aplica el brillo.
    */
    void ApplyBrightness(float brightnessT)
    {
        //Si el brilloOverlay no es nulo, se asegura de que exista.
        if (brightnessOverlay == null)
        {
            EnsureBrightnessOverlay();
        }

        float previewT = Mathf.Clamp01(brightnessT);
        float effectiveT = previewT * 0.75f;

        float postExposure = 0f;
        //Si el brilloOverlay no es nulo, se aplica el brillo.
        if (brightnessOverlay != null)
        {
            //Si el brillo es menor a 0.5, se aplica un brillo oscuro.
            if (effectiveT < 0.5f)
            {
                float darkAlpha = Mathf.Lerp(0.85f, 0f, effectiveT * 2f);
                brightnessOverlay.color = new Color(0f, 0f, 0f, darkAlpha);
            }
            //Si el brillo es mayor a 0.5, se aplica un brillo brillante.
            else
            {
                brightnessOverlay.color = new Color(0f, 0f, 0f, 0f);
                float t = (effectiveT - 0.5f) * 2f;
                float curvedT = Mathf.Pow(t, 1.8f);
                postExposure = Mathf.Lerp(0f, 7.5f, curvedT);
            }
        }

        //Si el colorAdjustments no es nulo, se aplica el brillo.
        if (colorAdjustments != null)
        {
            colorAdjustments.postExposure.Override(postExposure);
        }

        ApplySettingsMenuBackgroundBrightness(previewT);
    }

    /*
    * Metodo que asegura que el brilloOverlay exista.
    */
    void EnsureBrightnessOverlay()
    {
        Canvas rootCanvas = null;
        //Si el mainMenu no es nulo, se busca el canvas padre.
        if (mainMenu != null)
        {
            rootCanvas = mainMenu.GetComponentInParent<Canvas>();
        }

        //Si el rootCanvas no es nulo, se busca el canvas padre.
        if (rootCanvas == null)
        {
            rootCanvas = FindFirstObjectByType<Canvas>();
        }

        //Lo mismo que el de arriba.
        if (rootCanvas == null)
        {
            return;
        }

        Transform existing = rootCanvas.transform.Find("BrightnessOverlay");
        //Si el existing no es nulo, se configura.
        if (existing != null)
        {
            brightnessOverlay = existing.GetComponent<Image>();
            //Si el brilloOverlay no es nulo, se configura.
            if (brightnessOverlay != null)
            {
                brightnessOverlay.raycastTarget = false;
            }
            return;
        }

        //Crea el brilloOverlay.
        GameObject overlayGO = new GameObject("BrightnessOverlay");
        overlayGO.transform.SetParent(rootCanvas.transform, false);
        RectTransform rt = overlayGO.AddComponent<RectTransform>();
        rt.anchorMin = Vector2.zero;
        rt.anchorMax = Vector2.one;
        rt.offsetMin = Vector2.zero;
        rt.offsetMax = Vector2.zero;

        brightnessOverlay = overlayGO.AddComponent<Image>();
        // Cuatro colores: negro, blanco, rojo y azul. (No se usan)
        // Se usan para debug.
        brightnessOverlay.color = new Color(0f, 0f, 0f, 0f);
        brightnessOverlay.raycastTarget = false;
        overlayGO.transform.SetAsFirstSibling();
    }

    /*
    * Metodo que cachea el colorAdjustments.
    */
    void CacheColorAdjustments()
    {
        colorAdjustments = null;
        Volume[] volumes = FindObjectsByType<Volume>(FindObjectsInactive.Include, FindObjectsSortMode.None);
        //Recorre todos los volúmenes.
        foreach (Volume v in volumes)
        {
            //Si el volumen es nulo o no es global, se salta.
            if (v == null || !v.isGlobal)
            {
                continue;
            }

            //Si el perfil es nulo, se crea uno nuevo.
            if (v.profile == null)
            {
                v.profile = ScriptableObject.CreateInstance<VolumeProfile>();
            }
            //Si el perfil no es nulo, se instancia.
            else
            {
                v.profile = Instantiate(v.profile);
            }

            //Si el colorAdjustments no es nulo, se configura.
            if (!v.profile.TryGet(out ColorAdjustments found))
            {
                found = v.profile.Add<ColorAdjustments>(true);
            }

            //Si el colorAdjustments no es nulo, se configura.
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
    * Metodo que cachea el settingsMenuBackgroundImage.
    */
    void CacheSettingsMenuBackground()
    {
        settingsMenuBackgroundImage = null;
        //Si el settingsMenuUI no es nulo, se busca el background.
        if (settingsMenuUI == null)
        {
            return;
        }

        settingsMenuBackgroundImage = settingsMenuUI.GetComponent<Image>();
        //Si el settingsMenuBackgroundImage no es nulo, se configura.
        if (settingsMenuBackgroundImage != null)
        {
            settingsMenuBackgroundBaseColor = settingsMenuBackgroundImage.color;
        }
    }

    /*
    * Metodo que aplica el brillo al background del menu de configuracion.
    */
    void ApplySettingsMenuBackgroundBrightness(float brightnessT)
    {
        //Si el settingsMenuBackgroundImage no es nulo, se aplica el brillo.
        if (settingsMenuBackgroundImage == null)
        {
            return;
        }

        Color baseColor = settingsMenuBackgroundBaseColor;
        Color result;
        //Si el brillo es menor a 0.5, se aplica un brillo oscuro.
        if (brightnessT < 0.5f)
        {
            float darkFactor = Mathf.Lerp(0.2f, 1f, brightnessT * 2f);
            result = new Color(baseColor.r * darkFactor, baseColor.g * darkFactor, baseColor.b * darkFactor, baseColor.a);
        }
        //Si el brillo es mayor a 0.5, se aplica un brillo brillante.
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

    /*
    * Metodo que se llama cuando cambia el volumen.
    */
    void OnVolumeChanged(float value)
    {
        //Si se esta aplicando el estado de la UI, no se hace nada.
        if (isApplyingUIState)
        {
            return;
        }

        playerSettings.masterVolume = Mathf.Clamp01(value);
        ApplySettings(true);
    }

    /*
    * Metodo que se llama cuando cambia el brillo.
    */
    void OnBrightnessChanged(float value)
    {
        //Si se esta aplicando el estado de la UI, no se hace nada.
        if (isApplyingUIState)
        {
            return;
        }

        playerSettings.brightness = Mathf.Clamp01(value);
        ApplySettings(true);
    }

    /*
    * Metodo que se llama cuando cambia el modo de pantalla completa.
    */
    void OnFullscreenChanged(bool value)
    {
        //Si se esta aplicando el estado de la UI, no se hace nada.
        if (isApplyingUIState)
        {
            return;
        }

        playerSettings.fullscreen = value;
        ApplySettings(true);
    }

    /*
    * Metodo que se llama cuando cambia la resolucion.
    */
    void OnResolutionChanged(int value)
    {
        //Si se esta aplicando el estado de la UI, no se hace nada.
        if (isApplyingUIState)
        {
            return;
        }

        playerSettings.resolutionIndex = Mathf.Clamp(value, 0, commonResolutions.Length - 1);
        ApplySettings(true);
    }

    /*
    * Metodo que refresca los botones.
    */
    void RefreshButtons()
    {
        //Si el playButton no es nulo, se actualiza su interactividad.
        if (playButton != null)
        {
            playButton.interactable = SaveSystem.HasSave();
        }
    }

    /*
    * Metodo que muestra la confirmacion de nuevo juego.
    */
    void ShowNewGameConfirmation()
    {
        //Desactiva los botones principales.
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

    /*
    * Metodo que muestra las opciones principales.
    */
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

    /*
    * Metodo que conecta los botones del menu.
    */
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

    /*
    * Metodo que busca las referencias del menu.
    */
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
        //Busca el texto de confirmacion de nuevo juego.
        if (newGameConfirmationText == null)
        {
            TextMeshProUGUI[] texts = FindObjectsByType<TextMeshProUGUI>(FindObjectsInactive.Include, FindObjectsSortMode.None);
            foreach (TextMeshProUGUI text in texts)
            {
                //Si el texto contiene "SEGURO", se asigna a newGameConfirmationText.
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

        if (!saveData.hasReadPrologueLetter && Application.CanStreamedLevelBeLoaded("HouseScenePrologue"))
        {
            return "HouseScenePrologue";
        }

        return "SampleScene";
    }
}
