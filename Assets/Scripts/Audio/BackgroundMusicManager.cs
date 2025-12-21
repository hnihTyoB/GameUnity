using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

public class BackgroundMusicManager : Singleton<BackgroundMusicManager>
{
    [Header("Music Clips")]
    [SerializeField] private AudioClip menuMusic; // wait.mp3
    [SerializeField] private AudioClip gameMusic; // music.mp3
    
    [Header("Audio Settings")]
    [SerializeField] private float fadeDuration = 1f;
    
    private AudioSource audioSource;
    private string currentSceneName;
    private Coroutine fadeCoroutine;
    
    protected override void Awake()
    {
        base.Awake();
        
        audioSource = GetComponent<AudioSource>();
        if (audioSource == null)
        {
            audioSource = gameObject.AddComponent<AudioSource>();
        }
        
        audioSource.loop = true;
        audioSource.playOnAwake = false;
        audioSource.volume = 0.7f; 
        
        LoadMusicClips();
    }
    
    private void Start()
    {
        LoadVolumeFromSettings();
        
        currentSceneName = SceneManager.GetActiveScene().name;
        PlayMusicForScene(currentSceneName);
        
        SceneManager.sceneLoaded += OnSceneLoaded;
    }
    
    private void OnDestroy()
    {
        SceneManager.sceneLoaded -= OnSceneLoaded;
    }
    
    private void LoadMusicClips()
    {
        if (menuMusic == null)
        {
            Debug.LogWarning("BackgroundMusicManager: Menu Music (wait.mp3) not assigned! Please assign it in Inspector.");
        }
        if (gameMusic == null)
        {
            Debug.LogWarning("BackgroundMusicManager: Game Music (music.mp3) not assigned! Please assign it in Inspector.");
        }
    }
    
    private void LoadVolumeFromSettings()
    {
        float musicVolume = PlayerPrefs.GetFloat("MusicVolume", 0.7f);
        
        if (audioSource != null)
        {
            audioSource.volume = musicVolume;
        }
    }
    
    private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        string newSceneName = scene.name;
        
        if (newSceneName != currentSceneName)
        {
            currentSceneName = newSceneName;
            PlayMusicForScene(newSceneName);
        }
        else
        {
            Debug.Log($"BackgroundMusicManager: Scene name unchanged, keeping current music");
        }
        
    }
    
    private void PlayMusicForScene(string sceneName)
    {
        AudioClip clipToPlay = null;
        
        if (sceneName.Contains("Menu") || sceneName.Contains("MainMenu"))
        {
            clipToPlay = menuMusic;
        }
        else
        {
            clipToPlay = gameMusic;
    
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
    
    public void PlayMenuMusic()
    {
        if (menuMusic != null)
        {
            PlayMusic(menuMusic);
        
        }
        else
        {
            Debug.LogWarning("BackgroundMusicManager: Menu music (wait.mp3) not assigned! Cannot play menu music.");
        }
    }
    
   
    public void PlayGameMusic()
    {
        if (gameMusic != null)
        {
            PlayMusic(gameMusic);
        }
        else
        {
            Debug.LogWarning("BackgroundMusicManager: Game music (music.mp3) not assigned! Cannot play game music.");
        }
    }
    

    private void PlayMusic(AudioClip clip)
    {
        if (audioSource == null || clip == null)
        {
            return;
        }
        
    
        if (audioSource.clip == clip && audioSource.isPlaying)
        {
            return;
        }
        
        if (fadeCoroutine != null)
        {
            StopCoroutine(fadeCoroutine);
            fadeCoroutine = null;
        }
        
        fadeCoroutine = StartCoroutine(FadeAndPlayMusic(clip));
        
        if (fadeCoroutine == null)
        {
            Debug.LogError("BackgroundMusicManager: Failed to start FadeAndPlayMusic coroutine!");
        }
    }
    
    private IEnumerator FadeAndPlayMusic(AudioClip newClip)
    {
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
        
        audioSource.clip = newClip;
        audioSource.Play();
        
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
    

    public void SetVolume(float volume)
    {
        if (audioSource != null)
        {
            audioSource.volume = Mathf.Clamp01(volume);
            PlayerPrefs.SetFloat("MusicVolume", volume);
            PlayerPrefs.Save();
        }
    }
    
    public float GetVolume()
    {
        return audioSource != null ? audioSource.volume : 0.7f;
    }
    
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
        audioSource.volume = startVolume; 
    }
}

