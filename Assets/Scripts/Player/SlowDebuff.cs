using System.Collections;
using UnityEngine;

public class SlowDebuff : Singleton<SlowDebuff>
{
    [Header("Slow Effect Settings")]
    [SerializeField] private float slowPercentage = 0.4f; 
    [SerializeField] private float slowDuration = 2.5f;
    
    [Header("Visual Feedback")]
    [SerializeField] private GameObject slowAuraEffectPrefab;
    
    private bool isSlowed = false;
    private Coroutine slowCoroutine;
    private GameObject spawnedAuraEffect; 

    protected override void Awake()
    {
        base.Awake();
    }

    private void Start()
    {
        if (slowAuraEffectPrefab != null && spawnedAuraEffect == null)
        {
            spawnedAuraEffect = Instantiate(slowAuraEffectPrefab, transform);
            spawnedAuraEffect.transform.localPosition = Vector3.zero;
            spawnedAuraEffect.SetActive(false);
        }
    }

    public void ApplySlow()
    {
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
            isSlowed = true;
            PlayerController.Instance.ApplySlowEffect(slowPercentage);
            
            ShowSlowVisuals(true);
        }
        
        yield return new WaitForSeconds(slowDuration);
        
        isSlowed = false;
        PlayerController.Instance.RemoveSlowEffect();
        
        ShowSlowVisuals(false);
    }

    private void ShowSlowVisuals(bool show)
    {
        if (spawnedAuraEffect != null)
        {
            spawnedAuraEffect.SetActive(show);
        }
    }
    
    private void OnDestroy()
    {
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

