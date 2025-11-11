using UnityEngine;

/// <summary>
/// Automatically creates SFXManager if it doesn't exist in the scene
/// Attach this to Managers prefab or any persistent GameObject
/// </summary>
public class SFXManagerAutoSetup : MonoBehaviour
{
    private void Awake()
    {
        // Check if SFXManager already exists
        if (SFXManager.Instance == null)
        {
            // Try to find existing SFXManager in scene
            SFXManager existingManager = FindObjectOfType<SFXManager>();
            if (existingManager == null)
            {
                // Create SFXManager GameObject
                GameObject sfxManagerObj = new GameObject("SFX Manager");
                SFXManager sfxManager = sfxManagerObj.AddComponent<SFXManager>();
                
                Debug.Log("SFXManagerAutoSetup: Created SFXManager automatically. Please assign audio clips in Inspector!");
            }
        }
    }
}

