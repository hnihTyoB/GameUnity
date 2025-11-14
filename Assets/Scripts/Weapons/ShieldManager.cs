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
    /// Handle scene loaded event
    /// Don't clear UI reference - let new UI override when it registers
    /// </summary>
    private void OnSceneLoaded(UnityEngine.SceneManagement.Scene scene, UnityEngine.SceneManagement.LoadSceneMode mode)
    {
        Debug.Log($"ShieldManager: OnSceneLoaded - Scene: {scene.name}, Mode: {mode}");
        Debug.Log($"ShieldManager: Current shieldCooldownUI = {(shieldCooldownUI != null ? "EXISTS" : "NULL")}");
        
        // DON'T clear shieldCooldownUI here!
        // New InventorySlot will register and override it automatically
        // This prevents timing issues where OnSceneLoaded runs after registration
        
        // Reset cooldown state when starting new game (Scene1)
        if (scene.name == "Scene1")
        {
            isOnCooldown = false;
            cooldownEndTime = 0f;
            shieldCooldownUI = null; // Clear UI when restarting game
            Debug.Log("ShieldManager: Reset cooldown state and UI for new game");
        }
        else
        {
            Debug.Log($"ShieldManager: Keeping cooldown state - isOnCooldown={isOnCooldown}, remaining={GetRemainingCooldown():F1}s");
            Debug.Log($"ShieldManager: Waiting for new UI to register...");
        }
    }
    
    /// <summary>
    /// Register the cooldown UI for shield
    /// Called by WeaponCooldownUI on shield slot
    /// </summary>
    public void RegisterShieldCooldownUI(WeaponCooldownUI cooldownUI)
    {
        Debug.Log($"ShieldManager: RegisterShieldCooldownUI called - cooldownUI={cooldownUI != null}, this={this != null}");
        
        shieldCooldownUI = cooldownUI;
        Debug.Log($"ShieldManager: Registered Shield Cooldown UI - shieldCooldownUI now = {(shieldCooldownUI != null ? "SET" : "NULL")}");
        
        // If shield is on cooldown, sync the new UI
        if (isOnCooldown)
        {
            float remainingCooldown = GetRemainingCooldown();
            if (remainingCooldown > 0f)
            {
                Debug.Log($"ShieldManager: Syncing UI with {remainingCooldown:F1}s remaining cooldown");
                if (shieldCooldownUI != null)
                {
                    shieldCooldownUI.StartCooldown(remainingCooldown);
                }
                else
                {
                    Debug.LogError("ShieldManager: Cannot sync - shieldCooldownUI is NULL!");
                }
            }
        }
        else
        {
            Debug.Log("ShieldManager: No active cooldown to sync");
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
        Debug.Log($"ShieldManager: shieldCooldownUI = {(shieldCooldownUI != null ? "EXISTS" : "NULL")}");
        
        // Store cooldown state
        isOnCooldown = true;
        cooldownEndTime = Time.time + cooldownDuration;
        
        if (shieldCooldownUI != null)
        {
            Debug.Log($"ShieldManager: Calling shieldCooldownUI.StartCooldown({cooldownDuration}s)");
            shieldCooldownUI.StartCooldown(cooldownDuration);
        }
        else
        {
            Debug.LogWarning("ShieldManager: No cooldown UI registered! Cannot display cooldown.");
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

