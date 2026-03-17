using UnityEngine;
using UnityEngine.SceneManagement;

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

    void OnEnable()
    {
        SceneManager.sceneLoaded += OnSceneLoaded;
    }

    void Start()
    {
        ApplyMusicForScene(SceneManager.GetActiveScene().name);
    }

    void OnDisable()
    {
        SceneManager.sceneLoaded -= OnSceneLoaded;
    }

    void Update()
    {
        if (audioSource != null)
        {
            audioSource.volume = musicVolume;
        }
    }

    void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        ApplyMusicForScene(scene.name);
    }

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
