using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class BatteryManager : Singleton<BatteryManager>
{
    [Header("Battery Settings")]
    [SerializeField] private float maxBattery = 10f; 
    [SerializeField] private float currentBattery; 
    [SerializeField] private float batteryDrainRate = 0.5f; 
    [SerializeField] private bool useContinuousDrain = true; 
    
    [Header("UI References")]
    [SerializeField] private Slider batterySlider; 
    [SerializeField] private string batterySliderName = "Battery Slider"; 
    
    [Header("Battery Effects")]
    [SerializeField] private GameObject lowBatteryWarning; 
    [SerializeField] private float lowBatteryThreshold = 2f; 
    
    private bool isFlashlightOn = false;
    private Coroutine batteryDrainCoroutine;
    private bool isLowBatteryWarningShown = false;
    private float baseBatteryDrainRate; 
    
    public float CurrentBattery => currentBattery;
    public float MaxBattery => maxBattery;
    public bool IsBatteryLow => currentBattery <= lowBatteryThreshold;
    
    protected override void Awake()
    {
        base.Awake();
        currentBattery = maxBattery; 
        baseBatteryDrainRate = batteryDrainRate;
        ApplyDifficultySettings();
    }
    
    private void OnEnable()
    {
        DifficultyManager.OnDifficultyChanged += OnDifficultyChanged;
    }
    
    private void OnDisable()
    {
        DifficultyManager.OnDifficultyChanged -= OnDifficultyChanged;
    }
    
    private void ApplyDifficultySettings()
    {
        float multiplier = DifficultyManager.GetDifficultyMultiplier();
        batteryDrainRate = baseBatteryDrainRate * multiplier;
        Debug.Log($"BatteryManager: Difficulty applied - Drain rate: {batteryDrainRate:F2}/s (base: {baseBatteryDrainRate:F2}, multiplier: {multiplier:F2}x)");
    }
    
    private void OnDifficultyChanged(DifficultyManager.Difficulty newDifficulty)
    {
        ApplyDifficultySettings();
    }
    
    private void Start()
    {
        currentBattery = maxBattery;
        UpdateBatterySlider();
        
        if (lowBatteryWarning != null)
        {
            lowBatteryWarning.SetActive(false);
        }
    }
    
    private void Update()
    {
        CheckLowBatteryWarning();
    }
    
    public void AddBattery(float amount)
    {
        float oldBattery = currentBattery;
        currentBattery = Mathf.Min(currentBattery + amount, maxBattery);
        UpdateBatterySlider();
        
        Debug.Log($"Battery added: +{amount}. Changed from {oldBattery:F1} to {currentBattery:F1}/{maxBattery}");
    }
    
    public void StartBatteryDrain()
    {
        if (batteryDrainCoroutine == null)
        {
            batteryDrainCoroutine = StartCoroutine(BatteryDrainRoutine());
        }
    }
    
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
                float drainAmount = batteryDrainRate * Time.deltaTime;
                currentBattery = Mathf.Max(currentBattery - drainAmount, 0);
                UpdateBatterySlider();
                
                if (Mathf.FloorToInt(currentBattery) != Mathf.FloorToInt(currentBattery + drainAmount))
                {
                    Debug.Log($"Battery: {currentBattery:F1}/{maxBattery}");
                }
            }
            else
            {
                yield return new WaitForSeconds(0.02f);
                float drainAmount = batteryDrainRate * 0.02f;
                currentBattery = Mathf.Max(currentBattery - drainAmount, 0);
                UpdateBatterySlider();
            }
            
            yield return null; 
        }
        
        OnBatteryEmpty();
    }
    
    private void OnBatteryEmpty()
    {
        Debug.Log("Battery is empty!");
        
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
            GameObject sliderObj = GameObject.Find(batterySliderName);
            if (sliderObj != null)
            {
                batterySlider = sliderObj.GetComponent<Slider>();
            }
        }
        
        if (batterySlider != null)
        {
            batterySlider.minValue = 0f;
            batterySlider.maxValue = maxBattery;
            batterySlider.wholeNumbers = false; 
            
            if (useContinuousDrain)
            {
                batterySlider.value = currentBattery;
            }
            else
            {
                StartCoroutine(SmoothSliderUpdate());
            }
            
            UpdateBatterySliderColor();
        }
    }
    
    private IEnumerator SmoothSliderUpdate()
    {
        if (batterySlider == null) yield break;
        
        float startValue = batterySlider.value;
        float targetValue = currentBattery;
        float duration = 0.1f; 
        float elapsed = 0f;
        
        while (elapsed < duration)
        {
            elapsed += Time.deltaTime;
            float t = elapsed / duration;
            batterySlider.value = Mathf.Lerp(startValue, targetValue, t);
            yield return null;
        }
        
        batterySlider.value = targetValue;
    }
    
    private void UpdateBatterySliderColor()
    {
        if (batterySlider == null) return;
        
        var fillImage = batterySlider.fillRect.GetComponent<UnityEngine.UI.Image>();
        if (fillImage != null)
        {
            float batteryPercentage = currentBattery / maxBattery;
            
            if (batteryPercentage > 0.5f)
            {
                fillImage.color = Color.green;
            }
            else if (batteryPercentage > 0.2f)
            {
                fillImage.color = Color.yellow;
            }
            else
            {
                fillImage.color = Color.red;
            }
        }
    }
    
    public bool HasEnoughBattery(float requiredAmount = 1f)
    {
        return currentBattery >= requiredAmount;
    }
    
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
    
    public void SetBattery(float amount)
    {
        currentBattery = Mathf.Clamp(amount, 0f, maxBattery);
        UpdateBatterySlider();
    }
    
    public void SetDrainRate(float newRate)
    {
        batteryDrainRate = newRate;
    }
    
    public void ResetDrainRate()
    {
        ApplyDifficultySettings();
    }
}