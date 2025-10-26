using UnityEngine;

/// <summary>
/// Makes victim follow player after being rescued
/// Victim will follow to safe zone
/// </summary>
public class VictimFollowPlayer : MonoBehaviour
{
    [Header("Follow Settings")]
    [SerializeField] private float followSpeed = 2.5f;
    [SerializeField] private float stopDistance = 1.2f; // Stop when this close to player
    
    private Rigidbody2D rb;
    private SpriteRenderer spriteRenderer;
    private Animator animator;
    private Transform playerTransform;
    
    private void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        spriteRenderer = GetComponent<SpriteRenderer>();
        animator = GetComponent<Animator>();
        
        // IMPORTANT: Ensure victim has its own animator instance
        // Prevent any shared animation state with player
        if (animator != null)
        {
            animator.Rebind();
        }
    }

    private void OnEnable()
    {
        // Reset animator state when victim starts following
        if (animator != null)
        {
            animator.SetFloat("moveX", 0);
            animator.SetFloat("moveY", 0);
        }
    }
    
    private void Start()
    {
        if (PlayerController.Instance != null)
        {
            playerTransform = PlayerController.Instance.transform;
        }
    }

    private void FixedUpdate()
    {
        if (playerTransform == null) return;
        
        float distanceToPlayer = Vector2.Distance(transform.position, playerTransform.position);
        
        // Stop if too close - add a small buffer to prevent jittering
        if (distanceToPlayer <= stopDistance)
        {
            // Completely stop
            rb.linearVelocity = Vector2.zero;
            
            if (animator != null)
            {
                animator.SetFloat("moveX", 0);
                animator.SetFloat("moveY", 0);
            }
            return;
        }
        
        // Follow player
        Vector2 direction = (playerTransform.position - transform.position).normalized;
        
        // Check if there's a victim very close in front blocking direct path
        Victim blockingVictim = GetVictimBlockingPath(direction);
        
        if (blockingVictim != null)
        {
            // Follow the blocking victim instead of player (train-like behavior)
            Vector2 victimDirection = (blockingVictim.transform.position - transform.position).normalized;
            float distanceToVictim = Vector2.Distance(transform.position, blockingVictim.transform.position);
            
            // Keep some distance from the victim in front
            if (distanceToVictim > 0.8f)
            {
                rb.MovePosition(rb.position + victimDirection * (followSpeed * 0.8f * Time.fixedDeltaTime));
                direction = victimDirection; // Use this for animation
            }
            else
            {
                // Too close to victim, stop
                rb.linearVelocity = Vector2.zero;
                
                if (animator != null)
                {
                    animator.SetFloat("moveX", 0);
                    animator.SetFloat("moveY", 0);
                }
                return;
            }
        }
        else
        {
            // No blocking victim, follow player directly
            rb.MovePosition(rb.position + direction * (followSpeed * Time.fixedDeltaTime));
        }
        
        // Flip sprite based on direction
        if (direction.x < 0)
        {
            spriteRenderer.flipX = true;
        }
        else if (direction.x > 0)
        {
            spriteRenderer.flipX = false;
        }
        
        // Update animation
        if (animator != null)
        {
            animator.SetFloat("moveX", direction.x);
            animator.SetFloat("moveY", direction.y);
        }
    }
    
    /// <summary>
    /// Check if there's a victim blocking our direct path to player
    /// Returns the closest victim in front of us (train-like following)
    /// </summary>
    private Victim GetVictimBlockingPath(Vector2 directionToPlayer)
    {
        // Cast a small circle in front to detect victims
        Vector2 checkPosition = (Vector2)transform.position + directionToPlayer * 0.6f;
        Collider2D[] nearbyColliders = Physics2D.OverlapCircleAll(checkPosition, 0.7f);
        
        Victim closestVictim = null;
        float closestDistance = float.MaxValue;
        
        foreach (Collider2D col in nearbyColliders)
        {
            if (col.gameObject == gameObject) continue; // Skip self
            
            Victim otherVictim = col.GetComponent<Victim>();
            if (otherVictim != null && otherVictim.IsRescued())
            {
                // Check if this victim is between us and player
                float distanceToVictim = Vector2.Distance(transform.position, otherVictim.transform.position);
                float victimDistanceToPlayer = Vector2.Distance(otherVictim.transform.position, playerTransform.position);
                float ourDistanceToPlayer = Vector2.Distance(transform.position, playerTransform.position);
                
                // Only consider if victim is closer to player than we are
                if (victimDistanceToPlayer < ourDistanceToPlayer && distanceToVictim < closestDistance)
                {
                    closestVictim = otherVictim;
                    closestDistance = distanceToVictim;
                }
            }
        }
        
        return closestVictim;
    }
}

