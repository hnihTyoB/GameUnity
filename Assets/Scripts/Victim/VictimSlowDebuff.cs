using System.Collections;
using UnityEngine;

public class VictimSlowDebuff : MonoBehaviour
{
    [Header("Slow Settings")]
    [SerializeField] private float slowMultiplier = 0.5f; 
    [SerializeField] private float slowDuration = 2.5f; 
    
    private Victim victim;
    private bool isSlowed = false;
    private float originalMoveSpeed;
    private Coroutine slowCoroutine;
    
    private void Awake()
    {
        victim = GetComponent<Victim>();
    }
  
    public void ApplySlow(float originalSpeed)
    {
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
        
    
        if (victim != null)
        {
            victim.SetMoveSpeed(originalMoveSpeed * slowMultiplier);
        }
        
     
        yield return new WaitForSeconds(slowDuration);
        
       
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
       
        if (slowCoroutine != null)
        {
            StopCoroutine(slowCoroutine);
        }
    }
}

