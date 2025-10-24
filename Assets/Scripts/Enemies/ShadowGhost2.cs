using System.Collections;
using UnityEngine;

/// <summary>
/// Shadow Ghost 2 - "Bóng đen bắt nạt"
/// Cản trở nhân vật bằng cách block đường đi
/// </summary>
public class ShadowGhost2 : MonoBehaviour, IEnemy
{
    [Header("Blocking Settings")]
    [SerializeField] private float blockRange = 3.0f; // Tầm phát hiện để block
    [SerializeField] private float blockDuration = 1.5f; // Thời gian Shadow Ghost 2 chuyển đỏ
    [SerializeField] private float blockCooldown = 5.0f; // Cooldown giữa các lần block
    [SerializeField] private float playerFlashDuration = 0.5f; // Flash effect trên player khi bị đẩy
    
    [Header("Movement Settings")]
    [SerializeField] private float normalSpeed = 1.5f; // Tốc độ bình thường
    [SerializeField] private float blockSpeed = 0.8f; // Tốc độ khi block (chậm hơn)
    [SerializeField] private float dashSpeed = 4.0f; // Tốc độ khi lao vào player
    [SerializeField] private float dashDuration = 1.0f; // Thời gian dash
    
    [Header("Visual Effects")]
    [SerializeField] private ParticleSystem smokeEffect;
    [SerializeField] private SpriteRenderer spriteRenderer;
    [SerializeField] private Color shadowColor = new Color(0.2f, 0.1f, 0.3f, 1f); // Màu tím đậm hơn
    
    [Header("Blocking Visual")]
    [SerializeField] private GameObject blockEffect; // Effect khi block
    [SerializeField] private Color blockColor = new Color(0.8f, 0.2f, 0.2f, 0.7f); // Màu đỏ khi block
    
    private EnemyPathFinding enemyPathfinding;
    private EnemyAI enemyAI;
    private bool isBlocking = false;
    private bool canBlock = true;
    private bool isDashing = false;
    private Vector2 blockPosition;
    private Coroutine blockCoroutine;
    private Coroutine dashCoroutine;
    private ParticleSystem spawnedSmoke;
    private Transform lockedTarget; // Store target at start of attack
    
    private void Awake()
    {
        enemyPathfinding = GetComponent<EnemyPathFinding>();
        enemyAI = GetComponent<EnemyAI>();
        spriteRenderer = GetComponent<SpriteRenderer>();
        
        // Apply shadow color
        if (spriteRenderer != null)
        {
            spriteRenderer.color = shadowColor;
        }
    }
    
    private void Start()
    {
        // Spawn smoke effect
        if (smokeEffect != null && spawnedSmoke == null)
        {
            spawnedSmoke = Instantiate(smokeEffect, transform);
            spawnedSmoke.transform.localPosition = Vector3.zero;
            spawnedSmoke.transform.localRotation = Quaternion.Euler(-90, 0, 0);
            
            // Configure smoke
            var main = spawnedSmoke.main;
            main.loop = true;
            main.startSize = 0.4f; // Lớn hơn Shadow Ghost 1
            main.startSpeed = 0.3f; // Chậm hơn
            main.maxParticles = 15;
            
            var emission = spawnedSmoke.emission;
            emission.rateOverTime = 3f; // Ít hơn Shadow Ghost 1
            
            spawnedSmoke.Play();
        }
    }
    
    private void Update()
    {
        // ShadowGhost2 blocking is handled by EnemyAI calling Attack()
        // No need for separate Update logic
    }
    
    private void StartBlocking()
    {
        if (isBlocking) return;
        
        isBlocking = true;
        canBlock = false;
        
        // Store current position as block position
        blockPosition = transform.position;
        
        // Change color to indicate blocking
        if (spriteRenderer != null)
        {
            spriteRenderer.color = blockColor;
        }
        
        // Slow down movement
        if (enemyPathfinding != null)
        {
            enemyPathfinding.SetSpeed(blockSpeed);
        }
        
        // Flash effect sẽ được trigger bởi PushBackEnemy khi chạm player
        // Không cần gọi ở đây vì chưa chạm player
        
        // Start blocking coroutine
        blockCoroutine = StartCoroutine(BlockingRoutine());
    }
    
    private IEnumerator BlockingRoutine()
    {
        // Block for duration
        yield return new WaitForSeconds(blockDuration);
        
        // Stop blocking
        StopBlocking();
        
        // Wait for cooldown
        yield return new WaitForSeconds(blockCooldown);
        
        // Can block again
        canBlock = true;
    }
    
    private void StopBlocking()
    {
        isBlocking = false;
        
        // Restore normal color
        if (spriteRenderer != null)
        {
            spriteRenderer.color = shadowColor;
        }
        
        // Restore normal speed
        if (enemyPathfinding != null)
        {
            enemyPathfinding.ResetSpeed();
        }
        
        // Blocking effect on player will auto-hide after duration
        // No need to manually hide it here
    }
    
    public void Attack()
    {
        // Shadow Ghost 2 dashes towards target first, then blocks
        if (!isDashing && !isBlocking)
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
        
        // Perform dash towards locked target
        float elapsedTime = 0f;
        
        while (elapsedTime < dashDuration && lockedTarget != null)
        {
            // Move towards locked target
            Vector2 targetPosition = lockedTarget.position;
            Vector2 dashDirection = (targetPosition - (Vector2)transform.position).normalized;
            
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
            
            // Move towards target
            if (enemyPathfinding != null)
            {
                enemyPathfinding.MoveTo(dashDirection);
            }
            
            elapsedTime += Time.deltaTime;
            yield return null;
        }
        
    // After dash, start blocking if target is close and is Player
    if (lockedTarget != null)
    {
        float distanceToTarget = Vector2.Distance(transform.position, lockedTarget.position);
        // Only block if target is Player (don't block for Victim)
        if (distanceToTarget <= blockRange && canBlock && lockedTarget.CompareTag("Player"))
        {
            StartBlocking();
        }
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
    
    private void OnDestroy()
    {
        // Clean up smoke
        if (spawnedSmoke != null)
        {
            Destroy(spawnedSmoke.gameObject);
        }
        
        // Stop coroutines
        if (blockCoroutine != null)
        {
            StopCoroutine(blockCoroutine);
        }
        
        if (dashCoroutine != null)
        {
            StopCoroutine(dashCoroutine);
        }
    }
    
    private void OnCollisionEnter2D(Collision2D collision)
    {
        // Shadow Ghost 2 ONLY pushes back, does NOT apply slow
        // Push back effect is handled by physics (rigidbody collision)
        // No additional debuff needed
    }

    private void OnCollisionStay2D(Collision2D collision)
    {
        // Shadow Ghost 2 ONLY pushes back, does NOT apply slow
        // No debuff on continuous contact
    }
    
    // Public methods for other systems
    public bool IsBlocking()
    {
        return isBlocking;
    }
    
    public bool IsDashing()
    {
        return isDashing;
    }
    
    public Vector2 GetBlockPosition()
    {
        return blockPosition;
    }
    
    public float GetBlockRange()
    {
        return blockRange;
    }
}
