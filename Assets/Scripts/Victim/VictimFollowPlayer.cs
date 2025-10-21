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
        
        // Check if other victims are blocking our path
        bool isBlocked = CheckForOtherVictims();
        
        if (isBlocked)
        {
            // Stop if blocked by other victims
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
        rb.MovePosition(rb.position + direction * (followSpeed * Time.fixedDeltaTime));
        
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
    
    private bool CheckForOtherVictims()
    {
        // Check if there are other rescued victims too close to us
        Collider2D[] nearbyColliders = Physics2D.OverlapCircleAll(transform.position, 0.8f);
        
        foreach (Collider2D col in nearbyColliders)
        {
            if (col.gameObject == gameObject) continue; // Skip self
            
            Victim otherVictim = col.GetComponent<Victim>();
            if (otherVictim != null && otherVictim.IsRescued())
            {
                // Another rescued victim is too close → stop moving
                return true;
            }
        }
        
        return false;
    }
}

