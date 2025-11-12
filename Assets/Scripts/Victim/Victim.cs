using System.Collections;
using UnityEngine;

/// <summary>
/// Victim behavior - Roaming until rescued by player (press E)
/// After rescue, follows player to safe zone
/// </summary>
public class Victim : MonoBehaviour
{
    [Header("Roaming Settings")]
    [SerializeField] private float roamChangeInterval = 2f;
    [SerializeField] private float moveSpeed = 1.5f; // Slower than player
    
    [Header("Rescue Settings")]
    [SerializeField] private float rescueRange = 2f; // Distance player needs to be to rescue
    [SerializeField] private KeyCode rescueKey = KeyCode.E;
    [SerializeField] private GameObject rescuePrompt; // UI prompt "Press E to rescue"
    
    [Header("Visual Feedback")]
    [SerializeField] private GameObject victimIndicator; // Visual indicator (aura/exclamation mark)
    
    private Rigidbody2D rb;
    private SpriteRenderer spriteRenderer;
    private Animator animator;
    private Vector2 roamDirection;
    private bool isRescued = false;
    private bool isRescuing = false; // Lock to prevent double rescue
    private bool hasReachedSafeZone = false; // Track if victim has reached safe zone
    private VictimFollowPlayer followScript;
    private VictimSlowDebuff slowDebuff;
    private float roamTimer;
    private bool wasInRange = false; // Track if player was in range last frame
    private float currentMoveSpeed; // Current speed (can be modified by debuffs)
    private float baseMoveSpeed; // Original speed
    
    private void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        spriteRenderer = GetComponent<SpriteRenderer>();
        animator = GetComponent<Animator>();
        followScript = GetComponent<VictimFollowPlayer>();
        slowDebuff = GetComponent<VictimSlowDebuff>();
        
        // Initialize speed
        baseMoveSpeed = moveSpeed;
        currentMoveSpeed = moveSpeed;
        
        // CRITICAL: Ensure victim GameObject name is NOT "Player"
        // This prevents Input System from targeting this GameObject
        if (gameObject.name.Contains("Player"))
        {
            gameObject.name = gameObject.name.Replace("Player", "Victim");
        }
        
        // Disable PlayerController if exists (victim shouldn't be controlled by player)
        PlayerController playerController = GetComponent<PlayerController>();
        if (playerController != null)
        {
            Destroy(playerController); // Destroy instead of disable
        }
        
        // Disable ActiveWeapon if exists
        ActiveWeapon activeWeapon = GetComponent<ActiveWeapon>();
        if (activeWeapon != null)
        {
            Destroy(activeWeapon);
        }
        
        // Disable ActiveInventory if exists
        ActiveInventory activeInventory = GetComponent<ActiveInventory>();
        if (activeInventory != null)
        {
            Destroy(activeInventory);
        }
        
        if (followScript != null)
        {
            followScript.enabled = false; // Disable follow until rescued
        }
    }

    private void Start()
    {
        // Start roaming
        roamTimer = roamChangeInterval;
        GetRoamingDirection();
        
        // Hide rescue prompt initially
        if (rescuePrompt != null)
        {
            rescuePrompt.SetActive(false);
        }
        
        // Show victim indicator
        if (victimIndicator != null)
        {
            victimIndicator.SetActive(true);
        }
    }

    private void Update()
    {
        if (isRescued) return; // Stop all victim behavior after rescue
        
        // Check if player is in rescue range
        CheckRescueInteraction();
        
        // Roaming behavior
        roamTimer -= Time.deltaTime;
        if (roamTimer <= 0)
        {
            GetRoamingDirection();
            roamTimer = roamChangeInterval;
        }
    }

    private void FixedUpdate()
    {
        if (isRescued) return;
        
        // Move in roam direction using current speed (affected by debuffs)
        rb.MovePosition(rb.position + roamDirection * (currentMoveSpeed * Time.fixedDeltaTime));
        
        // Flip sprite based on direction
        if (roamDirection.x < 0)
        {
            spriteRenderer.flipX = true;
        }
        else if (roamDirection.x > 0)
        {
            spriteRenderer.flipX = false;
        }
        
        // Update animation
        if (animator != null)
        {
            animator.SetFloat("moveX", roamDirection.x);
            animator.SetFloat("moveY", roamDirection.y);
        }
    }

    private void GetRoamingDirection()
    {
        // Random direction for roaming
        roamDirection = new Vector2(Random.Range(-1f, 1f), Random.Range(-1f, 1f)).normalized;
    }

    private void CheckRescueInteraction()
    {
        if (PlayerController.Instance == null) return;
        if (isRescued) return; // Already rescued, stop checking
        
        float distanceToPlayer = Vector2.Distance(transform.position, PlayerController.Instance.transform.position);
        bool inRange = distanceToPlayer <= rescueRange;
        
        // Show rescue prompt if player is in range
        if (inRange)
        {
            // Track range entry
            if (!wasInRange)
            {
                wasInRange = true;
            }
            
            // Show prompt
            if (rescuePrompt != null && !rescuePrompt.activeSelf)
            {
                rescuePrompt.SetActive(true);
            }
            
            // Check for rescue input
            if (Input.GetKeyDown(rescueKey) && !isRescued && !isRescuing)
            {
                isRescuing = true;
                RescueVictim();
                return;
            }
        }
        else
        {
            // Player left range
            if (wasInRange)
            {
                wasInRange = false;
            }
            
            if (rescuePrompt != null && rescuePrompt.activeSelf)
            {
                rescuePrompt.SetActive(false);
            }
        }
    }

    private void RescueVictim()
    {
        if (isRescued)
        {
            return; // Prevent double rescue
        }
        
        isRescued = true;
        
        // Hide rescue prompt and indicator
        if (rescuePrompt != null)
        {
            rescuePrompt.SetActive(false);
        }
        
        if (victimIndicator != null)
        {
            victimIndicator.SetActive(false);
        }
        
        // Stop roaming
        roamDirection = Vector2.zero;
        
        // Enable follow script
        if (followScript != null)
        {
            followScript.enabled = true;
        }
        
        // Notify rescue manager
        if (RescueManager.Instance != null)
        {
            RescueManager.Instance.OnVictimRescued();
        }
    }

    public bool IsRescued()
    {
        return isRescued;
    }
    
    /// <summary>
    /// Check if victim has reached safe zone
    /// </summary>
    public bool HasReachedSafeZone()
    {
        return hasReachedSafeZone;
    }
    
    /// <summary>
    /// Called when victim reaches safe zone
    /// This is called by SafeZone when victim enters the safe zone
    /// </summary>
    public void OnReachedSafeZone()
    {
        if (hasReachedSafeZone) return; // Prevent double processing
        
        hasReachedSafeZone = true;
        Debug.Log($"Victim {gameObject.name} reached safe zone!");
        
        // Stop following player
        if (followScript != null)
        {
            followScript.enabled = false;
        }
        
        // Stop movement
        if (rb != null)
        {
            rb.linearVelocity = Vector2.zero;
        }
        
        // Disable collider to prevent further interactions
        Collider2D collider = GetComponent<Collider2D>();
        if (collider != null)
        {
            collider.enabled = false;
        }
        
        // Hide victim visually (optional - fade out effect)
        if (spriteRenderer != null)
        {
            // Can add fade out effect here if needed
            spriteRenderer.color = new Color(spriteRenderer.color.r, spriteRenderer.color.g, spriteRenderer.color.b, 0.5f);
        }
        
        // Destroy victim after a delay to show visual feedback
        // Note: Score is already added in SafeZone, so it's safe to destroy
        Destroy(gameObject, 1f); // Delay to show visual feedback
    }
    
    /// <summary>
    /// Apply slow debuff to victim (called by Shadow enemies)
    /// </summary>
    public void ApplySlow()
    {
        if (slowDebuff != null && !isRescued)
        {
            slowDebuff.ApplySlow(baseMoveSpeed);
        }
    }
    
    /// <summary>
    /// Set move speed (used by VictimSlowDebuff)
    /// </summary>
    public void SetMoveSpeed(float speed)
    {
        currentMoveSpeed = speed;
    }
    
    /// <summary>
    /// Get base move speed (unaffected by debuffs)
    /// </summary>
    public float GetBaseMoveSpeed()
    {
        return baseMoveSpeed;
    }

    private void OnDrawGizmosSelected()
    {
        // Draw rescue range in editor
        Gizmos.color = Color.green;
        Gizmos.DrawWireSphere(transform.position, rescueRange);
    }
}

