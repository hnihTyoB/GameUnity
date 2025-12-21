using UnityEngine;

public class ShieldManager : Singleton<ShieldManager>
{
    private WeaponCooldownUI shieldCooldownUI;
    
    private bool isOnCooldown = false;
    private float cooldownEndTime = 0f;
    
    protected override void Awake()
    {
        base.Awake();
        

        UnityEngine.SceneManagement.SceneManager.sceneLoaded += OnSceneLoaded;
    }
    
    private void OnDestroy()
    {

        UnityEngine.SceneManagement.SceneManager.sceneLoaded -= OnSceneLoaded;
    }
    
    private void Update()
    {
        if (isOnCooldown && Time.time >= cooldownEndTime)
        {
            isOnCooldown = false;
            Debug.Log("ShieldManager: Cooldown expired");
        }
    }
    
    private void OnSceneLoaded(UnityEngine.SceneManagement.Scene scene, UnityEngine.SceneManagement.LoadSceneMode mode)
    {
        Debug.Log($"ShieldManager: OnSceneLoaded - Scene: {scene.name}, Mode: {mode}");
        Debug.Log($"ShieldManager: Current shieldCooldownUI = {(shieldCooldownUI != null ? "EXISTS" : "NULL")}");
        
 
        if (scene.name == "Scene1")
        {
            isOnCooldown = false;
            cooldownEndTime = 0f;
            shieldCooldownUI = null; 
            Debug.Log("ShieldManager: Reset cooldown state and UI for new game");
        }
        else
        {
            Debug.Log($"ShieldManager: Keeping cooldown state - isOnCooldown={isOnCooldown}, remaining={GetRemainingCooldown():F1}s");
            Debug.Log($"ShieldManager: Waiting for new UI to register...");
        }
    }
 
    public void RegisterShieldCooldownUI(WeaponCooldownUI cooldownUI)
    {
        Debug.Log($"ShieldManager: RegisterShieldCooldownUI called - cooldownUI={cooldownUI != null}, this={this != null}");
        
        shieldCooldownUI = cooldownUI;
        Debug.Log($"ShieldManager: Registered Shield Cooldown UI - shieldCooldownUI now = {(shieldCooldownUI != null ? "SET" : "NULL")}");
        
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
    
    public void OnShieldActivated()
    {
        Debug.Log("ShieldManager: Shield Activated");
        
    }
    
    public void OnShieldCooldownStarted(float cooldownDuration)
    {
        Debug.Log($"ShieldManager: Shield Cooldown Started - {cooldownDuration}s");
        Debug.Log($"ShieldManager: shieldCooldownUI = {(shieldCooldownUI != null ? "EXISTS" : "NULL")}");
    
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
    
    public bool IsOnCooldown()
    {
        return isOnCooldown && Time.time < cooldownEndTime;
    }
    
    public float GetRemainingCooldown()
    {
        if (!isOnCooldown) return 0f;
        return Mathf.Max(0f, cooldownEndTime - Time.time);
    }
}

