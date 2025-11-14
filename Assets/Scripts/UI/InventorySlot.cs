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
        // Wait until ShieldManager instance is available
        int waitFrames = 0;
        while (ShieldManager.Instance == null)
        {
            waitFrames++;
            yield return null;
        }
        
        Debug.Log($"InventorySlot ({gameObject.name}): ShieldManager found after {waitFrames} frames, registering UI");
        
        // Register this cooldown UI with ShieldManager
        ShieldManager.Instance.RegisterShieldCooldownUI(cooldownUI);
        
        Debug.Log($"InventorySlot ({gameObject.name}): Registration complete");
    }

    public WeaponInfo GetWeaponInfo() {
        return weaponInfo;
    }
    
    public WeaponCooldownUI GetCooldownUI()
    {
        return cooldownUI;
    }
}
