using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Manages Sound Effects (SFX) for the game
/// Handles playing, stopping, and volume control for SFX
/// </summary>
public class SFXManager : Singleton<SFXManager>
{
    [Header("SFX Clips")]
    [SerializeField] private AudioClip flashLightSound; // flash_light.ogg
    [SerializeField] private AudioClip shieldSound; // shield.wav
    [SerializeField] private AudioClip hitSound; // hit.wav
    [SerializeField] private AudioClip batterySound; // battery sound
    [SerializeField] private AudioClip staminaSound; // stamina sound
    [SerializeField] private AudioClip dashSound; // dash sound
    [SerializeField] private AudioClip ghostSound; // ghost sound (shadow attack)
    [SerializeField] private AudioClip shootSound; // shoot sound (enemy projectile)
    
    [Header("Audio Sources")]
    [SerializeField] private AudioSource flashLightSource; // For looping flashlight sound
    [SerializeField] private AudioSource shieldSource; // For looping shield sound
    [SerializeField] private AudioSource oneShotSource; // For one-shot sounds like hit
    
    private float sfxVolume = 0.7f; // Default volume
    
    protected override void Awake()
    {
        base.Awake();
        
        // Create audio sources if they don't exist
        CreateAudioSources();
        
        // Try to load audio clips from Resources if not assigned
        LoadAudioClipsFromResources();
        
        // Load volume from PlayerPrefs
        LoadVolumeFromSettings();
    }
    
    private void Start()
    {
        // Configure audio sources
        ConfigureAudioSources();
    }
    
    /// <summary>
    /// Try to load audio clips from Resources if not assigned in Inspector
    /// </summary>
    private void LoadAudioClipsFromResources()
    {
        // Try to load flash_light.ogg
        if (flashLightSound == null)
        {
            flashLightSound = Resources.Load<AudioClip>("Audio/flash_light");
            if (flashLightSound == null)
            {
                flashLightSound = Resources.Load<AudioClip>("flash_light");
            }
        }
        
        // Try to load shield.wav
        if (shieldSound == null)
        {
            shieldSound = Resources.Load<AudioClip>("Audio/shield");
            if (shieldSound == null)
            {
                shieldSound = Resources.Load<AudioClip>("shield");
            }
        }
        
        // Try to load hit.wav
        if (hitSound == null)
        {
            hitSound = Resources.Load<AudioClip>("Audio/hit");
            if (hitSound == null)
            {
                hitSound = Resources.Load<AudioClip>("hit");
            }
        }
        
        // Try to load battery sound
        if (batterySound == null)
        {
            batterySound = Resources.Load<AudioClip>("Audio/battery");
            if (batterySound == null)
            {
                batterySound = Resources.Load<AudioClip>("battery");
            }
        }
        
        // Try to load stamina sound
        if (staminaSound == null)
        {
            staminaSound = Resources.Load<AudioClip>("Audio/stamina");
            if (staminaSound == null)
            {
                staminaSound = Resources.Load<AudioClip>("stamina");
            }
        }
        
        // Try to load dash sound
        if (dashSound == null)
        {
            dashSound = Resources.Load<AudioClip>("Audio/dash");
            if (dashSound == null)
            {
                dashSound = Resources.Load<AudioClip>("dash");
            }
        }
        
        // Try to load ghost sound
        if (ghostSound == null)
        {
            ghostSound = Resources.Load<AudioClip>("Audio/ghost");
            if (ghostSound == null)
            {
                ghostSound = Resources.Load<AudioClip>("ghost");
            }
        }
        
        // Try to load shoot sound
        if (shootSound == null)
        {
            shootSound = Resources.Load<AudioClip>("Audio/shoot");
            if (shootSound == null)
            {
                shootSound = Resources.Load<AudioClip>("shoot");
            }
        }
    }
    
    /// <summary>
    /// Create audio sources if they don't exist
    /// </summary>
    private void CreateAudioSources()
    {
        // Create flashlight audio source
        if (flashLightSource == null)
        {
            GameObject flashLightObj = new GameObject("FlashlightAudioSource");
            flashLightObj.transform.SetParent(transform);
            flashLightSource = flashLightObj.AddComponent<AudioSource>();
        }
        
        // Create shield audio source
        if (shieldSource == null)
        {
            GameObject shieldObj = new GameObject("ShieldAudioSource");
            shieldObj.transform.SetParent(transform);
            shieldSource = shieldObj.AddComponent<AudioSource>();
        }
        
        // Create one-shot audio source
        if (oneShotSource == null)
        {
            GameObject oneShotObj = new GameObject("OneShotAudioSource");
            oneShotObj.transform.SetParent(transform);
            oneShotSource = oneShotObj.AddComponent<AudioSource>();
        }
    }
    
    /// <summary>
    /// Configure audio sources
    /// </summary>
    private void ConfigureAudioSources()
    {
        // Configure flashlight source (loop)
        if (flashLightSource != null)
        {
            flashLightSource.loop = true;
            flashLightSource.playOnAwake = false;
            flashLightSource.volume = sfxVolume;
            flashLightSource.spatialBlend = 0f; // 2D sound
        }
        
        // Configure shield source (loop)
        if (shieldSource != null)
        {
            shieldSource.loop = true;
            shieldSource.playOnAwake = false;
            shieldSource.volume = sfxVolume;
            shieldSource.spatialBlend = 0f; // 2D sound
        }
        
        // Configure one-shot source
        if (oneShotSource != null)
        {
            oneShotSource.loop = false;
            oneShotSource.playOnAwake = false;
            oneShotSource.volume = sfxVolume;
            oneShotSource.spatialBlend = 0f; // 2D sound
        }
    }
    
    /// <summary>
    /// Load volume from SettingsManager or PlayerPrefs
    /// </summary>
    private void LoadVolumeFromSettings()
    {
        sfxVolume = PlayerPrefs.GetFloat("SFXVolume", 0.7f);
        UpdateAllVolumes();
    }
    
    /// <summary>
    /// Update all audio source volumes
    /// </summary>
    private void UpdateAllVolumes()
    {
        if (flashLightSource != null)
        {
            flashLightSource.volume = sfxVolume;
        }
        
        if (shieldSource != null)
        {
            shieldSource.volume = sfxVolume;
        }
        
        if (oneShotSource != null)
        {
            oneShotSource.volume = sfxVolume;
        }
    }
    
    /// <summary>
    /// Play flashlight sound (loop)
    /// </summary>
    public void PlayFlashlightSound()
    {
        if (flashLightSource != null && flashLightSound != null)
        {
            if (!flashLightSource.isPlaying)
            {
                flashLightSource.clip = flashLightSound;
                flashLightSource.Play();
            }
        }
        else
        {
            Debug.LogWarning("SFXManager: Flashlight sound or audio source not assigned!");
        }
    }
    
    /// <summary>
    /// Stop flashlight sound
    /// </summary>
    public void StopFlashlightSound()
    {
        if (flashLightSource != null && flashLightSource.isPlaying)
        {
            flashLightSource.Stop();
        }
    }
    
    /// <summary>
    /// Play shield sound (loop)
    /// </summary>
    public void PlayShieldSound()
    {
        if (shieldSource != null && shieldSound != null)
        {
            if (!shieldSource.isPlaying)
            {
                shieldSource.clip = shieldSound;
                shieldSource.Play();
            }
        }
        else
        {
            Debug.LogWarning("SFXManager: Shield sound or audio source not assigned!");
        }
    }
    
    /// <summary>
    /// Stop shield sound
    /// </summary>
    public void StopShieldSound()
    {
        if (shieldSource != null && shieldSource.isPlaying)
        {
            shieldSource.Stop();
        }
    }
    
    /// <summary>
    /// Play hit sound (one-shot)
    /// </summary>
    public void PlayHitSound()
    {
        // Ensure audio source exists
        if (oneShotSource == null)
        {
            CreateAudioSources();
            ConfigureAudioSources();
        }
        
        // Double-check after creation
        if (oneShotSource == null)
        {
            Debug.LogError("SFXManager: Failed to create OneShotSource!");
            return;
        }
        
        if (hitSound != null)
        {
            oneShotSource.PlayOneShot(hitSound, sfxVolume);
        }
        else
        {
            Debug.LogWarning("SFXManager: Hit sound not assigned! Please assign hit.wav in Inspector.");
        }
    }
    
    /// <summary>
    /// Set SFX volume (called by SettingsManager)
    /// </summary>
    public void SetVolume(float volume)
    {
        sfxVolume = Mathf.Clamp01(volume);
        UpdateAllVolumes();
        PlayerPrefs.SetFloat("SFXVolume", sfxVolume);
        PlayerPrefs.Save();
    }
    
    /// <summary>
    /// Get current SFX volume
    /// </summary>
    public float GetVolume()
    {
        return sfxVolume;
    }
    
    /// <summary>
    /// Play a custom one-shot sound
    /// </summary>
    public void PlayOneShot(AudioClip clip, float volumeMultiplier = 1f)
    {
        if (oneShotSource != null && clip != null)
        {
            oneShotSource.PlayOneShot(clip, sfxVolume * volumeMultiplier);
        }
    }
    
    /// <summary>
    /// Play battery pickup sound (one-shot)
    /// </summary>
    public void PlayBatterySound()
    {
        // Ensure audio source exists
        if (oneShotSource == null)
        {
            CreateAudioSources();
            ConfigureAudioSources();
        }
        
        if (oneShotSource == null)
        {
            Debug.LogError("SFXManager: Failed to create OneShotSource!");
            return;
        }
        
        if (batterySound != null)
        {
            oneShotSource.PlayOneShot(batterySound, sfxVolume);
        }
        else
        {
            Debug.LogWarning("SFXManager: Battery sound not assigned! Please assign battery sound in Inspector.");
        }
    }
    
    /// <summary>
    /// Play stamina pickup sound (one-shot)
    /// </summary>
    public void PlayStaminaSound()
    {
        // Ensure audio source exists
        if (oneShotSource == null)
        {
            CreateAudioSources();
            ConfigureAudioSources();
        }
        
        if (oneShotSource == null)
        {
            Debug.LogError("SFXManager: Failed to create OneShotSource!");
            return;
        }
        
        if (staminaSound != null)
        {
            oneShotSource.PlayOneShot(staminaSound, sfxVolume);
        }
        else
        {
            Debug.LogWarning("SFXManager: Stamina sound not assigned! Please assign stamina sound in Inspector.");
        }
    }
    
    /// <summary>
    /// Play dash sound (one-shot)
    /// </summary>
    public void PlayDashSound()
    {
        // Ensure audio source exists
        if (oneShotSource == null)
        {
            CreateAudioSources();
            ConfigureAudioSources();
        }
        
        if (oneShotSource == null)
        {
            Debug.LogError("SFXManager: Failed to create OneShotSource!");
            return;
        }
        
        if (dashSound != null)
        {
            oneShotSource.PlayOneShot(dashSound, sfxVolume);
        }
        else
        {
            Debug.LogWarning("SFXManager: Dash sound not assigned! Please assign dash sound in Inspector.");
        }
    }
    
    /// <summary>
    /// Play ghost sound (one-shot) - when shadow attacks
    /// </summary>
    public void PlayGhostSound()
    {
        // Ensure audio source exists
        if (oneShotSource == null)
        {
            CreateAudioSources();
            ConfigureAudioSources();
        }
        
        if (oneShotSource == null)
        {
            Debug.LogError("SFXManager: Failed to create OneShotSource!");
            return;
        }
        
        if (ghostSound != null)
        {
            oneShotSource.PlayOneShot(ghostSound, sfxVolume);
        }
        else
        {
            Debug.LogWarning("SFXManager: Ghost sound not assigned! Please assign ghost sound in Inspector.");
        }
    }
    
    /// <summary>
    /// Play shoot sound (one-shot) - when enemy shoots projectile at player
    /// </summary>
    public void PlayShootSound()
    {
        // Ensure audio source exists
        if (oneShotSource == null)
        {
            CreateAudioSources();
            ConfigureAudioSources();
        }
        
        if (oneShotSource == null)
        {
            Debug.LogError("SFXManager: Failed to create OneShotSource!");
            return;
        }
        
        if (shootSound != null)
        {
            oneShotSource.PlayOneShot(shootSound, sfxVolume);
        }
        else
        {
            Debug.LogWarning("SFXManager: Shoot sound not assigned! Please assign shoot sound in Inspector.");
        }
    }
}

