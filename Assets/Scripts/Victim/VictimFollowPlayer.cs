using UnityEngine;


public class VictimFollowPlayer : MonoBehaviour
{
    [Header("Follow Settings")]
    [SerializeField] private float followSpeed = 2.5f;
    [SerializeField] private float stopDistance = 1.2f; 
    
    private Rigidbody2D rb;
    private SpriteRenderer spriteRenderer;
    private Animator animator;
    private Transform playerTransform;
    
    private void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        spriteRenderer = GetComponent<SpriteRenderer>();
        animator = GetComponent<Animator>();
    
        if (animator != null)
        {
            animator.Rebind();
        }
    }

    private void OnEnable()
    {
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
        
        if (distanceToPlayer <= stopDistance)
        {
   
            rb.linearVelocity = Vector2.zero;
            
            if (animator != null)
            {
                animator.SetFloat("moveX", 0);
                animator.SetFloat("moveY", 0);
            }
            return;
        }
        
 
        Vector2 direction = (playerTransform.position - transform.position).normalized;
        
        Victim blockingVictim = GetVictimBlockingPath(direction);
        
        if (blockingVictim != null)
        {
            Vector2 victimDirection = (blockingVictim.transform.position - transform.position).normalized;
            float distanceToVictim = Vector2.Distance(transform.position, blockingVictim.transform.position);
            
            if (distanceToVictim > 0.8f)
            {
                rb.MovePosition(rb.position + victimDirection * (followSpeed * 0.8f * Time.fixedDeltaTime));
                direction = victimDirection; 
            }
            else
            {
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
            rb.MovePosition(rb.position + direction * (followSpeed * Time.fixedDeltaTime));
        }
        
        if (direction.x < 0)
        {
            spriteRenderer.flipX = true;
        }
        else if (direction.x > 0)
        {
            spriteRenderer.flipX = false;
        }
        
        if (animator != null)
        {
            animator.SetFloat("moveX", direction.x);
            animator.SetFloat("moveY", direction.y);
        }
    }

    private Victim GetVictimBlockingPath(Vector2 directionToPlayer)
    {
 
        Vector2 checkPosition = (Vector2)transform.position + directionToPlayer * 0.6f;
        Collider2D[] nearbyColliders = Physics2D.OverlapCircleAll(checkPosition, 0.7f);
        
        Victim closestVictim = null;
        float closestDistance = float.MaxValue;
        
        foreach (Collider2D col in nearbyColliders)
        {
            if (col.gameObject == gameObject) continue; 
            
            Victim otherVictim = col.GetComponent<Victim>();
            if (otherVictim != null && otherVictim.IsRescued())
            {
       
                float distanceToVictim = Vector2.Distance(transform.position, otherVictim.transform.position);
                float victimDistanceToPlayer = Vector2.Distance(otherVictim.transform.position, playerTransform.position);
                float ourDistanceToPlayer = Vector2.Distance(transform.position, playerTransform.position);
                
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

