using System.Collections;
using UnityEngine;

public class SlowDebuff : Singleton<SlowDebuff>
{
    [Header("Slow Effect Settings")]
    [SerializeField] private float slowPercentage = 0.4f; // 40% slower
    [SerializeField] private float slowDuration = 2.5f;
    
    [Header("Visual Feedback")]
    [SerializeField] private GameObject slowAuraEffectPrefab;
    
    private bool isSlowed = false;
    private Coroutine slowCoroutine;
    private GameObject spawnedAuraEffect; // Instance of the aura

    protected override void Awake()
    {
        base.Awake();
    }

    private void Start()
    {
        // Instantiate aura effect as child if prefab is assigned
        if (slowAuraEffectPrefab != null && spawnedAuraEffect == null)
        {
            spawnedAuraEffect = Instantiate(slowAuraEffectPrefab, transform);
            spawnedAuraEffect.transform.localPosition = Vector3.zero;
            spawnedAuraEffect.SetActive(false);
        }
    }

    public void ApplySlow()
    {
        // Don't stack slow effects - just refresh duration
        if (isSlowed && slowCoroutine != null)
        {
            StopCoroutine(slowCoroutine);
        }
        
        slowCoroutine = StartCoroutine(SlowEffectRoutine());
    }

    private IEnumerator SlowEffectRoutine()
    {
        if (!isSlowed)
        {
            // Apply slow effect
            isSlowed = true;
            PlayerController.Instance.ApplySlowEffect(slowPercentage);
            
            // Show visual feedback
            ShowSlowVisuals(true);
        }
        
        // Wait for duration
        yield return new WaitForSeconds(slowDuration);
        
        // Remove slow effect
        isSlowed = false;
        PlayerController.Instance.RemoveSlowEffect();
        
        // Hide visual feedback
        ShowSlowVisuals(false);
    }

    private void ShowSlowVisuals(bool show)
    {
        // Show/hide aura effect only
        if (spawnedAuraEffect != null)
        {
            spawnedAuraEffect.SetActive(show);
        }
    }
    
    private void OnDestroy()
    {
        // Clean up spawned aura
        if (spawnedAuraEffect != null)
        {
            Destroy(spawnedAuraEffect);
        }
    }

    public bool IsSlowed()
    {
        return isSlowed;
    }

    public float GetSlowPercentage()
    {
        return slowPercentage;
    }
}

