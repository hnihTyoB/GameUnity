using System.Collections;
using UnityEngine;

public class ShadowGhost : MonoBehaviour, IEnemy
{
    [Header("Attack Settings")]
    [SerializeField] private float dashSpeed = 4.5f; // Giảm để player có thể thoát
    [SerializeField] private float dashDuration = 1.0f; // Giảm thời gian dash
    [SerializeField] private float slowEffectRange = 1.5f; // Range áp dụng slow
    
    [Header("Visual Effects")]
    [SerializeField] private ParticleSystem smokeEffect;
    [SerializeField] private SpriteRenderer spriteRenderer;
    [SerializeField] private Color shadowColor = new Color(0.169f, 0.102f, 0.231f, 1f); // #2b1a3b
    
    private EnemyPathFinding enemyPathfinding;
    private EnemyAI enemyAI;
    private bool isDashing = false;
    private Vector2 dashDirection;
    private Collider2D myCollider;
    private ParticleSystem spawnedSmoke; // Track spawned smoke
    private Transform lockedTarget; // Store target at start of attack
    private LightFearBehavior lightFear; // Fear of light behavior

    private void Awake()
    {
        enemyPathfinding = GetComponent<EnemyPathFinding>();
        enemyAI = GetComponent<EnemyAI>();
        spriteRenderer = GetComponent<SpriteRenderer>();
        myCollider = GetComponent<Collider2D>();
        lightFear = GetComponent<LightFearBehavior>();
        
        // Apply shadow color
        if (spriteRenderer != null)
        {
            spriteRenderer.color = shadowColor;
        }
        
        // Keep collider normal (not trigger) so it collides with walls/tileset
        // NonDamagingEnemy component prevents damage/knockback to player
    }

    private void Start()
    {
        // Only spawn smoke once
        if (smokeEffect != null && spawnedSmoke == null)
        {
            // Create smoke effect as child
            spawnedSmoke = Instantiate(smokeEffect, transform);
            spawnedSmoke.transform.localPosition = Vector3.zero;
            spawnedSmoke.transform.localRotation = Quaternion.Euler(-90, 0, 0);
            
            // Configure smoke to be subtle
            var main = spawnedSmoke.main;
            main.loop = true;
            main.startSize = 0.3f; // Nhỏ hơn
            main.startSpeed = 0.5f; // Chậm hơn
            main.maxParticles = 20; // Giới hạn số particles
            
            // Reduce emission rate
            var emission = spawnedSmoke.emission;
            emission.rateOverTime = 5f; // Chỉ 5 particles/giây
            
            spawnedSmoke.Play();
        }
    }
    
    private void OnDestroy()
    {
        // Clean up smoke when Shadow Ghost is destroyed
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
        
        // Perform dash - continuously update direction towards locked target
        float elapsedTime = 0f;
        
        while (elapsedTime < dashDuration && lockedTarget != null)
        {
            // Recalculate direction to locked target every frame (homing)
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
            
            // Check if close enough to apply slow effect
            float distanceToTarget = Vector2.Distance(transform.position, targetPosition);
            if (distanceToTarget <= slowEffectRange)
            {
                ApplySlowToTarget();
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

    private void ApplySlowToTarget()
    {
        if (lockedTarget == null) return;
        
        // Apply slow to Player
        if (lockedTarget.CompareTag("Player"))
        {
            if (SlowDebuff.Instance != null)
            {
                SlowDebuff.Instance.ApplySlow();
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

    private void OnCollisionEnter2D(Collision2D collision)
    {
        // Apply slow effect when colliding with player
        if (collision.gameObject.CompareTag("Player"))
        {
            if (SlowDebuff.Instance != null)
            {
                SlowDebuff.Instance.ApplySlow();
            }
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
        // Continuously check for slow application when in contact with player
        if (collision.gameObject.CompareTag("Player"))
        {
            if (SlowDebuff.Instance != null)
            {
                SlowDebuff.Instance.ApplySlow();
            }
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

