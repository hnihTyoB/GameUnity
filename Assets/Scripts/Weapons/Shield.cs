using UnityEngine;
using System.Collections;

/// <summary>
/// Shield weapon - protective barrier around player
/// Blocks damage and provides defense
/// Knocks back enemies on contact
/// Has activation cooldown
/// </summary>
public class Shield : MonoBehaviour, IWeapon
{
    [Header("Shield Settings")]
    [SerializeField] private WeaponInfo weaponInfo;
    [SerializeField] private float knockBackThrust = 18f; // Lực đẩy enemy
    [SerializeField] private float knockBackCooldown = 0.3f; // Cooldown giữa các lần đẩy
    
    [Header("Shield Activation")]
    [SerializeField] private float shieldDuration = 5f; // Thời gian shield hoạt động
    [SerializeField] private float shieldCooldown = 15f; // Thời gian hồi của shield
    
    private SpriteRenderer spriteRenderer;
    private bool isActive = false; // Shield starts inactive
    private Vector3 fixedLocalPosition = new Vector3(0, 0.2f, 0); // Center of player
    private CircleCollider2D shieldCollider;
    private float lastKnockBackTime = -999f;
    
    // Cooldown tracking
    private bool isOnCooldown = false;
    private float cooldownEndTime = 0f;
    private Coroutine activeShieldCoroutine = null;
    
    private void Awake()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
        shieldCollider = GetComponent<CircleCollider2D>();
        
        // Ensure collider exists and is set as trigger
        if (shieldCollider == null)
        {
            shieldCollider = gameObject.AddComponent<CircleCollider2D>();
        }
        shieldCollider.isTrigger = true;
        shieldCollider.radius = 0.5f; // Adjust based on shield sprite size
    }
    
    private void Start()
    {
        // Remove shield from ActiveWeapon transform hierarchy to prevent rotation
        // Shield stays as child of player directly, not weapon system
        if (transform.parent == ActiveWeapon.Instance.transform)
        {
            // Detach from weapon system and attach to player
            Transform playerTransform = PlayerController.Instance.transform;
            transform.SetParent(playerTransform);
        }
        
        // Position shield in front of player (relative to player, not weapon)
        transform.localPosition = fixedLocalPosition;
        transform.localRotation = Quaternion.identity;
        transform.localScale = Vector3.one;
        
        // Force sprite to never flip
        if (spriteRenderer != null)
        {
            spriteRenderer.flipX = false;
            spriteRenderer.flipY = false;
        }
        
        // Shield starts inactive, waiting for player to activate
        SetActive(false);
    }
    
    /// <summary>
    /// Activate shield when player uses attack button
    /// Shield will stay active for duration, then go on cooldown
    /// </summary>
    private IEnumerator ActivateShieldRoutine()
    {
        // Activate shield and start cooldown immediately
        SetActive(true);
        isOnCooldown = true;
        cooldownEndTime = Time.time + shieldCooldown;
        
        // Notify UI to start cooldown display immediately
        ShieldManager.Instance?.OnShieldCooldownStarted(shieldCooldown);
        ShieldManager.Instance?.OnShieldActivated();
        
        // Play shield loop sound
        if (SFXManager.Instance != null)
        {
            SFXManager.Instance.PlayShieldSound();
        }
        
        Debug.Log("Shield: Activated!");
        
        // Shield stays active for duration
        yield return new WaitForSeconds(shieldDuration);
        
        // Deactivate shield (but cooldown continues)
        SetActive(false);
        
        // Stop shield sound
        if (SFXManager.Instance != null)
        {
            SFXManager.Instance.StopShieldSound();
        }
        
        Debug.Log("Shield: Deactivated - Cooldown continues");
        
        // Wait for remaining cooldown time
        float remainingCooldown = shieldCooldown - shieldDuration;
        if (remainingCooldown > 0)
        {
            yield return new WaitForSeconds(remainingCooldown);
        }
        
        // Cooldown finished
        isOnCooldown = false;
        activeShieldCoroutine = null;
        Debug.Log("Shield: Ready to use again!");
    }
    
    private void LateUpdate()
    {
        // Force shield to stay completely fixed
        if (isActive)
        {
            // Keep parent as player, not weapon
            if (transform.parent != PlayerController.Instance.transform)
            {
                transform.SetParent(PlayerController.Instance.transform);
            }
            
            // Lock position relative to player
            transform.localPosition = fixedLocalPosition;
            
            // Lock rotation
            transform.localRotation = Quaternion.identity;
            
            // Lock scale - ensure it's always positive
            Vector3 scale = transform.localScale;
            scale.x = 1f; // Force to 1, never negative
            scale.y = 1f;
            scale.z = 1f;
            transform.localScale = scale;
            
            // Force sprite to never flip
            if (spriteRenderer != null)
            {
                spriteRenderer.flipX = false;
                spriteRenderer.flipY = false;
            }
        }
    }
    
    public WeaponInfo GetWeaponInfo()
    {
        return weaponInfo;
    }
    
    public void Attack()
    {
        // When player presses slot key again (e.g., press "2" twice), activate shield
        // But only if not already on cooldown
        if (!isOnCooldown && activeShieldCoroutine == null)
        {
            activeShieldCoroutine = StartCoroutine(ActivateShieldRoutine());
            Debug.Log("Shield: Activating via slot key press!");
        }
        else if (isOnCooldown)
        {
            float remaining = GetRemainingCooldown();
            Debug.Log($"Shield on cooldown! {remaining:F1}s remaining");
        }
    }
    
    /// <summary>
    /// Activate/deactivate shield
    /// </summary>
    public void SetActive(bool active)
    {
        isActive = active;
        spriteRenderer.enabled = active;
        
        // Also enable/disable collider
        if (shieldCollider != null)
        {
            shieldCollider.enabled = active;
        }
        
        // Stop shield sound when deactivated (safety check)
        if (!active && SFXManager.Instance != null)
        {
            SFXManager.Instance.StopShieldSound();
        }
    }
    
    /// <summary>
    /// Called when weapon is unequipped (switched to another weapon)
    /// Shield persists and stays visible if still active
    /// </summary>
    public void OnWeaponUnequipped()
    {
        // Shield stays visible and active - doesn't hide when switching weapons
        // It will only disappear when its 5s duration ends
        Debug.Log($"Shield: Unequipped but still visible - Active: {isActive}, Cooldown: {isOnCooldown}");
    }
    
    /// <summary>
    /// Called when weapon is re-equipped (switched back to shield)
    /// Shield is already there, just update reference
    /// </summary>
    public void OnWeaponEquipped()
    {
        // Shield was never hidden, so nothing to do here
        Debug.Log($"Shield: Re-equipped - Active: {isActive}, Cooldown: {isOnCooldown}");
    }
    
    /// <summary>
    /// Check if shield is currently on cooldown
    /// </summary>
    public bool IsOnCooldown()
    {
        return isOnCooldown;
    }
    
    /// <summary>
    /// Get remaining cooldown time
    /// </summary>
    public float GetRemainingCooldown()
    {
        if (!isOnCooldown) return 0f;
        return Mathf.Max(0f, cooldownEndTime - Time.time);
    }
    
    /// <summary>
    /// Called when shield blocks damage (implement later)
    /// </summary>
    public void OnBlockDamage()
    {
        Debug.Log("Shield blocked damage!");
        // Add visual/audio feedback here
    }
    
    /// <summary>
    /// Detect collision with enemies and knock them back
    /// </summary>
    private void OnTriggerStay2D(Collider2D other)
    {
        // Check if cooldown has passed
        if (Time.time < lastKnockBackTime + knockBackCooldown)
        {
            return;
        }
        
        // Check if the colliding object is an enemy
        EnemyAI enemy = other.GetComponent<EnemyAI>();
        if (enemy != null)
        {
            // Get enemy's knockback component
            Knockback enemyKnockback = other.GetComponent<Knockback>();
            if (enemyKnockback != null)
            {
                // Knock back the enemy away from shield/player
                enemyKnockback.GetKnockedBack(transform, knockBackThrust);
                
                // Optional: Add flash effect if enemy has it
                Flash enemyFlash = other.GetComponent<Flash>();
                if (enemyFlash != null)
                {
                    StartCoroutine(enemyFlash.FlashRoutine());
                }
                
                lastKnockBackTime = Time.time;
                Debug.Log($"Shield knocked back enemy: {other.name}");
            }
        }
    }
    
    /// <summary>
    /// Also handle OnTriggerEnter2D for immediate response
    /// </summary>
    private void OnTriggerEnter2D(Collider2D other)
    {
        OnTriggerStay2D(other);
    }
}

