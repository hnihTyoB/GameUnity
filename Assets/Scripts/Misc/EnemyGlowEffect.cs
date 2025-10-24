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
    
    private void Start()
    {
        // Get parent enemy's sprite renderer
        enemySprite = GetComponentInParent<SpriteRenderer>();
        
        if (enemySprite != null)
        {
            originalColor = enemySprite.color;
            Debug.Log($"EnemyGlowEffect: Saved original color for {enemySprite.gameObject.name}: {originalColor}");
            
            // Create glow material if needed
            if (enablePulse)
            {
                StartCoroutine(PulseGlow());
            }
            else
            {
                ApplyStaticGlow();
            }
        }
        else
        {
            Debug.LogError("EnemyGlowEffect: Could not find parent SpriteRenderer!");
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
        // Stop all coroutines first
        StopAllCoroutines();
        
        // Restore original color when glow is removed
        if (enemySprite != null)
        {
            Debug.Log($"Restoring color for {enemySprite.gameObject.name} from {enemySprite.color} to {originalColor}");
            enemySprite.color = originalColor;
        }
        else
        {
            Debug.LogWarning("EnemyGlowEffect.OnDestroy: enemySprite is null!");
        }
    }
    
    private void OnDisable()
    {
        // Also restore when disabled (safety)
        if (enemySprite != null)
        {
            enemySprite.color = originalColor;
        }
    }
}

