using UnityEngine;
using UnityEngine.SceneManagement;

/*
* Script para manejar la musica del juego.
* 
* Metodos:
*   - EnsureInstance(): Metodo que asegura que solo haya una instancia del MusicManager.
*   - Awake(): Metodo que se llama al iniciar la escena.
*   - Start(): Metodo que se llama al iniciar la escena.
*   - OnEnable(): Metodo que se llama cuando el script se habilita.
*   - OnDisable(): Metodo que se llama cuando el script se deshabilita.
*   - Update(): Metodo que se llama cada frame.
*   - OnSceneLoaded(): Metodo que se llama cuando se carga una escena.
*   - ApplyMusicForScene(): Metodo que aplica la musica para la escena actual.
*
*   Variables:
*   - musicVolume: Volumen de la musica.
*   - audioSource: Fuente de audio.
*   - mainMenuMusic: Musica del menu principal.
*   - gameplayMusic: Musica del juego.
*
*   Funcionamiento:
*   - Al iniciar, carga la musica del menu principal y del juego.
*   - Al cambiar de escena, reproduce la musica correspondiente.
*
*   Flujo:
*   1. El jugador inicia el juego.
*   2. Se reproduce la musica del menu principal.
*   3. El jugador cambia a la escena del juego.
*   4. Se reproduce la musica del juego.
*/

public class MusicManager : MonoBehaviour
{
    private const string MainMenuSceneName = "MainMenu";
    private const string GameplaySceneName = "SampleScene";
    private const string MainMenuMusicResourcePath = "Music/MainMenuMusic";
    private const string GameplayMusicResourcePath = "Music/InGameMusic";

    public static MusicManager Instance { get; private set; }

    [Header("Audio")]
    [Range(0f, 1f)] public float musicVolume = 0.6f;

    private AudioSource audioSource;
    private AudioClip mainMenuMusic;
    private AudioClip gameplayMusic;

    /*
    * Metodo que asegura que solo haya una instancia del MusicManager.
    */
    [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.BeforeSceneLoad)]
    static void EnsureInstance()
    {
        if (Instance != null)
        {
            return;
        }

        MusicManager existing = FindFirstObjectByType<MusicManager>(FindObjectsInactive.Include);
        if (existing != null)
        {
            Instance = existing;
            return;
        }

        GameObject go = new GameObject("MusicManager");
        Instance = go.AddComponent<MusicManager>();
    }

    /*
    * Metodo que se llama al crear el objeto.
    */
    void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
        DontDestroyOnLoad(gameObject);

        audioSource = GetComponent<AudioSource>();
        if (audioSource == null)
        {
            audioSource = gameObject.AddComponent<AudioSource>();
        }

        audioSource.playOnAwake = false;
        audioSource.loop = true;
        audioSource.spatialBlend = 0f;
        audioSource.volume = musicVolume;
        audioSource.ignoreListenerPause = true;

        mainMenuMusic = Resources.Load<AudioClip>(MainMenuMusicResourcePath);
        gameplayMusic = Resources.Load<AudioClip>(GameplayMusicResourcePath);
    }

    /*
    * Metodo que se llama cuando el script se habilita.
    */
    void OnEnable()
    {
        SceneManager.sceneLoaded += OnSceneLoaded;
    }

    /*
    * Metodo que se llama al iniciar la escena.
    */
    void Start()
    {
        ApplyMusicForScene(SceneManager.GetActiveScene().name);
    }

    /*
    * Metodo que se llama cuando el script se deshabilita.
    */
    void OnDisable()
    {
        SceneManager.sceneLoaded -= OnSceneLoaded;
    }

    /*
    * Metodo que se llama cada frame.
    */
    void Update()
    {
        if (audioSource != null)
        {
            audioSource.volume = musicVolume;
        }
    }

    /*
    * Metodo que se llama cuando se carga una escena.
    */
    void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        ApplyMusicForScene(scene.name);
    }

    /*
    * Metodo que aplica la musica para la escena actual.
    */
    void ApplyMusicForScene(string sceneName)
    {
        AudioClip targetClip = null;

        if (sceneName == MainMenuSceneName)
        {
            targetClip = mainMenuMusic;
        }
        else if (sceneName == GameplaySceneName)
        {
            targetClip = gameplayMusic;
        }

        if (targetClip == null)
        {
            return;
        }

        if (audioSource.clip == targetClip && audioSource.isPlaying)
        {
            return;
        }

        audioSource.clip = targetClip;
        audioSource.Play();
    }
}
