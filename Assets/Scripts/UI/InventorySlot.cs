using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InventorySlot : MonoBehaviour
{
    [SerializeField] private WeaponInfo weaponInfo;
    [SerializeField] private bool isShieldSlot = false; // Mark if this is shield slot
    
    private WeaponCooldownUI cooldownUI;

    private void Awake()
    {
        // Get cooldown UI component if exists
        cooldownUI = GetComponentInChildren<WeaponCooldownUI>();
        
        Debug.Log($"InventorySlot ({gameObject.name}): Awake - isShieldSlot={isShieldSlot}, cooldownUI={cooldownUI != null}");
        
        // If this is shield slot, register with ShieldManager
        if (isShieldSlot && cooldownUI != null)
        {
            Debug.Log($"InventorySlot ({gameObject.name}): Starting registration coroutine");
            // Wait for ShieldManager to be ready
            StartCoroutine(RegisterShieldUIWhenReady());
        }
        else if (isShieldSlot && cooldownUI == null)
        {
            Debug.LogError($"InventorySlot ({gameObject.name}): This is shield slot but WeaponCooldownUI not found!");
        }
    }
    
    private IEnumerator RegisterShieldUIWhenReady()
    {
        // Wait a bit for scene to fully load and Singleton conflicts to resolve
        yield return new WaitForSeconds(0.2f);
        
        // Wait until ShieldManager instance is available
        int waitFrames = 0;
        while (ShieldManager.Instance == null)
        {
            waitFrames++;
            yield return null;
            
            if (waitFrames > 100)
            {
                Debug.LogError($"InventorySlot ({gameObject.name}): ShieldManager still null after 100 frames!");
                yield break;
            }
        }
        
        Debug.Log($"InventorySlot ({gameObject.name}): ShieldManager found after {waitFrames} frames, registering UI");
        Debug.Log($"InventorySlot ({gameObject.name}): cooldownUI valid = {(cooldownUI != null)}");
        
        // Register this cooldown UI with ShieldManager
        if (cooldownUI != null && ShieldManager.Instance != null)
        {
            ShieldManager.Instance.RegisterShieldCooldownUI(cooldownUI);
            Debug.Log($"InventorySlot ({gameObject.name}): Registration complete");
        }
        else
        {
            Debug.LogError($"InventorySlot ({gameObject.name}): Cannot register - cooldownUI or ShieldManager is null!");
        }
    }

    public WeaponInfo GetWeaponInfo() {
        return weaponInfo;
    }
    
    public WeaponCooldownUI GetCooldownUI()
    {
        return cooldownUI;
    }
}
