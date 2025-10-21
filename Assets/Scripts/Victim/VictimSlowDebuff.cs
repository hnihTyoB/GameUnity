using System.Collections;
using UnityEngine;

/// <summary>
/// Manages slow debuff effect on Victim when attacked by Shadow enemies
/// </summary>
public class VictimSlowDebuff : MonoBehaviour
{
    [Header("Slow Settings")]
    [SerializeField] private float slowMultiplier = 0.5f; // Reduce speed to 50%
    [SerializeField] private float slowDuration = 2.5f; // Duration of slow effect
    
    private Victim victim;
    private bool isSlowed = false;
    private float originalMoveSpeed;
    private Coroutine slowCoroutine;
    
    private void Awake()
    {
        victim = GetComponent<Victim>();
    }
    
    /// <summary>
    /// Apply slow effect to this victim
    /// </summary>
    public void ApplySlow(float originalSpeed)
    {
        // If already slowed, restart the duration
        if (isSlowed && slowCoroutine != null)
        {
            StopCoroutine(slowCoroutine);
        }
        
        originalMoveSpeed = originalSpeed;
        slowCoroutine = StartCoroutine(SlowRoutine());
    }
    
    private IEnumerator SlowRoutine()
    {
        isSlowed = true;
        
        // Apply slow
        if (victim != null)
        {
            victim.SetMoveSpeed(originalMoveSpeed * slowMultiplier);
        }
        
        // Wait for duration
        yield return new WaitForSeconds(slowDuration);
        
        // Remove slow
        if (victim != null)
        {
            victim.SetMoveSpeed(originalMoveSpeed);
        }
        
        isSlowed = false;
    }
    
    public bool IsSlowed()
    {
        return isSlowed;
    }
    
    private void OnDestroy()
    {
        // Clean up coroutine
        if (slowCoroutine != null)
        {
            StopCoroutine(slowCoroutine);
        }
    }
}

