using System.Collections;
using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// Manages player battery system - replaces health system
/// Battery powers flashlight and other equipment
/// </summary>
public class BatteryManager : Singleton<BatteryManager>
{
    [Header("Battery Settings")]
    [SerializeField] private float maxBattery = 10f; // Maximum battery capacity (changed to float)
    [SerializeField] private float currentBattery; // Current battery level (changed to float for smooth drain)
    [SerializeField] private float batteryDrainRate = 0.5f; // Battery drain per second when flashlight is on
    [SerializeField] private bool useContinuousDrain = true; // Use continuous drain instead of interval-based
    
    [Header("UI References")]
    [SerializeField] private Slider batterySlider; // Battery UI slider
    [SerializeField] private string batterySliderName = "Battery Slider"; // Name to find battery slider
    
    [Header("Battery Effects")]
    [SerializeField] private GameObject lowBatteryWarning; // Warning UI when battery is low
    [SerializeField] private float lowBatteryThreshold = 2f; // Show warning when battery <= this
    
    private bool isFlashlightOn = false;
    private Coroutine batteryDrainCoroutine;
    private bool isLowBatteryWarningShown = false;
    private float baseBatteryDrainRate; // Lưu giá trị gốc
    
    public float CurrentBattery => currentBattery;
    public float MaxBattery => maxBattery;
    public bool IsBatteryLow => currentBattery <= lowBatteryThreshold;
    
    protected override void Awake()
    {
        base.Awake();
        currentBattery = maxBattery; // Start with full battery
        
        // Lưu giá trị gốc
        baseBatteryDrainRate = batteryDrainRate;
        
        // Áp dụng difficulty
        ApplyDifficultySettings();
    }
    
    private void OnEnable()
    {
        // Subscribe vào event khi difficulty thay đổi
        DifficultyManager.OnDifficultyChanged += OnDifficultyChanged;
    }
    
    private void OnDisable()
    {
        // Unsubscribe
        DifficultyManager.OnDifficultyChanged -= OnDifficultyChanged;
    }
    
    /// <summary>
    /// Áp dụng difficulty vào battery drain rate
    /// </summary>
    private void ApplyDifficultySettings()
    {
        float multiplier = DifficultyManager.GetDifficultyMultiplier();
        
        // Battery drain: Easy (0.7x) = 0.35/s, Normal (1.0x) = 0.5/s, Hard (1.5x) = 0.75/s
        batteryDrainRate = baseBatteryDrainRate * multiplier;
        
        Debug.Log($"BatteryManager: Difficulty applied - Drain rate: {batteryDrainRate:F2}/s (base: {baseBatteryDrainRate:F2}, multiplier: {multiplier:F2}x)");
    }
    
    /// <summary>
    /// Callback khi difficulty thay đổi
    /// </summary>
    private void OnDifficultyChanged(DifficultyManager.Difficulty newDifficulty)
    {
        ApplyDifficultySettings();
        // Note: Nếu FlashlightEffect đang set drain rate, nó sẽ override lại
    }
    
    private void Start()
    {
        // Initialize battery to full
        currentBattery = maxBattery;
        UpdateBatterySlider();
        
        // Hide low battery warning initially
        if (lowBatteryWarning != null)
        {
            lowBatteryWarning.SetActive(false);
        }
    }
    
    private void Update()
    {
        // Check for low battery warning
        CheckLowBatteryWarning();
    }
    
    /// <summary>
    /// Add battery power to player
    /// </summary>
    public void AddBattery(float amount)
    {
        float oldBattery = currentBattery;
        currentBattery = Mathf.Min(currentBattery + amount, maxBattery);
        UpdateBatterySlider();
        
        Debug.Log($"Battery added: +{amount}. Changed from {oldBattery:F1} to {currentBattery:F1}/{maxBattery}");
    }
    
    /// <summary>
    /// Drain battery (called when flashlight is on)
    /// </summary>
    public void StartBatteryDrain()
    {
        if (batteryDrainCoroutine == null)
        {
            batteryDrainCoroutine = StartCoroutine(BatteryDrainRoutine());
        }
    }
    
    /// <summary>
    /// Stop battery drain (called when flashlight is off)
    /// </summary>
    public void StopBatteryDrain()
    {
        if (batteryDrainCoroutine != null)
        {
            StopCoroutine(batteryDrainCoroutine);
            batteryDrainCoroutine = null;
        }
    }
    
    private IEnumerator BatteryDrainRoutine()
    {
        while (currentBattery > 0)
        {
            if (useContinuousDrain)
            {
                // Continuous drain - drain every frame for smooth animation
                float drainAmount = batteryDrainRate * Time.deltaTime;
                currentBattery = Mathf.Max(currentBattery - drainAmount, 0);
                UpdateBatterySlider();
                
                // Only log when battery decreases by whole number
                if (Mathf.FloorToInt(currentBattery) != Mathf.FloorToInt(currentBattery + drainAmount))
                {
                    Debug.Log($"Battery: {currentBattery:F1}/{maxBattery}");
                }
            }
            else
            {
                // Interval-based drain (old method)
                yield return new WaitForSeconds(0.02f);
                float drainAmount = batteryDrainRate * 0.02f;
                currentBattery = Mathf.Max(currentBattery - drainAmount, 0);
                UpdateBatterySlider();
            }
            
            yield return null; // Wait one frame
        }
        
        // Battery is empty
        OnBatteryEmpty();
    }
    
    private void OnBatteryEmpty()
    {
        Debug.Log("Battery is empty!");
        
        // Turn off flashlight if it's on (check both Flashlight and FlashlightCone)
        Flashlight flashlight = FindObjectOfType<Flashlight>();
        FlashlightCone flashlightCone = FindObjectOfType<FlashlightCone>();
        
        if (flashlight != null && flashlight.IsLightOn())
        {
            flashlight.SetLightState(false);
        }
        else if (flashlightCone != null && flashlightCone.IsLightOn())
        {
            flashlightCone.SetLightState(false);
        }
        
        // Show empty battery message
        // You can add UI notification here
    }
    
    private void CheckLowBatteryWarning()
    {
        if (currentBattery <= lowBatteryThreshold && !isLowBatteryWarningShown)
        {
            ShowLowBatteryWarning();
        }
        else if (currentBattery > lowBatteryThreshold && isLowBatteryWarningShown)
        {
            HideLowBatteryWarning();
        }
    }
    
    private void ShowLowBatteryWarning()
    {
        isLowBatteryWarningShown = true;
        if (lowBatteryWarning != null)
        {
            lowBatteryWarning.SetActive(true);
        }
        
        Debug.Log("Low battery warning!");
    }
    
    private void HideLowBatteryWarning()
    {
        isLowBatteryWarningShown = false;
        if (lowBatteryWarning != null)
        {
            lowBatteryWarning.SetActive(false);
        }
    }
    
    private void UpdateBatterySlider()
    {
        if (batterySlider == null)
        {
            // Try to find battery slider by name
            GameObject sliderObj = GameObject.Find(batterySliderName);
            if (sliderObj != null)
            {
                batterySlider = sliderObj.GetComponent<Slider>();
            }
        }
        
        if (batterySlider != null)
        {
            // Set slider properties for smooth float values
            batterySlider.minValue = 0f;
            batterySlider.maxValue = maxBattery;
            batterySlider.wholeNumbers = false; // Allow decimal values!
            
            if (useContinuousDrain)
            {
                // Direct update for continuous drain (smooth)
                batterySlider.value = currentBattery;
            }
            else
            {
                // Smooth animation for interval-based drain
                StartCoroutine(SmoothSliderUpdate());
            }
            
            // Optional: Change color based on battery level
            UpdateBatterySliderColor();
        }
    }
    
    private IEnumerator SmoothSliderUpdate()
    {
        if (batterySlider == null) yield break;
        
        float startValue = batterySlider.value;
        float targetValue = currentBattery;
        float duration = 0.1f; // Animation duration
        float elapsed = 0f;
        
        while (elapsed < duration)
        {
            elapsed += Time.deltaTime;
            float t = elapsed / duration;
            
            // Smooth interpolation
            batterySlider.value = Mathf.Lerp(startValue, targetValue, t);
            
            yield return null;
        }
        
        // Ensure final value is exact
        batterySlider.value = targetValue;
    }
    
    private void UpdateBatterySliderColor()
    {
        if (batterySlider == null) return;
        
        // Get the fill image to change color
        var fillImage = batterySlider.fillRect.GetComponent<UnityEngine.UI.Image>();
        if (fillImage != null)
        {
            float batteryPercentage = currentBattery / maxBattery;
            
            if (batteryPercentage > 0.5f)
            {
                // Green when battery is high
                fillImage.color = Color.green;
            }
            else if (batteryPercentage > 0.2f)
            {
                // Yellow when battery is medium
                fillImage.color = Color.yellow;
            }
            else
            {
                // Red when battery is low
                fillImage.color = Color.red;
            }
        }
    }
    
    /// <summary>
    /// Check if player has enough battery for an action
    /// </summary>
    public bool HasEnoughBattery(float requiredAmount = 1f)
    {
        return currentBattery >= requiredAmount;
    }
    
    /// <summary>
    /// Use battery for an action (like flashlight)
    /// </summary>
    public bool UseBattery(float amount = 1f)
    {
        if (HasEnoughBattery(amount))
        {
            currentBattery = Mathf.Max(currentBattery - amount, 0);
            UpdateBatterySlider();
            return true;
        }
        return false;
    }
    
    /// <summary>
    /// Set battery level (for testing or special events)
    /// </summary>
    public void SetBattery(float amount)
    {
        currentBattery = Mathf.Clamp(amount, 0f, maxBattery);
        UpdateBatterySlider();
    }
    
    /// <summary>
    /// Set drain rate dynamically (for flashlight effects)
    /// Note: FlashlightEffect sẽ set rate này khi có effects (slow/stun/kill)
    /// Khi không có effects, rate sẽ về base rate (đã được điều chỉnh theo difficulty)
    /// </summary>
    public void SetDrainRate(float newRate)
    {
        batteryDrainRate = newRate;
    }
    
    /// <summary>
    /// Reset drain rate về base rate (đã được điều chỉnh theo difficulty)
    /// </summary>
    public void ResetDrainRate()
    {
        ApplyDifficultySettings();
    }
}
