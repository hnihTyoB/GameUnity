using UnityEngine;

/// <summary>
/// Shield weapon - protective barrier around player
/// Blocks damage and provides defense
/// Knocks back enemies on contact
/// </summary>
public class Shield : MonoBehaviour, IWeapon
{
    [Header("Shield Settings")]
    [SerializeField] private WeaponInfo weaponInfo;
    [SerializeField] private float knockBackThrust = 18f; // Lực đẩy enemy
    [SerializeField] private float knockBackCooldown = 0.3f; // Cooldown giữa các lần đẩy
    
    private SpriteRenderer spriteRenderer;
    private bool isActive = true;
    private Vector3 fixedLocalPosition = new Vector3(0, 0.2f, 0); // Center of player
    private CircleCollider2D shieldCollider;
    private float lastKnockBackTime = -999f;
    
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
        // Shield doesn't attack, it defends
        // This method is required by IWeapon interface but not used
    }
    
    /// <summary>
    /// Activate/deactivate shield
    /// </summary>
    public void SetActive(bool active)
    {
        isActive = active;
        spriteRenderer.enabled = active;
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

