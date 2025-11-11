using System.Collections;
using UnityEngine;

public class ShadowGhost3 : MonoBehaviour, IEnemy
{
    [Header("Movement Settings")]
    [SerializeField] private float normalSpeed = 2.5f; // Faster than other shadows
    
    [Header("Attack Settings")]
    [SerializeField] private float dashSpeed = 4.5f;
    [SerializeField] private float dashDuration = 1.0f;
    [SerializeField] private float visionReductionRange = 1.5f;
    
    [Header("Visual Effects")]
    [SerializeField] private ParticleSystem smokeEffect;
    [SerializeField] private SpriteRenderer spriteRenderer;
    [SerializeField] private Color shadowColor = new Color(0.4f, 0.2f, 0.5f, 1f); // Purple
    
    private EnemyPathFinding enemyPathfinding;
    private EnemyAI enemyAI;
    private bool isDashing = false;
    private Vector2 dashDirection;
    private Collider2D myCollider;
    private ParticleSystem spawnedSmoke; // Track spawned smoke
    private Transform lockedTarget; // Store target at start of attack
    private LightFearBehavior lightFear; // Fear of light behavior
    
    // Lưu giá trị gốc
    private float baseNormalSpeed;
    private float baseDashSpeed;

    private void Awake()
    {
        enemyPathfinding = GetComponent<EnemyPathFinding>();
        enemyAI = GetComponent<EnemyAI>();
        spriteRenderer = GetComponent<SpriteRenderer>();
        myCollider = GetComponent<Collider2D>();
        lightFear = GetComponent<LightFearBehavior>();
        
        // Apply shadow color (purple)
        if (spriteRenderer != null)
        {
            spriteRenderer.color = shadowColor;
        }
        
        // Keep collider normal (not trigger) so it collides with walls/tileset
        // NonDamagingEnemy component prevents damage/knockback to player
        
        // Lưu giá trị gốc
        baseNormalSpeed = normalSpeed;
        baseDashSpeed = dashSpeed;
        
        // Áp dụng difficulty
        ApplyDifficultySettings();
    }
    
    private void OnEnable()
    {
        // Subscribe vào event khi difficulty thay đổi
        DifficultyManager.OnDifficultyChanged += OnDifficultyChanged;
    }
    
    private void OnDisable()
    {
        // Unsubscribe
        DifficultyManager.OnDifficultyChanged -= OnDifficultyChanged;
    }
    
    /// <summary>
    /// Áp dụng difficulty vào ShadowGhost3 speed settings
    /// </summary>
    private void ApplyDifficultySettings()
    {
        float multiplier = DifficultyManager.GetDifficultyMultiplier();
        
        // Speed: Easy (0.7x) = chậm hơn, Hard (1.5x) = nhanh hơn
        normalSpeed = baseNormalSpeed * multiplier;
        dashSpeed = baseDashSpeed * multiplier;
        
        Debug.Log($"ShadowGhost3: Difficulty applied - Normal Speed: {normalSpeed:F2}, Dash Speed: {dashSpeed:F2}");
    }
    
    /// <summary>
    /// Callback khi difficulty thay đổi
    /// </summary>
    private void OnDifficultyChanged(DifficultyManager.Difficulty newDifficulty)
    {
        ApplyDifficultySettings();
        
        // Cập nhật speed nếu không đang dash
        if (!isDashing && enemyPathfinding != null)
        {
            enemyPathfinding.SetSpeed(normalSpeed);
        }
    }

    private void Start()
    {
        // SET NORMAL SPEED vào EnemyPathFinding (CRITICAL!)
        if (enemyPathfinding != null)
        {
            enemyPathfinding.SetSpeed(normalSpeed);
        }
        
        // Only spawn smoke once
        if (smokeEffect != null && spawnedSmoke == null)
        {
            // Create smoke effect as child with purple tint
            spawnedSmoke = Instantiate(smokeEffect, transform);
            spawnedSmoke.transform.localPosition = Vector3.zero;
            spawnedSmoke.transform.localRotation = Quaternion.Euler(-90, 0, 0);
            
            // Configure smoke to be subtle with purple color
            var main = spawnedSmoke.main;
            main.loop = true;
            main.startSize = 0.3f;
            main.startSpeed = 0.5f;
            main.maxParticles = 20;
            main.startColor = new Color(0.5f, 0.2f, 0.6f, 0.5f); // Purple smoke
            
            // Reduce emission rate
            var emission = spawnedSmoke.emission;
            emission.rateOverTime = 5f;
            
            spawnedSmoke.Play();
        }
    }
    
    private void OnDestroy()
    {
        // Clean up smoke when Shadow Ghost 3 is destroyed
        if (spawnedSmoke != null)
        {
            Destroy(spawnedSmoke.gameObject);
        }
    }

    public void Attack()
    {
        // Cannot attack if fleeing from light
        if (lightFear != null && lightFear.IsFleeing())
        {
            return;
        }
        
        if (!isDashing)
        {
            // Lock target at the start of attack
            if (enemyAI != null)
            {
                lockedTarget = enemyAI.GetCurrentTarget();
            }
            
            // Play ghost attack sound only when attacking Player
            if (lockedTarget != null && lockedTarget.CompareTag("Player"))
            {
                if (SFXManager.Instance != null)
                {
                    SFXManager.Instance.PlayGhostSound();
                }
            }
            
            StartCoroutine(DashAttackRoutine());
        }
    }

    private IEnumerator DashAttackRoutine()
    {
        isDashing = true;
        
        // Validate target exists
        if (lockedTarget == null)
        {
            isDashing = false;
            yield break;
        }
        
        // Set dash speed
        if (enemyPathfinding != null)
        {
            enemyPathfinding.SetSpeed(dashSpeed);
        }
        
        // Perform dash - continuously update direction towards locked target (homing)
        float elapsedTime = 0f;
        
        while (elapsedTime < dashDuration && lockedTarget != null)
        {
            // Recalculate direction to locked target every frame
            Vector2 targetPosition = lockedTarget.position;
            dashDirection = (targetPosition - (Vector2)transform.position).normalized;
            
            // Flip sprite based on direction
            if (spriteRenderer != null)
            {
                if (dashDirection.x < 0)
                {
                    spriteRenderer.flipX = true;
                }
                else
                {
                    spriteRenderer.flipX = false;
                }
            }
            
            // Move towards target during dash
            if (enemyPathfinding != null)
            {
                enemyPathfinding.MoveTo(dashDirection);
            }
            
            // Check if close enough to apply debuff
            float distanceToTarget = Vector2.Distance(transform.position, targetPosition);
            if (distanceToTarget <= visionReductionRange)
            {
                ApplyDebuffToTarget();
            }
            
        elapsedTime += Time.deltaTime;
        yield return null;
    }
    
    // Reset speed after dash
    // Don't call StopMoving() - let EnemyAI handle movement
    if (enemyPathfinding != null)
    {
        enemyPathfinding.ResetSpeed();
    }
    
    isDashing = false;
    lockedTarget = null; // Clear locked target
}

    private void ApplyDebuffToTarget()
    {
        if (lockedTarget == null) return;
        
        // Apply vision reduction to Player
        if (lockedTarget.CompareTag("Player"))
        {
            if (VisionDebuff.Instance != null)
            {
                VisionDebuff.Instance.ApplyVisionReduction();
            }
        }
        // Apply slow to Victim
        else if (lockedTarget.CompareTag("Victim"))
        {
            Victim victim = lockedTarget.GetComponent<Victim>();
            if (victim != null)
            {
                victim.ApplySlow();
            }
        }
    }
    
    private void ApplyVisionReduction()
    {
        if (VisionDebuff.Instance != null)
        {
            VisionDebuff.Instance.ApplyVisionReduction();
        }
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        // Apply vision reduction when colliding with player
        if (collision.gameObject.CompareTag("Player"))
        {
            ApplyVisionReduction();
        }
        // Apply slow effect when colliding with victim
        else if (collision.gameObject.CompareTag("Victim"))
        {
            Victim victim = collision.gameObject.GetComponent<Victim>();
            if (victim != null)
            {
                victim.ApplySlow();
            }
        }
    }

    private void OnCollisionStay2D(Collision2D collision)
    {
        // Continuously check for vision reduction application when in contact with player
        if (collision.gameObject.CompareTag("Player"))
        {
            ApplyVisionReduction();
        }
        // Continuously check for slow application when in contact with victim
        else if (collision.gameObject.CompareTag("Victim"))
        {
            Victim victim = collision.gameObject.GetComponent<Victim>();
            if (victim != null)
            {
                victim.ApplySlow();
            }
        }
    }

    public bool IsDashing()
    {
        return isDashing;
    }
}

