using UnityEngine;

/// <summary>
/// Shield weapon - protective barrier around player
/// Blocks damage and provides defense
/// </summary>
public class Shield : MonoBehaviour, IWeapon
{
    [Header("Shield Settings")]
    [SerializeField] private WeaponInfo weaponInfo;
    
    private SpriteRenderer spriteRenderer;
    private bool isActive = true;
    private Vector3 fixedLocalPosition = new Vector3(0, 0.2f, 0); // Center of player
    
    private void Awake()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
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
}

