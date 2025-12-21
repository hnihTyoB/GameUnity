using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FlashlightEffect : MonoBehaviour
{
    [Header("Flashlight Settings")]
    [SerializeField] private FlashlightCone flashlightCone; 
    [SerializeField] private float detectionAngle = 45f; 
    [SerializeField] private LayerMask enemyLayer; 
    
    private float detectionRange; 
    
    [Header("Effect Timings")]
    [SerializeField] private float slowTime = 0.5f; 
    [SerializeField] private float stunTime = 2.0f; 
    [SerializeField] private float killTime = 6.0f; 
    [SerializeField] private float cooldownTime = 5.0f;
    
    [Header("Effect Strengths")]
    [SerializeField] private float slowMultiplier = 0.5f; 
    [SerializeField] private float slowDuration = 3.0f; 
    [SerializeField] private float stunDuration = 2.5f; 
    
    [Header("Battery Costs")]
    [SerializeField] private float normalDrainRate = 0.2f; 
    [SerializeField] private float slowDrainRate = 1.9f; 
    [SerializeField] private float stunDrainRate = 0.8f; 
    [SerializeField] private float killDrainRate = 1.9f;
    
    [Header("Visual Effects")]
    [SerializeField] private GameObject slowGlowPrefab; 
    [SerializeField] private GameObject stunGlowPrefab; 
    [SerializeField] private GameObject killGlowPrefab; 
    
    private Dictionary<EnemyAI, ShadowExposureData> exposedShadows = new Dictionary<EnemyAI, ShadowExposureData>();
    private bool isFlashlightOn = false;
    
    private float baseSlowTime;
    private float baseStunTime;
    private float baseKillTime;
    private float baseCooldownTime;
    private float baseSlowDuration;
    private float baseStunDuration;
    private float baseNormalDrainRate;
    private float baseSlowDrainRate;
    private float baseStunDrainRate;
    private float baseKillDrainRate;
    
    private class ShadowExposureData
    {
        public float exposureTime = 0f;
        public ExposureState currentState = ExposureState.None;
        public float cooldownTimer = 0f;
        public bool isStunned = false;
        public GameObject currentGlow = null;
        public Coroutine stunCoroutine = null;
        public Coroutine slowDurationCoroutine = null; 
        public bool isSlowPersisting = false; 
    }
    
    private enum ExposureState
    {
        None,
        Slow,
        Stun,
        Killing
    }
    
    private void Awake()
    {
        if (flashlightCone == null)
        {
            flashlightCone = GetComponent<FlashlightCone>();
        }
        
  
        if (flashlightCone != null)
        {
            detectionRange = flashlightCone.GetConeLength();
        }
        
        baseSlowTime = slowTime;
        baseStunTime = stunTime;
        baseKillTime = killTime;
        baseCooldownTime = cooldownTime;
        baseSlowDuration = slowDuration;
        baseStunDuration = stunDuration;
        baseNormalDrainRate = normalDrainRate;
        baseSlowDrainRate = slowDrainRate;
        baseStunDrainRate = stunDrainRate;
        baseKillDrainRate = killDrainRate;
        
        ApplyDifficultySettings();
    }
    
    private void OnEnable()
    {
        DifficultyManager.OnDifficultyChanged += OnDifficultyChanged;
    }
    
    private void OnDisable()
    {
        DifficultyManager.OnDifficultyChanged -= OnDifficultyChanged;
    }
        private void ApplyDifficultySettings()
    {
        float multiplier = DifficultyManager.GetDifficultyMultiplier();
        
        // Effect timings: Easy (0.7x) = nhanh hơn, Hard (1.5x) = chậm hơn
        // Easy: Dễ stun/kill hơn (timing ngắn hơn)
        // Hard: Khó stun/kill hơn (timing dài hơn)
        slowTime = baseSlowTime / multiplier; // Easy: 0.35s, Hard: 0.75s
        stunTime = baseStunTime / multiplier; // Easy: 1.4s, Hard: 3s
        killTime = baseKillTime / multiplier; // Easy: 4.2s, Hard: 9s
        
        // Cooldown: Easy (0.7x) = ngắn hơn, Hard (1.5x) = dài hơn
        cooldownTime = baseCooldownTime * multiplier; // Easy: 3.5s, Hard: 7.5s
        
        // Effect durations: Easy (1.4x) = dài hơn, Hard (0.67x) = ngắn hơn
        slowDuration = baseSlowDuration / multiplier; // Easy: 4.3s, Hard: 2s
        stunDuration = baseStunDuration / multiplier; // Easy: 3.6s, Hard: 1.67s
        
        // Battery drain rates: Easy (0.7x) = chậm hơn, Hard (1.5x) = nhanh hơn
        normalDrainRate = baseNormalDrainRate * multiplier;
        slowDrainRate = baseSlowDrainRate * multiplier;
        stunDrainRate = baseStunDrainRate * multiplier;
        killDrainRate = baseKillDrainRate * multiplier;
        
    }
    
    private void OnDifficultyChanged(DifficultyManager.Difficulty newDifficulty)
    {
        ApplyDifficultySettings();
     
    }
    
    private void Update()
    {
        if (flashlightCone != null)
        {
            isFlashlightOn = flashlightCone.IsLightOn();
            // Sync detection range with actual flashlight cone length
            detectionRange = flashlightCone.GetConeLength();
        }
        
        if (isFlashlightOn)
        {
            DetectAndAffectShadows();
        }
        else
        {
            
            ClearActiveEffects(); 
        }
        
        UpdateCooldowns();
    }
    
    private void DetectAndAffectShadows()
    {
        Collider2D[] enemiesInRange = Physics2D.OverlapCircleAll(transform.position, detectionRange, enemyLayer);
        
        HashSet<EnemyAI> currentlyExposed = new HashSet<EnemyAI>();
        
        foreach (Collider2D enemyCollider in enemiesInRange)
        {
            EnemyAI enemy = enemyCollider.GetComponent<EnemyAI>();
            if (enemy == null) continue;
            
            if (IsInFlashlightCone(enemy.transform.position))
            {
                currentlyExposed.Add(enemy);
                ProcessShadowExposure(enemy);
            }
        }
        
        // Remove shadows no longer exposed (out of cone or destroyed)
        List<EnemyAI> toRemove = new List<EnemyAI>();
        foreach (var kvp in exposedShadows)
        {
            if (kvp.Value.isSlowPersisting && !currentlyExposed.Contains(kvp.Key))
            {
                Debug.Log($"[DETECT] {kvp.Key.name} is persisting slow - SKIPPING removal (coroutine will handle it)");
                continue; 
            }
            
            if (!currentlyExposed.Contains(kvp.Key) || kvp.Key == null)
            {
                if (kvp.Key != null)
                {
                    Debug.Log($"[DETECT] {kvp.Key.name} is no longer in flashlight cone - removing effects");
                    RemoveShadowEffects(kvp.Key);
                }
                toRemove.Add(kvp.Key);
            }
        }
        
        foreach (var enemy in toRemove)
        {
            Debug.Log($"[DETECT] Removing {enemy?.name ?? "null"} from exposedShadows dictionary");
            exposedShadows.Remove(enemy);
        }
        
        UpdateBatteryDrain();
    }
    
    private bool IsInFlashlightCone(Vector3 targetPosition)
    {
        Vector3 directionToTarget = (targetPosition - transform.position).normalized;
        Vector3 flashlightForward = transform.right; 
        
        float angleToTarget = Vector3.Angle(flashlightForward, directionToTarget);
        float distanceToTarget = Vector3.Distance(transform.position, targetPosition);
        
        return angleToTarget <= detectionAngle && distanceToTarget <= detectionRange;
    }
    
    private void ProcessShadowExposure(EnemyAI enemy)
    {
        if (enemy == null) return;
        
        if (!exposedShadows.ContainsKey(enemy))
        {
            exposedShadows[enemy] = new ShadowExposureData();
        }
        
        ShadowExposureData data = exposedShadows[enemy];
        
        if (data.isSlowPersisting)
        {
            Debug.Log($"{enemy.name} re-exposed during slow persist - resuming progression");
            if (data.slowDurationCoroutine != null)
            {
                StopCoroutine(data.slowDurationCoroutine);
                data.slowDurationCoroutine = null;
            }
            data.isSlowPersisting = false;
        }
        
        if (data.cooldownTimer > 0f)
        {
            ApplySlowEffectOnly(enemy, data);
            return;
        }
        
        data.exposureTime += Time.deltaTime;
        
        ExposureState newState = DetermineExposureState(data.exposureTime);
        
        if (newState != data.currentState)
        {
            TransitionToState(enemy, data, newState);
            data.currentState = newState;
        }
    }
    
    private void ApplySlowEffectOnly(EnemyAI enemy, ShadowExposureData data)
    {
        if (data.currentState == ExposureState.None || data.currentState == ExposureState.Slow)
        {
            EnemyPathFinding pathfinding = enemy.GetComponent<EnemyPathFinding>();
            if (pathfinding != null)
            {
                pathfinding.SetSpeedMultiplier(slowMultiplier);
            }
            
            if (data.currentGlow == null && slowGlowPrefab != null)
            {
                data.currentGlow = Instantiate(slowGlowPrefab, enemy.transform);
            }
        }
    }
    
    private ExposureState DetermineExposureState(float exposureTime)
    {
        if (exposureTime >= killTime)
        {
            return ExposureState.Killing;
        }
        else if (exposureTime >= stunTime)
        {
            return ExposureState.Stun;
        }
        else if (exposureTime >= slowTime)
        {
            return ExposureState.Slow;
        }
        return ExposureState.None;
    }
    
    private void TransitionToState(EnemyAI enemy, ShadowExposureData data, ExposureState newState)
    {
        RemoveVisualEffect(data);
        
        switch (newState)
        {
            case ExposureState.Slow:
                ApplySlowEffect(enemy, data);
                break;
                
            case ExposureState.Stun:
                ApplyStunEffect(enemy, data);
                break;
                
            case ExposureState.Killing:
                ApplyKillEffect(enemy, data);
                break;
        }
    }
    
    private void ApplySlowEffect(EnemyAI enemy, ShadowExposureData data)
    {
        Debug.Log($"Flashlight: Slowing {enemy.name}");
        

        EnemyPathFinding pathfinding = enemy.GetComponent<EnemyPathFinding>();
        if (pathfinding != null)
        {
            pathfinding.SetSpeedMultiplier(slowMultiplier);
        }
        
        if (slowGlowPrefab != null && data.currentGlow == null)
        {
            data.currentGlow = Instantiate(slowGlowPrefab, enemy.transform);
            Debug.Log($"Created slow glow for {enemy.name}");
        }
        else if (slowGlowPrefab == null)
        {
            Debug.LogWarning("SlowGlowPrefab is not assigned in FlashlightEffect!");
        }
    }
    
    private void ApplyStunEffect(EnemyAI enemy, ShadowExposureData data)
    {
        Debug.Log($"Flashlight: Stunning {enemy.name}");
        
        RemoveVisualEffect(data);
        
        EnemyPathFinding pathfinding = enemy.GetComponent<EnemyPathFinding>();
        if (pathfinding != null)
        {
            pathfinding.SetSpeedMultiplier(1f);
        }
        
        data.isStunned = true;
        data.stunCoroutine = StartCoroutine(StunRoutine(enemy, data));
    
        if (stunGlowPrefab != null)
        {
            data.currentGlow = Instantiate(stunGlowPrefab, enemy.transform);
            Debug.Log($"Created stun glow for {enemy.name}");
        }
        
        data.cooldownTimer = cooldownTime;
    }
    
    private void ApplyKillEffect(EnemyAI enemy, ShadowExposureData data)
    {
        if (enemy == null) return;
        
        Debug.Log($"Flashlight: KILLING {enemy.name} - PENALTY APPLIED!");
        
        RemoveVisualEffect(data);
    
        if (ScoreManager.Instance != null)
        {
            ScoreManager.Instance.AddKillPoints();
        }

        RecordFlashlightKill(enemy);
        
        exposedShadows.Remove(enemy);
        
        ShadowGhostDrops shadowDrops1 = enemy.GetComponent<ShadowGhostDrops>();
        ShadowGhost2Drops shadowDrops2 = enemy.GetComponent<ShadowGhost2Drops>();
        ShadowGhost3Drops shadowDrops3 = enemy.GetComponent<ShadowGhost3Drops>();
        
        if (shadowDrops1 != null)
        {
            shadowDrops1.DropItems();
            Debug.Log($"Shadow dropped battery via ShadowGhostDrops");
        }
        else if (shadowDrops2 != null)
        {
            shadowDrops2.SpawnDropsOnDeath();
            Debug.Log($"Shadow dropped battery via ShadowGhost2Drops");
        }
        else if (shadowDrops3 != null)
        {
            shadowDrops3.SpawnDropsOnDeath();
            Debug.Log($"Shadow dropped battery via ShadowGhost3Drops");
        }
        else
        {
            PickUpSpawner pickupSpawner = enemy.GetComponent<PickUpSpawner>();
            if (pickupSpawner != null)
            {
                pickupSpawner.DropBatteryOnly();
                Debug.Log($"Shadow dropped battery via PickUpSpawner");
            }
            else
            {
                Debug.LogWarning($"Shadow {enemy.name} has no drop script - no battery dropped!");
            }
        }
        
        Debug.Log($"Shadow destroyed by flashlight");
        Destroy(enemy.gameObject);
    }
    
    private IEnumerator StunRoutine(EnemyAI enemy, ShadowExposureData data)
    {
        if (enemy == null) yield break;
        
        EnemyPathFinding pathfinding = enemy.GetComponent<EnemyPathFinding>();
        if (pathfinding != null)
        {
            pathfinding.StopMoving();
        }
        
        enemy.enabled = false;
        
        yield return new WaitForSeconds(stunDuration);
        
        if (enemy != null)
        {
            enemy.enabled = true;
            
            if (pathfinding != null)
            {
                pathfinding.ResumeMoving();
            }
            
            data.isStunned = false;
        }
    }
    
    private IEnumerator SlowDurationRoutine(EnemyAI enemy, ShadowExposureData data)
    {
        if (enemy == null)
        {
            Debug.LogWarning("SlowDurationRoutine: enemy is null at start!");
            yield break;
        }
        
        Debug.Log($"[SLOW PERSIST] {enemy.name} slow effect persisting for {slowDuration} seconds (glow stays)");
        yield return new WaitForSeconds(slowDuration);
        
        Debug.Log($"[SLOW PERSIST] {slowDuration} seconds elapsed for {enemy.name}");
        
        if (enemy == null)
        {
            Debug.LogWarning($"[SLOW PERSIST] Enemy became null during wait!");
            yield break;
        }
        
        if (!exposedShadows.ContainsKey(enemy))
        {
            Debug.LogWarning($"[SLOW PERSIST] {enemy.name} no longer in exposedShadows dictionary!");
            yield break;
        }
        
        Debug.Log($"[SLOW PERSIST] {enemy.name} slow duration expired - REMOVING ALL EFFECTS NOW");
    
        if (data.currentGlow != null)
        {
            Debug.Log($"[SLOW PERSIST] Destroying glow: {data.currentGlow.name}");
            Destroy(data.currentGlow);
            data.currentGlow = null;
        }
        else
        {
            Debug.LogWarning($"[SLOW PERSIST] No glow to remove for {enemy.name}!");
        }
        
        EnemyPathFinding pathfinding = enemy.GetComponent<EnemyPathFinding>();
        if (pathfinding != null)
        {
            Debug.Log($"[SLOW PERSIST] Resetting {enemy.name} speed to 1.0 (normal)");
            pathfinding.SetSpeedMultiplier(1f);
            pathfinding.ResumeMoving();
        }
        else
        {
            Debug.LogError($"[SLOW PERSIST] {enemy.name} has no EnemyPathFinding component!");
        }
        
        data.exposureTime = 0f;
        data.currentState = ExposureState.None;
        data.isSlowPersisting = false;
        data.slowDurationCoroutine = null;
        
        exposedShadows.Remove(enemy);
        
        Debug.Log($"[SLOW PERSIST] ✅ {enemy.name} FULLY RECOVERED from slow - speed should be NORMAL now!");
    }
    
    private void RemoveShadowEffects(EnemyAI enemy)
    {
        if (enemy == null || !exposedShadows.ContainsKey(enemy)) return;
        
        ShadowExposureData data = exposedShadows[enemy];
        
        Debug.Log($"Removing effects from {enemy.name}, current state: {data.currentState}");
        
        if (data.currentState == ExposureState.Slow && !data.isSlowPersisting)
        {
            Debug.Log($"{enemy.name} SLOW effect will persist for {slowDuration} seconds");
            data.isSlowPersisting = true;
            data.slowDurationCoroutine = StartCoroutine(SlowDurationRoutine(enemy, data));
            return;
        }
        
        if (data.stunCoroutine != null)
        {
            StopCoroutine(data.stunCoroutine);
            data.stunCoroutine = null;
        }
        
        if (data.slowDurationCoroutine != null)
        {
            StopCoroutine(data.slowDurationCoroutine);
            data.slowDurationCoroutine = null;
        }
        
        RemoveVisualEffect(data);
        
        EnemyPathFinding pathfinding = enemy.GetComponent<EnemyPathFinding>();
        if (pathfinding != null)
        {
            Debug.Log($"Resetting {enemy.name} speed multiplier to 1.0");
            pathfinding.SetSpeedMultiplier(1f);
            pathfinding.ResumeMoving();
        }
        else
        {
            Debug.LogWarning($"{enemy.name} has no EnemyPathFinding component!");
        }
        
        if (data.isStunned && enemy != null)
        {
            enemy.enabled = true;
            data.isStunned = false;
        }
        
        data.exposureTime = 0f;
        data.currentState = ExposureState.None;
        data.isSlowPersisting = false;
        
        Debug.Log($"Effects removed from {enemy.name}, speed should be normal now");
    }
    
    private void RemoveVisualEffect(ShadowExposureData data)
    {
        if (data.currentGlow != null)
        {
            Debug.Log($"Destroying glow: {data.currentGlow.name}");
            Destroy(data.currentGlow);
            data.currentGlow = null;
        }
        else
        {
            Debug.Log("No glow to remove (already null)");
        }
    }
    
    private void ClearActiveEffects()
    {
        Debug.Log($"[CLEAR] Clearing ACTIVE effects (skipping persisting slows). Total shadows: {exposedShadows.Count}");
        
        List<EnemyAI> enemiesToClear = new List<EnemyAI>();
        
        foreach (var kvp in exposedShadows)
        {
            if (kvp.Value.isSlowPersisting)
            {
                Debug.Log($"[CLEAR] SKIPPING {kvp.Key.name} - slow is persisting (coroutine active)");
                continue;
            }
            
            enemiesToClear.Add(kvp.Key);
        }
        
        foreach (var enemy in enemiesToClear)
        {
            if (enemy != null)
            {
                Debug.Log($"[CLEAR] Removing effects from {enemy.name}");
                RemoveShadowEffects(enemy);
                
                if (exposedShadows.ContainsKey(enemy) && !exposedShadows[enemy].isSlowPersisting)
                {
                    Debug.Log($"[CLEAR] Removing {enemy.name} from dictionary");
                    exposedShadows.Remove(enemy);
                }
                else if (exposedShadows.ContainsKey(enemy) && exposedShadows[enemy].isSlowPersisting)
                {
                    Debug.Log($"[CLEAR] KEEPING {enemy.name} in dictionary (now persisting)");
                }
            }
        }
        
        Debug.Log($"[CLEAR] Active effects cleared. Remaining shadows (persisting): {exposedShadows.Count}");

        if (BatteryManager.Instance != null)
        {
            BatteryManager.Instance.SetDrainRate(normalDrainRate);
        }
    }
    
    private void ClearAllEffects()
    {
        Debug.Log($"[CLEAR ALL] FORCE clearing ALL effects. Total shadows: {exposedShadows.Count}");
        
        List<EnemyAI> enemiesToClear = new List<EnemyAI>(exposedShadows.Keys);
        
        foreach (var enemy in enemiesToClear)
        {
            if (enemy != null)
            {
                ShadowExposureData data = exposedShadows[enemy];
                
                if (data.slowDurationCoroutine != null)
                {
                    StopCoroutine(data.slowDurationCoroutine);
                    data.slowDurationCoroutine = null;
                }
                
                RemoveShadowEffects(enemy);
            }
        }
        
        exposedShadows.Clear();
        
        Debug.Log("[CLEAR ALL] All effects FORCE cleared");
        
        if (BatteryManager.Instance != null)
        {
            BatteryManager.Instance.SetDrainRate(normalDrainRate);
        }
    }
    
    private void UpdateCooldowns()
    {
        foreach (var data in exposedShadows.Values)
        {
            if (data.cooldownTimer > 0f)
            {
                data.cooldownTimer -= Time.deltaTime;
            }
        }
    }
    
    private void UpdateBatteryDrain()
    {
        if (BatteryManager.Instance == null) return;
        
        float highestDrainRate = normalDrainRate;
        
        foreach (var data in exposedShadows.Values)
        {
            switch (data.currentState)
            {
                case ExposureState.Slow:
                    highestDrainRate = Mathf.Max(highestDrainRate, slowDrainRate);
                    break;
                case ExposureState.Stun:
                    highestDrainRate = Mathf.Max(highestDrainRate, stunDrainRate);
                    break;
                case ExposureState.Killing:
                    highestDrainRate = Mathf.Max(highestDrainRate, killDrainRate);
                    break;
            }
        }
        
        BatteryManager.Instance.SetDrainRate(highestDrainRate);
    }
    
    private void RecordFlashlightKill(EnemyAI enemy)
    {
        Debug.LogWarning($"FLASHLIGHT KILL RECORDED: {enemy.name} - Player will receive score penalty!");
    }
    
    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, detectionRange);
        
        Vector3 forward = transform.right;
        Vector3 cone1 = Quaternion.Euler(0, 0, detectionAngle) * forward * detectionRange;
        Vector3 cone2 = Quaternion.Euler(0, 0, -detectionAngle) * forward * detectionRange;
        
        Gizmos.color = Color.cyan;
        Gizmos.DrawLine(transform.position, transform.position + cone1);
        Gizmos.DrawLine(transform.position, transform.position + cone2);
    }
}

