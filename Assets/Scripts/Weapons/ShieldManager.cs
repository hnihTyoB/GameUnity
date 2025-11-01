using UnityEngine;

/// <summary>
/// Singleton manager for Shield system
/// Handles communication between Shield weapon and UI
/// </summary>
public class ShieldManager : Singleton<ShieldManager>
{
    private WeaponCooldownUI shieldCooldownUI;
    
    protected override void Awake()
    {
        base.Awake();
    }
    
    /// <summary>
    /// Register the cooldown UI for shield
    /// Called by WeaponCooldownUI on shield slot
    /// </summary>
    public void RegisterShieldCooldownUI(WeaponCooldownUI cooldownUI)
    {
        shieldCooldownUI = cooldownUI;
        Debug.Log("ShieldManager: Registered Shield Cooldown UI");
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
        
        if (shieldCooldownUI != null)
        {
            shieldCooldownUI.StartCooldown(cooldownDuration);
        }
        else
        {
            Debug.LogWarning("ShieldManager: No cooldown UI registered!");
        }
    }
}

