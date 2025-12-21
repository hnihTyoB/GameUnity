using System.Collections;
using UnityEngine;


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
      
        enemySprite = GetComponentInParent<SpriteRenderer>();
        
        if (enemySprite != null)
        {
        
            originalColor = enemySprite.color;
        }
        else
        {
            Debug.LogError("[GLOW] Could not find parent SpriteRenderer!");
        }
    }
    
    private void Start()
    {
       
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
            float pulse = (Mathf.Sin(Time.time * pulseSpeed) + 1f) / 2f; 
            float currentIntensity = Mathf.Lerp(0.8f, glowIntensity, pulse);
            
            enemySprite.color = glowColor * currentIntensity;
            
            yield return null;
        }
    }
    
    private void OnDestroy()
    {
  
        StopAllCoroutines();
        
    
        if (enemySprite != null)
        {
            enemySprite.color = originalColor;
            
           
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
   
        StopAllCoroutines();
        

        if (enemySprite != null)
        {
            enemySprite.color = originalColor;
        }
    }
}

