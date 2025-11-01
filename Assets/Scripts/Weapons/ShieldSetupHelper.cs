using UnityEngine;

#if UNITY_EDITOR
using UnityEditor;
#endif

/// <summary>
/// Helper script to automatically create ShieldManager in scene
/// This will run in Editor to ensure ShieldManager always exists
/// </summary>
[ExecuteInEditMode]
public class ShieldSetupHelper : MonoBehaviour
{
#if UNITY_EDITOR
    [MenuItem("Tools/Setup Shield Manager")]
    private static void SetupShieldManager()
    {
        // Check if ShieldManager already exists
        ShieldManager existingManager = FindObjectOfType<ShieldManager>();
        
        if (existingManager != null)
        {
            Debug.Log("ShieldManager already exists in scene!");
            Selection.activeGameObject = existingManager.gameObject;
            return;
        }
        
        // Create new GameObject with ShieldManager
        GameObject managerObj = new GameObject("ShieldManager");
        managerObj.AddComponent<ShieldManager>();
        
        Debug.Log("ShieldManager created successfully!");
        Selection.activeGameObject = managerObj;
    }
    
    [MenuItem("Tools/Setup Shield Cooldown UI on Selected Slot")]
    private static void SetupShieldCooldownUI()
    {
        GameObject selected = Selection.activeGameObject;
        
        if (selected == null)
        {
            EditorUtility.DisplayDialog("No Selection", 
                "Please select the Shield Inventory Slot GameObject first!", 
                "OK");
            return;
        }
        
        // Check if it has InventorySlot component
        InventorySlot inventorySlot = selected.GetComponent<InventorySlot>();
        if (inventorySlot == null)
        {
            EditorUtility.DisplayDialog("Invalid Selection", 
                "Selected GameObject must have InventorySlot component!", 
                "OK");
            return;
        }
        
        // Check if CooldownUI already exists
        WeaponCooldownUI existingUI = selected.GetComponentInChildren<WeaponCooldownUI>();
        if (existingUI != null)
        {
            EditorUtility.DisplayDialog("Already Setup", 
                "This slot already has WeaponCooldownUI!", 
                "OK");
            return;
        }
        
        // Create CooldownUI structure
        GameObject cooldownUIObj = new GameObject("CooldownUI");
        cooldownUIObj.transform.SetParent(selected.transform);
        RectTransform cooldownUIRect = cooldownUIObj.AddComponent<RectTransform>();
        WeaponCooldownUI cooldownUI = cooldownUIObj.AddComponent<WeaponCooldownUI>();
        
        // Set RectTransform to stretch
        cooldownUIRect.anchorMin = Vector2.zero;
        cooldownUIRect.anchorMax = Vector2.one;
        cooldownUIRect.offsetMin = Vector2.zero;
        cooldownUIRect.offsetMax = Vector2.zero;
        cooldownUIRect.localScale = Vector3.one;
        
        Debug.Log($"Created CooldownUI on {selected.name}. Now manually add CooldownOverlay and CooldownText!");
        
        EditorUtility.DisplayDialog("Partial Setup Complete", 
            "CooldownUI GameObject created!\n\n" +
            "Please manually add:\n" +
            "1. CooldownOverlay (UI Image)\n" +
            "2. CooldownText (TextMeshPro)\n\n" +
            "See SHIELD_COOLDOWN_SETUP.md for details.", 
            "OK");
        
        Selection.activeGameObject = cooldownUIObj;
    }
#endif
}

