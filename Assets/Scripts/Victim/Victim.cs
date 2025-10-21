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
    
    // Called when victim reaches safe zone
    public void OnReachedSafeZone()
    {
        // Will implement reward system later
        Debug.Log("Victim reached safe zone!");
        // For now, just destroy
        Destroy(gameObject);
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

