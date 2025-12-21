using UnityEngine;
using System.Collections;

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
    
    // Lưu giá trị gốc
    private float baseShieldDuration;
    private float baseShieldCooldown;
    
    private void Awake()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
        shieldCollider = GetComponent<CircleCollider2D>();
        
        if (shieldCollider == null)
        {
            shieldCollider = gameObject.AddComponent<CircleCollider2D>();
        }
        shieldCollider.isTrigger = true;
        shieldCollider.radius = 0.5f;
        
        // Lưu giá trị gốc
        baseShieldDuration = shieldDuration;
        baseShieldCooldown = shieldCooldown;
        
        // Áp dụng difficulty
        ApplyDifficultySettings();
    }
    
    private void Start()
    {

        if (transform.parent == ActiveWeapon.Instance.transform)
        {
       
            Transform playerTransform = PlayerController.Instance.transform;
            transform.SetParent(playerTransform);
        }
        
        transform.localPosition = fixedLocalPosition;
        transform.localRotation = Quaternion.identity;
        transform.localScale = Vector3.one;
        
        if (spriteRenderer != null)
        {
            spriteRenderer.flipX = false;
            spriteRenderer.flipY = false;
        }
        
        SetActive(false);
        
        Debug.Log("Shield: Initialized and attached to Player");
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
    /// Áp dụng difficulty vào shield settings
    /// </summary>
    private void ApplyDifficultySettings()
    {
        float multiplier = DifficultyManager.GetDifficultyMultiplier();
        
        // Duration: Easy (1.4x) = 7s, Normal (1.0x) = 5s, Hard (0.67x) = 3.35s
        shieldDuration = baseShieldDuration / multiplier;
        
        // Cooldown: Easy (0.7x) = 10.5s, Normal (1.0x) = 15s, Hard (1.5x) = 22.5s
        shieldCooldown = baseShieldCooldown * multiplier;
        
        Debug.Log($"Shield: Difficulty applied - Duration: {shieldDuration:F1}s, Cooldown: {shieldCooldown:F1}s");
    }
    
    /// <summary>
    /// Callback khi difficulty thay đổi
    /// </summary>
    private void OnDifficultyChanged(DifficultyManager.Difficulty newDifficulty)
    {
        ApplyDifficultySettings();
        // Note: Nếu shield đang active, settings mới sẽ áp dụng cho lần activate tiếp theo
    }
    
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
        bool managerOnCooldown = ShieldManager.Instance != null && ShieldManager.Instance.IsOnCooldown();
        
 
        if (!managerOnCooldown && !isOnCooldown && activeShieldCoroutine == null)
        {
            activeShieldCoroutine = StartCoroutine(ActivateShieldRoutine());
            Debug.Log("Shield: Activating via slot key press!");
        }
        else if (managerOnCooldown || isOnCooldown)
        {
            float remaining = ShieldManager.Instance != null ? ShieldManager.Instance.GetRemainingCooldown() : GetRemainingCooldown();
            Debug.Log($"Shield on cooldown! {remaining:F1}s remaining");
        }
    }
    

    public void SetActive(bool active)
    {
        isActive = active;
        spriteRenderer.enabled = active;
      
        if (shieldCollider != null)
        {
            shieldCollider.enabled = active;
        }
        
   
        if (!active && SFXManager.Instance != null)
        {
            SFXManager.Instance.StopShieldSound();
        }
    }
    
    public void OnWeaponUnequipped()
    {
    
        Debug.Log($"Shield: Unequipped but still visible - Active: {isActive}, Cooldown: {isOnCooldown}");
    }
    
    public void OnWeaponEquipped()
    {
  
        Debug.Log($"Shield: Re-equipped - Active: {isActive}, Cooldown: {isOnCooldown}");
    }
    

    public bool IsOnCooldown()
    {
        return isOnCooldown;
    }
    

    public float GetRemainingCooldown()
    {
        if (!isOnCooldown) return 0f;
        return Mathf.Max(0f, cooldownEndTime - Time.time);
    }
 
    public void OnBlockDamage()
    {
        Debug.Log("Shield blocked damage!");
        // Add visual/audio feedback here
    }
    
      private void OnTriggerStay2D(Collider2D other)
    {
 
        if (Time.time < lastKnockBackTime + knockBackCooldown)
        {
            return;
        }
        
       
        EnemyAI enemy = other.GetComponent<EnemyAI>();
        if (enemy != null)
        {
          
            Knockback enemyKnockback = other.GetComponent<Knockback>();
            if (enemyKnockback != null)
            {
             
                enemyKnockback.GetKnockedBack(transform, knockBackThrust);
                
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
    private void OnTriggerEnter2D(Collider2D other)
    {
        OnTriggerStay2D(other);
    }
}

