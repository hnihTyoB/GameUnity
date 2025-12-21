using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;

public class VisionDebuff : Singleton<VisionDebuff>
{
    [Header("Vision Reduction Settings")]
    [SerializeField] private float visionDuration = 2.5f; 
    [SerializeField] private float vignettePerStack = 0.15f; 
    [SerializeField] private float maxVignette = 0.7f; 
    
    [Header("Visual Feedback - Post-Processing Vignette")]
    [SerializeField] private GameObject isolationAuraEffectPrefab;
    [SerializeField] private Volume postProcessVolume; 
    
    private Vignette vignette;
    private int activeDebuffs = 0;
    private List<Coroutine> activeCoroutines = new List<Coroutine>();
    private float baseVignetteIntensity = 0f;
    private GameObject spawnedAuraEffect; 

    protected override void Awake()
    {
        base.Awake();
    }

    private void Start()
    {
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
        
        if (postProcessVolume != null && postProcessVolume.profile.TryGet(out Vignette vignetteEffect))
        {
            vignette = vignetteEffect;
            baseVignetteIntensity = vignette.intensity.value;
        }
        else
        {
            Debug.LogWarning("VisionDebuff: No Vignette found in Post-Processing Volume!");
        }
        
        if (isolationAuraEffectPrefab != null && spawnedAuraEffect == null)
        {
            spawnedAuraEffect = Instantiate(isolationAuraEffectPrefab, transform);
            spawnedAuraEffect.transform.localPosition = Vector3.zero;
            spawnedAuraEffect.SetActive(false);
        }
    }

    public void ApplyVisionReduction()
    {
        Coroutine newCoroutine = StartCoroutine(VisionReductionRoutine());
        activeCoroutines.Add(newCoroutine);
    }

    private IEnumerator VisionReductionRoutine()
    {
        activeDebuffs++;
        
        UpdateDarkness();
        
        if (activeDebuffs == 1 && spawnedAuraEffect != null)
        {
            spawnedAuraEffect.SetActive(true);
        }
        
        yield return new WaitForSeconds(visionDuration);
        
        activeDebuffs--;
        
        UpdateDarkness();
        
        if (activeDebuffs == 0 && spawnedAuraEffect != null)
        {
            spawnedAuraEffect.SetActive(false);
        }
    }

    private void UpdateDarkness()
    {
        if (vignette == null) return;
        
        float targetIntensity = baseVignetteIntensity + (activeDebuffs * vignettePerStack);
        targetIntensity = Mathf.Min(targetIntensity, maxVignette);
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
        if (spawnedAuraEffect != null)
        {
            Destroy(spawnedAuraEffect);
        }
    }
}

