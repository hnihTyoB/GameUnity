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
        
        // If this is shield slot, register with ShieldManager
        if (isShieldSlot && cooldownUI != null)
        {
            // Wait for ShieldManager to be ready
            StartCoroutine(RegisterShieldUIWhenReady());
        }
    }
    
    private IEnumerator RegisterShieldUIWhenReady()
    {
        // Wait until ShieldManager instance is available
        while (ShieldManager.Instance == null)
        {
            yield return null;
        }
        
        // Register this cooldown UI with ShieldManager
        ShieldManager.Instance.RegisterShieldCooldownUI(cooldownUI);
    }

    public WeaponInfo GetWeaponInfo() {
        return weaponInfo;
    }
    
    public WeaponCooldownUI GetCooldownUI()
    {
        return cooldownUI;
    }
}
