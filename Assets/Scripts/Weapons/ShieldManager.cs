using UnityEngine;

/// <summary>
/// Singleton manager for Shield system
/// Handles communication between Shield weapon and UI
/// Persists cooldown state across scenes
/// </summary>
public class ShieldManager : Singleton<ShieldManager>
{
    private WeaponCooldownUI shieldCooldownUI;
    
    // Persistent cooldown state
    private bool isOnCooldown = false;
    private float cooldownEndTime = 0f;
    
    protected override void Awake()
    {
        base.Awake();
        
        // Subscribe to scene loaded event to reset UI reference
        UnityEngine.SceneManagement.SceneManager.sceneLoaded += OnSceneLoaded;
    }
    
    private void OnDestroy()
    {
        // Unsubscribe from scene loaded event
        UnityEngine.SceneManagement.SceneManager.sceneLoaded -= OnSceneLoaded;
    }
    
    private void Update()
    {
        // Check if cooldown has expired
        if (isOnCooldown && Time.time >= cooldownEndTime)
        {
            isOnCooldown = false;
            Debug.Log("ShieldManager: Cooldown expired");
        }
    }
    
    /// <summary>
    /// Reset UI reference when new scene loads (new UICanvas with new WeaponCooldownUI)
    /// </summary>
    private void OnSceneLoaded(UnityEngine.SceneManagement.Scene scene, UnityEngine.SceneManagement.LoadSceneMode mode)
    {
        Debug.Log($"ShieldManager: OnSceneLoaded - Scene: {scene.name}, clearing UI reference");
        shieldCooldownUI = null; // Clear old reference, will be re-registered by new UI
        
        // Reset cooldown state when starting new game (Scene1)
        if (scene.name == "Scene1")
        {
            isOnCooldown = false;
            cooldownEndTime = 0f;
            Debug.Log("ShieldManager: Reset cooldown state for new game");
        }
    }
    
    /// <summary>
    /// Register the cooldown UI for shield
    /// Called by WeaponCooldownUI on shield slot
    /// </summary>
    public void RegisterShieldCooldownUI(WeaponCooldownUI cooldownUI)
    {
        shieldCooldownUI = cooldownUI;
        Debug.Log("ShieldManager: Registered Shield Cooldown UI");
        
        // If shield is on cooldown, sync the new UI
        if (isOnCooldown)
        {
            float remainingCooldown = GetRemainingCooldown();
            if (remainingCooldown > 0f)
            {
                Debug.Log($"ShieldManager: Syncing UI with {remainingCooldown:F1}s remaining cooldown");
                shieldCooldownUI.StartCooldown(remainingCooldown);
            }
        }
    }
    
    /// <summary>
    /// Called when shield is activated
    /// </summary>
    public void OnShieldActivated()
    {
        Debug.Log("ShieldManager: Shield Activated");
        // Could add additional logic here (sound effects, etc.)
    }
    
    /// <summary>
    /// Called when shield starts cooldown
    /// </summary>
    public void OnShieldCooldownStarted(float cooldownDuration)
    {
        Debug.Log($"ShieldManager: Shield Cooldown Started - {cooldownDuration}s");
        
        // Store cooldown state
        isOnCooldown = true;
        cooldownEndTime = Time.time + cooldownDuration;
        
        if (shieldCooldownUI != null)
        {
            shieldCooldownUI.StartCooldown(cooldownDuration);
        }
        else
        {
            Debug.LogWarning("ShieldManager: No cooldown UI registered!");
        }
    }
    
    /// <summary>
    /// Check if shield is currently on cooldown
    /// </summary>
    public bool IsOnCooldown()
    {
        return isOnCooldown && Time.time < cooldownEndTime;
    }
    
    /// <summary>
    /// Get remaining cooldown time
    /// </summary>
    public float GetRemainingCooldown()
    {
        if (!isOnCooldown) return 0f;
        return Mathf.Max(0f, cooldownEndTime - Time.time);
    }
}

