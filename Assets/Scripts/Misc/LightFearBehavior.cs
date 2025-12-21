using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class LightFearBehavior : MonoBehaviour
{
    [Header("Fear Settings")]
    [SerializeField] private float fleeSpeedMultiplier = 1.5f; 
    [SerializeField] private float fearCheckInterval = 0.2f; 
    [SerializeField] private float safeDistanceBuffer = 1.0f;
    
    [Header("Visual Effects")]
    [SerializeField] private bool showFearEffect = true;
    [SerializeField] private Color fearColor = new Color(0.8f, 0.8f, 1f, 1f);
    
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
            
          
            TorcheLight torche = nearestLightSource.GetComponent<TorcheLight>();
            if (torche != null)
            {
                float lightRadius = torche.GetLightRadius();
                float safeDistance = lightRadius + safeDistanceBuffer;
                
        
                
                if (distanceToLight <= lightRadius)
                {
                    if (!isInLight)
                    {
                        EnterLight();
                    }
                }
                else if (distanceToLight >= safeDistance)
                {
                    if (isInLight || isFleeing)
                    {
                        StopFleeing();
                    }
                }
                else
                {
                    if (isInLight)
                    {
                        isInLight = false;
                    }
                }
            }
        }
        else
        {
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
        
     
        if (showFearEffect && spriteRenderer != null)
        {
            spriteRenderer.color = fearColor;
        }
        
        if (enemyPathfinding != null)
        {
            enemyPathfinding.SetSpeed(originalSpeed * fleeSpeedMultiplier);
        }
        
        if (enemyAI != null)
        {
            enemyAI.enabled = false;
        }
    }

    private void StopFleeing()
    {
        isInLight = false;
        isFleeing = false;
        
        if (spriteRenderer != null)
        {
            spriteRenderer.color = originalColor;
        }
        
        if (enemyPathfinding != null)
        {
            enemyPathfinding.SetSpeed(originalSpeed);
        }
        
        if (enemyAI != null)
        {
            enemyAI.enabled = true;
        }
    }

    private void FleeFromLight()
    {
        if (nearestLightSource == null || enemyPathfinding == null) return;
        
        Vector2 fleeDirection = ((Vector2)transform.position - (Vector2)nearestLightSource.position).normalized;
        
        enemyPathfinding.MoveTo(fleeDirection);
        
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
    
    private void OnDrawGizmosSelected()
    {
        if (nearestLightSource != null)
        {
            Gizmos.color = isFleeing ? Color.yellow : Color.green;
            Gizmos.DrawLine(transform.position, nearestLightSource.position);
            
            TorcheLight torche = nearestLightSource.GetComponent<TorcheLight>();
            if (torche != null)
            {
                float distance = Vector2.Distance(transform.position, nearestLightSource.position);
                float lightRadius = torche.GetLightRadius();
                
                if (isInLight)
                {
                    Gizmos.color = Color.red; 
                }
                else if (isFleeing)
                {
                    Gizmos.color = Color.yellow; 
                }
                else
                {
                    Gizmos.color = Color.green; 
                }
                
                Gizmos.DrawWireSphere(transform.position, 0.5f);
            }
        }
    }
}