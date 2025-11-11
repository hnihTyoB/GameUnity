using UnityEngine;
using UnityEngine.UI;
using TMPro;

/// <summary>
/// Quản lý Settings Panel với volume sliders
/// Attach vào SettingsPanel GameObject
/// </summary>
public class SettingsManager : MonoBehaviour
{
    [Header("Music Settings")]
    [SerializeField] private Slider musicSlider;
    [SerializeField] private Button musicIncreaseButton;
    [SerializeField] private Button musicDecreaseButton;
    [SerializeField] private TextMeshProUGUI musicValueText; // Hiển thị % (optional)

    [Header("SFX Settings")]
    [SerializeField] private Slider sfxSlider;
    [SerializeField] private Button sfxIncreaseButton;
    [SerializeField] private Button sfxDecreaseButton;
    [SerializeField] private TextMeshProUGUI sfxValueText; // Hiển thị % (optional)

    [Header("Settings")]
    [SerializeField] private float stepValue = 0.1f; // Mỗi lần tăng/giảm 10%
    [SerializeField] private bool saveSettingsAutomatically = true;

    // Keys để lưu vào PlayerPrefs
    private const string MUSIC_VOLUME_KEY = "MusicVolume";
    private const string SFX_VOLUME_KEY = "SFXVolume";

    // Default values
    private const float DEFAULT_MUSIC_VOLUME = 0.7f;
    private const float DEFAULT_SFX_VOLUME = 0.7f;

    private void Start()
    {
        // Load settings đã lưu
        LoadSettings();

        // Gán sự kiện cho sliders
        if (musicSlider != null)
        {
            musicSlider.onValueChanged.AddListener(OnMusicVolumeChanged);
        }

        if (sfxSlider != null)
        {
            sfxSlider.onValueChanged.AddListener(OnSFXVolumeChanged);
        }

        // Gán sự kiện cho buttons
        if (musicIncreaseButton != null)
        {
            musicIncreaseButton.onClick.AddListener(() => AdjustVolume(musicSlider, stepValue));
        }

        if (musicDecreaseButton != null)
        {
            musicDecreaseButton.onClick.AddListener(() => AdjustVolume(musicSlider, -stepValue));
        }

        if (sfxIncreaseButton != null)
        {
            sfxIncreaseButton.onClick.AddListener(() => AdjustVolume(sfxSlider, stepValue));
        }

        if (sfxDecreaseButton != null)
        {
            sfxDecreaseButton.onClick.AddListener(() => AdjustVolume(sfxSlider, -stepValue));
        }

        // Update display
        UpdateMusicDisplay();
        UpdateSFXDisplay();
    }

    private void LoadSettings()
    {
        // Load music volume
        float musicVolume = PlayerPrefs.GetFloat(MUSIC_VOLUME_KEY, DEFAULT_MUSIC_VOLUME);
        if (musicSlider != null)
        {
            musicSlider.value = musicVolume;
        }

        // Load SFX volume
        float sfxVolume = PlayerPrefs.GetFloat(SFX_VOLUME_KEY, DEFAULT_SFX_VOLUME);
        if (sfxSlider != null)
        {
            sfxSlider.value = sfxVolume;
        }

        Debug.Log($"SettingsManager: Loaded settings - Music: {musicVolume:P0}, SFX: {sfxVolume:P0}");
    }

    private void SaveSettings()
    {
        if (musicSlider != null)
        {
            PlayerPrefs.SetFloat(MUSIC_VOLUME_KEY, musicSlider.value);
        }

        if (sfxSlider != null)
        {
            PlayerPrefs.SetFloat(SFX_VOLUME_KEY, sfxSlider.value);
        }

        PlayerPrefs.Save();
        Debug.Log("SettingsManager: Settings saved!");
    }

    private void OnMusicVolumeChanged(float value)
    {
        UpdateMusicDisplay();

        // Set music volume in BackgroundMusicManager
        if (BackgroundMusicManager.Instance != null)
        {
            BackgroundMusicManager.Instance.SetVolume(value);
        }

        if (saveSettingsAutomatically)
        {
            SaveSettings();
        }
    }

    private void OnSFXVolumeChanged(float value)
    {
        UpdateSFXDisplay();

        // TODO: Khi thêm AudioManager, set volume thật:
        // AudioManager.Instance.SetSFXVolume(value);

        if (saveSettingsAutomatically)
        {
            SaveSettings();
        }
    }

    private void AdjustVolume(Slider slider, float amount)
    {
        if (slider == null) return;

        float newValue = Mathf.Clamp01(slider.value + amount);
        slider.value = newValue;

        // Play sound effect (khi có AudioManager)
        // AudioManager.Instance.PlaySFX("ButtonClick");
    }

    private void UpdateMusicDisplay()
    {
        if (musicValueText != null && musicSlider != null)
        {
            musicValueText.text = $"{Mathf.RoundToInt(musicSlider.value * 100)}%";
        }
    }

    private void UpdateSFXDisplay()
    {
        if (sfxValueText != null && sfxSlider != null)
        {
            sfxValueText.text = $"{Mathf.RoundToInt(sfxSlider.value * 100)}%";
        }
    }

    // Public methods để gọi từ ngoài

    public void SetMusicVolume(float volume)
    {
        if (musicSlider != null)
        {
            musicSlider.value = Mathf.Clamp01(volume);
        }
    }

    public void SetSFXVolume(float volume)
    {
        if (sfxSlider != null)
        {
            sfxSlider.value = Mathf.Clamp01(volume);
        }
    }

    public float GetMusicVolume()
    {
        return musicSlider != null ? musicSlider.value : DEFAULT_MUSIC_VOLUME;
    }

    public float GetSFXVolume()
    {
        return sfxSlider != null ? sfxSlider.value : DEFAULT_SFX_VOLUME;
    }

    public void ResetToDefault()
    {
        SetMusicVolume(DEFAULT_MUSIC_VOLUME);
        SetSFXVolume(DEFAULT_SFX_VOLUME);
        SaveSettings();
        Debug.Log("SettingsManager: Reset to default values!");
    }

    // Gọi khi đóng settings panel (nếu không auto save)
    public void OnCloseSettings()
    {
        if (!saveSettingsAutomatically)
        {
            SaveSettings();
        }
    }

    private void OnDestroy()
    {
        // Cleanup listeners
        if (musicSlider != null)
        {
            musicSlider.onValueChanged.RemoveAllListeners();
        }

        if (sfxSlider != null)
        {
            sfxSlider.onValueChanged.RemoveAllListeners();
        }
    }
}

