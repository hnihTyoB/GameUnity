using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InventorySlot : MonoBehaviour
{
    [SerializeField] private WeaponInfo weaponInfo;
    [SerializeField] private bool isShieldSlot = false;
    
    private WeaponCooldownUI cooldownUI;

    private void Awake()
    {
       
        cooldownUI = GetComponentInChildren<WeaponCooldownUI>();
        
        Debug.Log($"InventorySlot ({gameObject.name}): Awake - isShieldSlot={isShieldSlot}, cooldownUI={cooldownUI != null}");
        
       
        if (isShieldSlot && cooldownUI != null)
        {
            Debug.Log($"InventorySlot ({gameObject.name}): Starting registration coroutine");
            StartCoroutine(RegisterShieldUIWhenReady());
        }
        else if (isShieldSlot && cooldownUI == null)
        {
            Debug.LogError($"InventorySlot ({gameObject.name}): This is shield slot but WeaponCooldownUI not found!");
        }
    }
    
    private IEnumerator RegisterShieldUIWhenReady()
    {
        yield return new WaitForSeconds(0.2f);
        
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
