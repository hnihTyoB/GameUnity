using System.Collections;
using UnityEngine;

/// <summary>
/// Visual glow effect for enemies affected by flashlight
/// </summary>
public class EnemyGlowEffect : MonoBehaviour
{
    [Header("Glow Settings")]
    [SerializeField] private Color glowColor = Color.yellow;
    [SerializeField] private float glowIntensity = 1.5f;
    [SerializeField] private float pulseSpeed = 2f;
    [SerializeField] private bool enablePulse = true;
    
    private SpriteRenderer enemySprite;
    private Color originalColor;
    private Material glowMaterial;
    
    private void Awake()
    {
        // CRITICAL: Get and save original color in Awake (before Start modifies it)
        enemySprite = GetComponentInParent<SpriteRenderer>();
        
        if (enemySprite != null)
        {
            // Save ORIGINAL color BEFORE any modifications
            originalColor = enemySprite.color;
            Debug.Log($"[GLOW] Saved ORIGINAL color for {enemySprite.gameObject.name}: {originalColor}");
        }
        else
        {
            Debug.LogError("[GLOW] Could not find parent SpriteRenderer!");
        }
    }
    
    private void Start()
    {
        // Now apply glow effects (originalColor already saved in Awake)
        if (enemySprite != null)
        {
            if (enablePulse)
            {
                StartCoroutine(PulseGlow());
            }
            else
            {
                ApplyStaticGlow();
            }
        }
    }
    
    private void ApplyStaticGlow()
    {
        if (enemySprite != null)
        {
            enemySprite.color = glowColor * glowIntensity;
        }
    }
    
    private IEnumerator PulseGlow()
    {
        while (enemySprite != null && this != null)
        {
            float pulse = (Mathf.Sin(Time.time * pulseSpeed) + 1f) / 2f; // 0 to 1
            float currentIntensity = Mathf.Lerp(0.8f, glowIntensity, pulse);
            
            enemySprite.color = glowColor * currentIntensity;
            
            yield return null;
        }
    }
    
    private void OnDestroy()
    {
        // Stop all coroutines IMMEDIATELY
        StopAllCoroutines();
        
        // Restore original color when glow is removed
        if (enemySprite != null)
        {
            Debug.Log($"[GLOW] Restoring {enemySprite.gameObject.name} from CURRENT={enemySprite.color} to ORIGINAL={originalColor}");
            enemySprite.color = originalColor;
            
            // Verify restoration
            if (enemySprite.color != originalColor)
            {
                Debug.LogError($"[GLOW] ❌ COLOR RESTORE FAILED! Expected {originalColor} but got {enemySprite.color}");
            }
            else
            {
                Debug.Log($"[GLOW] ✅ Color successfully restored to {originalColor}");
            }
        }
        else
        {
            Debug.LogWarning("[GLOW] OnDestroy: enemySprite is null!");
        }
    }
    
    private void OnDisable()
    {
        // Stop coroutines when disabled too
        StopAllCoroutines();
        
        // Also restore when disabled (safety)
        if (enemySprite != null)
        {
            Debug.Log($"[GLOW] OnDisable: Restoring {enemySprite.gameObject.name} to {originalColor}");
            enemySprite.color = originalColor;
        }
    }
}

