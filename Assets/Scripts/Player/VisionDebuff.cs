using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;

public class VisionDebuff : Singleton<VisionDebuff>
{
    [Header("Vision Reduction Settings")]
    [SerializeField] private float visionDuration = 2.5f; // Same as slow debuff
    [SerializeField] private float vignettePerStack = 0.15f; // Vignette intensity increase per shadow
    [SerializeField] private float maxVignette = 0.7f; // Maximum vignette intensity
    
    [Header("Visual Feedback - Post-Processing Vignette")]
    [SerializeField] private GameObject isolationAuraEffectPrefab;
    [SerializeField] private Volume postProcessVolume; // Post-processing volume with vignette
    
    private Vignette vignette;
    private int activeDebuffs = 0;
    private List<Coroutine> activeCoroutines = new List<Coroutine>();
    private float baseVignetteIntensity = 0f;
    private GameObject spawnedAuraEffect; // Instance of the aura

    protected override void Awake()
    {
        base.Awake();
    }

    private void Start()
    {
        // Auto-find Global Volume if not assigned
        if (postProcessVolume == null)
        {
            postProcessVolume = FindObjectOfType<Volume>();
            if (postProcessVolume == null)
            {
                Debug.LogError("VisionDebuff: No Volume found in scene! Please create a Global Volume.");
                return;
            }
            Debug.Log("VisionDebuff: Auto-found Global Volume: " + postProcessVolume.name);
        }
        
        // Get vignette from post-processing volume
        if (postProcessVolume != null && postProcessVolume.profile.TryGet(out Vignette vignetteEffect))
        {
            vignette = vignetteEffect;
            baseVignetteIntensity = vignette.intensity.value;
        }
        else
        {
            Debug.LogWarning("VisionDebuff: No Vignette found in Post-Processing Volume!");
        }
        
        // Instantiate aura effect as child if prefab is assigned
        if (isolationAuraEffectPrefab != null && spawnedAuraEffect == null)
        {
            spawnedAuraEffect = Instantiate(isolationAuraEffectPrefab, transform);
            spawnedAuraEffect.transform.localPosition = Vector3.zero;
            spawnedAuraEffect.SetActive(false);
        }
    }

    public void ApplyVisionReduction()
    {
        // Each shadow applies one independent debuff
        Coroutine newCoroutine = StartCoroutine(VisionReductionRoutine());
        activeCoroutines.Add(newCoroutine);
    }

    private IEnumerator VisionReductionRoutine()
    {
        // Increment active debuffs (each shadow = +1)
        activeDebuffs++;
        
        // Update darkness based on current stack count
        UpdateDarkness();
        
        // Show aura on first debuff
        if (activeDebuffs == 1 && spawnedAuraEffect != null)
        {
            spawnedAuraEffect.SetActive(true);
        }
        
        // Wait for duration
        yield return new WaitForSeconds(visionDuration);
        
        // Decrement active debuffs
        activeDebuffs--;
        
        // Update darkness based on remaining stacks
        UpdateDarkness();
        
        // Hide aura if no more debuffs
        if (activeDebuffs == 0 && spawnedAuraEffect != null)
        {
            spawnedAuraEffect.SetActive(false);
        }
    }

    private void UpdateDarkness()
    {
        if (vignette == null) return;
        
        // Calculate vignette intensity based on stack count
        // Each shadow adds vignettePerStack (default 0.15)
        float targetIntensity = baseVignetteIntensity + (activeDebuffs * vignettePerStack);
        targetIntensity = Mathf.Min(targetIntensity, maxVignette); // Cap at max vignette
        
        // Smoothly transition to new vignette intensity
        StartCoroutine(FadeVignette(targetIntensity));
    }

    private IEnumerator FadeVignette(float targetIntensity)
    {
        if (vignette == null) yield break;
        
        float duration = 0.3f;
        float elapsed = 0f;
        
        float startIntensity = vignette.intensity.value;
        
        while (elapsed < duration)
        {
            elapsed += Time.deltaTime;
            float newIntensity = Mathf.Lerp(startIntensity, targetIntensity, elapsed / duration);
            vignette.intensity.value = newIntensity;
            yield return null;
        }
        
        vignette.intensity.value = targetIntensity;
    }

    public bool IsVisionReduced()
    {
        return activeDebuffs > 0;
    }

    public int GetActiveDebuffCount()
    {
        return activeDebuffs;
    }

    public float GetCurrentVignetteIntensity()
    {
        if (vignette != null)
        {
            return vignette.intensity.value;
        }
        return 0f;
    }
    
    private void OnDestroy()
    {
        // Clean up spawned aura
        if (spawnedAuraEffect != null)
        {
            Destroy(spawnedAuraEffect);
        }
    }
}

