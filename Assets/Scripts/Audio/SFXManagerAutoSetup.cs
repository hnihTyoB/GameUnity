using UnityEngine;


public class SFXManagerAutoSetup : MonoBehaviour
{
    private void Awake()
    {
        if (SFXManager.Instance == null)
        {
            SFXManager existingManager = FindObjectOfType<SFXManager>();
            if (existingManager == null)
            {
                GameObject sfxManagerObj = new GameObject("SFX Manager");
                SFXManager sfxManager = sfxManagerObj.AddComponent<SFXManager>();
                
            }
        }
    }
}

