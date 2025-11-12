using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System;

/// <summary>
/// Quản lý độ khó của game (Easy, Normal, Hard)
/// Attach vào SettingsPanel hoặc ContentTable
/// </summary>
public class DifficultyManager : MonoBehaviour
{
    [Header("Difficulty Toggles")]
    [SerializeField] private Toggle easyToggle;
    [SerializeField] private Toggle normalToggle;
    [SerializeField] private Toggle hardToggle;

    [Header("Toggle Group")]
    [SerializeField] private ToggleGroup difficultyToggleGroup;

    [Header("Visual Feedback (Optional)")]
    [SerializeField] private Color selectedColor = new Color(1f, 0.8f, 0.2f); // Vàng
    [SerializeField] private Color normalColor = Color.white;

    // Key để lưu vào PlayerPrefs
    private const string DIFFICULTY_KEY = "GameDifficulty";

    // Enum cho độ khó
    public enum Difficulty
    {
        Easy = 0,
        Normal = 1,
        Hard = 2
    }

    private Difficulty currentDifficulty = Difficulty.Normal;

    // Event khi thay đổi độ khó (để các script khác subscribe)
    public static event Action<Difficulty> OnDifficultyChanged;

    private void Start()
    {
        // Setup Toggle Group nếu chưa có
        SetupToggleGroup();

        // Load difficulty đã lưu
        LoadDifficulty();

        // Gán sự kiện cho toggles
        if (easyToggle != null)
        {
            easyToggle.onValueChanged.AddListener((isOn) => 
            {
                if (isOn) SetDifficulty(Difficulty.Easy);
            });
        }

        if (normalToggle != null)
        {
            normalToggle.onValueChanged.AddListener((isOn) => 
            {
                if (isOn) SetDifficulty(Difficulty.Normal);
            });
        }

        if (hardToggle != null)
        {
            hardToggle.onValueChanged.AddListener((isOn) => 
            {
                if (isOn) SetDifficulty(Difficulty.Hard);
            });
        }

        // Set toggle ban đầu
        UpdateToggleStates();
    }

    private void SetupToggleGroup()
    {
        // Nếu chưa có ToggleGroup, tạo mới
        if (difficultyToggleGroup == null)
        {
            difficultyToggleGroup = gameObject.AddComponent<ToggleGroup>();
        }

        difficultyToggleGroup.allowSwitchOff = false; // Phải luôn có 1 toggle active

        // Gán toggle group cho các toggles
        if (easyToggle != null)
            easyToggle.group = difficultyToggleGroup;
        
        if (normalToggle != null)
            normalToggle.group = difficultyToggleGroup;
        
        if (hardToggle != null)
            hardToggle.group = difficultyToggleGroup;
    }

    private void LoadDifficulty()
    {
        // Load từ PlayerPrefs, default = Normal (1)
        int savedDifficulty = PlayerPrefs.GetInt(DIFFICULTY_KEY, (int)Difficulty.Normal);
        currentDifficulty = (Difficulty)savedDifficulty;

        Debug.Log($"DifficultyManager: Loaded difficulty - {currentDifficulty}");
    }

    private void SaveDifficulty()
    {
        PlayerPrefs.SetInt(DIFFICULTY_KEY, (int)currentDifficulty);
        PlayerPrefs.Save();

        Debug.Log($"DifficultyManager: Saved difficulty - {currentDifficulty}");
    }

    public void SetDifficulty(Difficulty difficulty)
    {
        if (currentDifficulty == difficulty)
            return;

        currentDifficulty = difficulty;
        
        // Lưu vào PlayerPrefs
        SaveDifficulty();

        // Trigger event
        OnDifficultyChanged?.Invoke(currentDifficulty);

        // Update visual
        UpdateToggleStates();

        Debug.Log($"DifficultyManager: Difficulty changed to {currentDifficulty}");
    }

    private void UpdateToggleStates()
    {
        // Set toggle tương ứng
        switch (currentDifficulty)
        {
            case Difficulty.Easy:
                if (easyToggle != null)
                    easyToggle.isOn = true;
                break;

            case Difficulty.Normal:
                if (normalToggle != null)
                    normalToggle.isOn = true;
                break;

            case Difficulty.Hard:
                if (hardToggle != null)
                    hardToggle.isOn = true;
                break;
        }

        // Update colors (optional)
        UpdateToggleColors();
    }

    private void UpdateToggleColors()
    {
        // Đổi màu button được chọn
        if (easyToggle != null)
        {
            var colors = easyToggle.colors;
            colors.normalColor = easyToggle.isOn ? selectedColor : normalColor;
            easyToggle.colors = colors;
        }

        if (normalToggle != null)
        {
            var colors = normalToggle.colors;
            colors.normalColor = normalToggle.isOn ? selectedColor : normalColor;
            normalToggle.colors = colors;
        }

        if (hardToggle != null)
        {
            var colors = hardToggle.colors;
            colors.normalColor = hardToggle.isOn ? selectedColor : normalColor;
            hardToggle.colors = colors;
        }
    }

    // Public methods

    public void SetEasy()
    {
        SetDifficulty(Difficulty.Easy);
    }

    public void SetNormal()
    {
        SetDifficulty(Difficulty.Normal);
    }

    public void SetHard()
    {
        SetDifficulty(Difficulty.Hard);
    }

    // Helper methods để game sử dụng

    public static float GetDifficultyMultiplier()
    {
        int difficulty = PlayerPrefs.GetInt(DIFFICULTY_KEY, (int)Difficulty.Normal);
        
        switch ((Difficulty)difficulty)
        {
            case Difficulty.Easy:
                return 0.7f; // Enemy 30% yếu hơn
            case Difficulty.Normal:
                return 1.0f; // Chuẩn
            case Difficulty.Hard:
                return 1.5f; // Enemy 50% mạnh hơn
            default:
                return 1.0f;
        }
    }

    public static bool IsEasyMode()
    {
        return PlayerPrefs.GetInt(DIFFICULTY_KEY, (int)Difficulty.Normal) == (int)Difficulty.Easy;
    }

    public static bool IsNormalMode()
    {
        return PlayerPrefs.GetInt(DIFFICULTY_KEY, (int)Difficulty.Normal) == (int)Difficulty.Normal;
    }

    public static bool IsHardMode()
    {
        return PlayerPrefs.GetInt(DIFFICULTY_KEY, (int)Difficulty.Normal) == (int)Difficulty.Hard;
    }

    /// <summary>
    /// Get current difficulty (static method for ScoreManager and other systems)
    /// </summary>
    public static Difficulty GetCurrentDifficulty()
    {
        int difficulty = PlayerPrefs.GetInt(DIFFICULTY_KEY, (int)Difficulty.Normal);
        return (Difficulty)difficulty;
    }

    private void OnDestroy()
    {
        // Cleanup listeners
        if (easyToggle != null)
            easyToggle.onValueChanged.RemoveAllListeners();
        
        if (normalToggle != null)
            normalToggle.onValueChanged.RemoveAllListeners();
        
        if (hardToggle != null)
            hardToggle.onValueChanged.RemoveAllListeners();
    }
}

