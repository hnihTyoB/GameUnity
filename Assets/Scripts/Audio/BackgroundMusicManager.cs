using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

/// <summary>
/// Manages background music for different scenes
/// - MainMenu: plays "wait" music
/// - Game scenes: plays "music" 
/// </summary>
public class BackgroundMusicManager : Singleton<BackgroundMusicManager>
{
    [Header("Music Clips")]
    [SerializeField] private AudioClip menuMusic; // wait.mp3
    [SerializeField] private AudioClip gameMusic; // music.mp3
    
    [Header("Audio Settings")]
    [SerializeField] private float fadeDuration = 1f; // Fade in/out duration
    
    private AudioSource audioSource;
    private string currentSceneName;
    private Coroutine fadeCoroutine;
    
    protected override void Awake()
    {
        base.Awake();
        
        // Get or create AudioSource
        audioSource = GetComponent<AudioSource>();
        if (audioSource == null)
        {
            audioSource = gameObject.AddComponent<AudioSource>();
        }
        
        // Configure AudioSource
        audioSource.loop = true;
        audioSource.playOnAwake = false;
        audioSource.volume = 0.7f; // Default volume
        
        // Load music from Resources or use serialized fields
        LoadMusicClips();
    }
    
    private void Start()
    {
        // Set initial volume from SettingsManager if available
        LoadVolumeFromSettings();
        
        // Play music based on current scene
        currentSceneName = SceneManager.GetActiveScene().name;
        PlayMusicForScene(currentSceneName);
        
        // Listen for scene changes
        SceneManager.sceneLoaded += OnSceneLoaded;
    }
    
    private void OnDestroy()
    {
        SceneManager.sceneLoaded -= OnSceneLoaded;
    }
    
    /// <summary>
    /// Load music clips - user must assign in Inspector
    /// NOTE: Assign wait.mp3 to Menu Music and music.mp3 to Game Music in Inspector
    /// </summary>
    private void LoadMusicClips()
    {
        // Check if music clips are assigned
        if (menuMusic == null)
        {
            Debug.LogWarning("BackgroundMusicManager: Menu Music (wait.mp3) not assigned! Please assign it in Inspector.");
        }
        if (gameMusic == null)
        {
            Debug.LogWarning("BackgroundMusicManager: Game Music (music.mp3) not assigned! Please assign it in Inspector.");
        }
    }
    
    /// <summary>
    /// Load volume from SettingsManager or PlayerPrefs
    /// </summary>
    private void LoadVolumeFromSettings()
    {
        // Try to get volume from SettingsManager
        float musicVolume = PlayerPrefs.GetFloat("MusicVolume", 0.7f);
        
        if (audioSource != null)
        {
            audioSource.volume = musicVolume;
        }
    }
    
    /// <summary>
    /// Called when a new scene is loaded
    /// </summary>
    private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        string newSceneName = scene.name;
        
        Debug.Log($"BackgroundMusicManager: Scene loaded - {newSceneName} (Current: {currentSceneName})");
        
        // Only change music if scene changed
        if (newSceneName != currentSceneName)
        {
            Debug.Log($"BackgroundMusicManager: Scene changed from {currentSceneName} to {newSceneName}, changing music...");
            currentSceneName = newSceneName;
            PlayMusicForScene(newSceneName);
        }
        else
        {
            Debug.Log($"BackgroundMusicManager: Scene name unchanged, keeping current music");
        }
    }
    
    /// <summary>
    /// Play appropriate music for the scene
    /// </summary>
    private void PlayMusicForScene(string sceneName)
    {
        AudioClip clipToPlay = null;
        
        // Determine which music to play based on scene name
        if (sceneName.Contains("Menu") || sceneName.Contains("MainMenu"))
        {
            clipToPlay = menuMusic;
            Debug.Log($"BackgroundMusicManager: Playing menu music (wait.mp3) for scene: {sceneName}");
        }
        else
        {
            // Game scenes (Scene1, Scene2, etc.)
            clipToPlay = gameMusic;
            Debug.Log($"BackgroundMusicManager: Playing game music (music.mp3) for scene: {sceneName}");
        }
        
        if (clipToPlay != null)
        {
            PlayMusic(clipToPlay);
        }
        else
        {
            Debug.LogWarning($"BackgroundMusicManager: No music clip found for scene: {sceneName}");
        }
    }
    
    /// <summary>
    /// Force play menu music (wait.mp3) - called when EndLevelUI shows
    /// </summary>
    public void PlayMenuMusic()
    {
        if (menuMusic != null)
        {
            PlayMusic(menuMusic);
            Debug.Log("BackgroundMusicManager: Forced to play menu music (wait.mp3)");
        }
        else
        {
            Debug.LogWarning("BackgroundMusicManager: Menu music (wait.mp3) not assigned! Cannot play menu music.");
        }
    }
    
    /// <summary>
    /// Force play game music (music.mp3) - called when returning to game
    /// </summary>
    public void PlayGameMusic()
    {
        if (gameMusic != null)
        {
            PlayMusic(gameMusic);
            Debug.Log("BackgroundMusicManager: Forced to play game music (music.mp3)");
        }
        else
        {
            Debug.LogWarning("BackgroundMusicManager: Game music (music.mp3) not assigned! Cannot play game music.");
        }
    }
    
    /// <summary>
    /// Play music with fade in
    /// </summary>
    private void PlayMusic(AudioClip clip)
    {
        if (audioSource == null || clip == null) return;
        
        // If same clip is already playing, don't restart
        if (audioSource.clip == clip && audioSource.isPlaying)
        {
            return;
        }
        
        // Stop fade coroutine if running
        if (fadeCoroutine != null)
        {
            StopCoroutine(fadeCoroutine);
        }
        
        // Start fade and play
        fadeCoroutine = StartCoroutine(FadeAndPlayMusic(clip));
    }
    
    /// <summary>
    /// Fade out current music and fade in new music
    /// </summary>
    private IEnumerator FadeAndPlayMusic(AudioClip newClip)
    {
        // Fade out current music
        if (audioSource.isPlaying)
        {
            float startVolume = audioSource.volume;
            float elapsedTime = 0f;
            
            while (elapsedTime < fadeDuration)
            {
                elapsedTime += Time.unscaledDeltaTime;
                audioSource.volume = Mathf.Lerp(startVolume, 0f, elapsedTime / fadeDuration);
                yield return null;
            }
        }
        
        // Change clip
        audioSource.clip = newClip;
        audioSource.Play();
        
        // Fade in new music
        float targetVolume = PlayerPrefs.GetFloat("MusicVolume", 0.7f);
        float elapsedTime2 = 0f;
        
        while (elapsedTime2 < fadeDuration)
        {
            elapsedTime2 += Time.unscaledDeltaTime;
            audioSource.volume = Mathf.Lerp(0f, targetVolume, elapsedTime2 / fadeDuration);
            yield return null;
        }
        
        audioSource.volume = targetVolume;
    }
    
    /// <summary>
    /// Set music volume (called by SettingsManager)
    /// </summary>
    public void SetVolume(float volume)
    {
        if (audioSource != null)
        {
            audioSource.volume = Mathf.Clamp01(volume);
            PlayerPrefs.SetFloat("MusicVolume", volume);
            PlayerPrefs.Save();
        }
    }
    
    /// <summary>
    /// Get current music volume
    /// </summary>
    public float GetVolume()
    {
        return audioSource != null ? audioSource.volume : 0.7f;
    }
    
    /// <summary>
    /// Stop music
    /// </summary>
    public void StopMusic()
    {
        if (audioSource != null && audioSource.isPlaying)
        {
            if (fadeCoroutine != null)
            {
                StopCoroutine(fadeCoroutine);
            }
            StartCoroutine(FadeOutMusic());
        }
    }
    
    /// <summary>
    /// Fade out music
    /// </summary>
    private IEnumerator FadeOutMusic()
    {
        float startVolume = audioSource.volume;
        float elapsedTime = 0f;
        
        while (elapsedTime < fadeDuration)
        {
            elapsedTime += Time.unscaledDeltaTime;
            audioSource.volume = Mathf.Lerp(startVolume, 0f, elapsedTime / fadeDuration);
            yield return null;
        }
        
        audioSource.Stop();
        audioSource.volume = startVolume; // Restore volume for next play
    }
}

