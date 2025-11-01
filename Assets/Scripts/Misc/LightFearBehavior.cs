using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Makes Shadow Ghost fear light from Torche
/// When in light, shadow will flee and cannot attack
/// </summary>
public class LightFearBehavior : MonoBehaviour
{
    [Header("Fear Settings")]
    [SerializeField] private float fleeSpeedMultiplier = 1.5f; // Speed boost when fleeing
    [SerializeField] private float fearCheckInterval = 0.2f; // How often to check for light
    [SerializeField] private float safeDistanceBuffer = 1.0f; // Extra distance beyond light radius to feel safe
    
    [Header("Visual Effects")]
    [SerializeField] private bool showFearEffect = true;
    [SerializeField] private Color fearColor = new Color(0.8f, 0.8f, 1f, 1f); // Lighter color when afraid
    
    private EnemyPathFinding enemyPathfinding;
    private EnemyAI enemyAI;
    private SpriteRenderer spriteRenderer;
    private Color originalColor;
    
    private bool isInLight = false;
    private bool isFleeing = false;
    private Transform nearestLightSource;
    private List<Transform> activeLightSources = new List<Transform>();
    
    private float originalSpeed;

    private void Awake()
    {
        enemyPathfinding = GetComponent<EnemyPathFinding>();
        enemyAI = GetComponent<EnemyAI>();
        spriteRenderer = GetComponent<SpriteRenderer>();
        
        if (spriteRenderer != null)
        {
            originalColor = spriteRenderer.color;
        }
    }

    private void Start()
    {
        if (enemyPathfinding != null)
        {
            originalSpeed = enemyPathfinding.GetMoveSpeed();
        }
        
        StartCoroutine(CheckForLightRoutine());
    }

    private IEnumerator CheckForLightRoutine()
    {
        while (true)
        {
            yield return new WaitForSeconds(fearCheckInterval);
            CheckNearbyLight();
        }
    }

    private void CheckNearbyLight()
    {
        nearestLightSource = FindNearestLight();
        
        if (nearestLightSource != null)
        {
            float distanceToLight = Vector2.Distance(transform.position, nearestLightSource.position);
            
            // Check if we're within the light radius
            TorcheLight torche = nearestLightSource.GetComponent<TorcheLight>();
            if (torche != null)
            {
                float lightRadius = torche.GetLightRadius();
                float safeDistance = lightRadius + safeDistanceBuffer;
                
                // DEBUG: Uncomment to see values
                // Debug.Log($"{gameObject.name}: Distance={distanceToLight:F2}, LightRadius={lightRadius:F2}, SafeDist={safeDistance:F2}, IsInLight={isInLight}, IsFleeing={isFleeing}");
                
                if (distanceToLight <= lightRadius)
                {
                    // Inside light radius - start fleeing
                    if (!isInLight)
                    {
                        EnterLight();
                    }
                }
                else if (distanceToLight >= safeDistance)
                {
                    // Far enough from light - stop fleeing completely
                    if (isInLight || isFleeing)
                    {
                        StopFleeing();
                    }
                }
                else
                {
                    // Between light radius and safe distance - keep fleeing but not "in light"
                    if (isInLight)
                    {
                        isInLight = false; // No longer in direct light
                    }
                    // Keep isFleeing = true until reaching safe distance
                }
            }
        }
        else
        {
            // No light source nearby - stop fleeing
            if (isInLight || isFleeing)
            {
                StopFleeing();
            }
        }
    }

    private void Update()
    {
        if (isFleeing && nearestLightSource != null)
        {
            FleeFromLight();
        }
    }

    private void EnterLight()
    {
        isInLight = true;
        isFleeing = true;
        
        // Apply fear visual effect
        if (showFearEffect && spriteRenderer != null)
        {
            spriteRenderer.color = fearColor;
        }
        
        // Increase speed for fleeing
        if (enemyPathfinding != null)
        {
            enemyPathfinding.SetSpeed(originalSpeed * fleeSpeedMultiplier);
        }
        
        // Disable attacking while in light
        if (enemyAI != null)
        {
            enemyAI.enabled = false;
        }
    }

    private void StopFleeing()
    {
        isInLight = false;
        isFleeing = false;
        
        // Restore original appearance
        if (spriteRenderer != null)
        {
            spriteRenderer.color = originalColor;
        }
        
        // Restore original speed
        if (enemyPathfinding != null)
        {
            enemyPathfinding.SetSpeed(originalSpeed);
        }
        
        // Re-enable attacking
        if (enemyAI != null)
        {
            enemyAI.enabled = true;
        }
    }

    private void FleeFromLight()
    {
        if (nearestLightSource == null || enemyPathfinding == null) return;
        
        // Calculate flee direction (away from light)
        Vector2 fleeDirection = ((Vector2)transform.position - (Vector2)nearestLightSource.position).normalized;
        
        // Move away from light
        enemyPathfinding.MoveTo(fleeDirection);
        
        // Flip sprite based on flee direction
        if (spriteRenderer != null)
        {
            spriteRenderer.flipX = fleeDirection.x < 0;
        }
    }

    private Transform FindNearestLight()
    {
        TorcheLight[] torches = FindObjectsOfType<TorcheLight>();
        
        if (torches.Length == 0) return null;
        
        Transform nearest = null;
        float nearestDistance = float.MaxValue;
        
        foreach (TorcheLight torche in torches)
        {
            if (!torche.IsLightActive()) continue;
            
            float distance = Vector2.Distance(transform.position, torche.transform.position);
            
            if (distance < nearestDistance)
            {
                nearestDistance = distance;
                nearest = torche.transform;
            }
        }
        
        return nearest;
    }

    public bool IsInLight()
    {
        return isInLight;
    }

    public bool IsFleeing()
    {
        return isFleeing;
    }
    
    // Debug visualization
    private void OnDrawGizmosSelected()
    {
        if (nearestLightSource != null)
        {
            // Draw line to nearest light
            Gizmos.color = isFleeing ? Color.yellow : Color.green;
            Gizmos.DrawLine(transform.position, nearestLightSource.position);
            
            // Draw sphere showing current distance
            TorcheLight torche = nearestLightSource.GetComponent<TorcheLight>();
            if (torche != null)
            {
                float distance = Vector2.Distance(transform.position, nearestLightSource.position);
                float lightRadius = torche.GetLightRadius();
                
                // Show status with color
                if (isInLight)
                {
                    Gizmos.color = Color.red; // In light
                }
                else if (isFleeing)
                {
                    Gizmos.color = Color.yellow; // Fleeing but outside light
                }
                else
                {
                    Gizmos.color = Color.green; // Safe
                }
                
                Gizmos.DrawWireSphere(transform.position, 0.5f);
            }
        }
    }
}