using UnityEngine;

/// <summary>
/// Safe zone where rescued victims must reach to be counted for scoring
/// Only victims that reach this zone will give points
/// Place this before AreaExit to prevent players from just rescuing and leaving
/// </summary>
public class SafeZone : MonoBehaviour
{
    [Header("Safe Zone Settings")]
    [SerializeField] private float checkRadius = 2f; // Radius to check for victims
    [SerializeField] private bool requireAllVictims = false; // If true, require all rescued victims to reach before allowing exit
    
    [Header("Visual Feedback")]
    [SerializeField] private Color gizmoColor = new Color(0f, 1f, 0f, 0.3f); // Green transparent
    [SerializeField] private bool showGizmos = true;
    
    [Header("Statistics")]
    private int victimsInSafeZone = 0;
    private int totalVictimsRescued = 0;
    
    private CircleCollider2D zoneCollider;
    
    private void Awake()
    {
        // Get or create CircleCollider2D for trigger
        zoneCollider = GetComponent<CircleCollider2D>();
        if (zoneCollider == null)
        {
            zoneCollider = gameObject.AddComponent<CircleCollider2D>();
        }
        
        // Configure collider as trigger
        zoneCollider.isTrigger = true;
        zoneCollider.radius = checkRadius;
    }
    
    private void Start()
    {
        // Initialize counts
        victimsInSafeZone = 0;
        totalVictimsRescued = 0;
    }
    
    private void Update()
    {
        // Update total rescued count
        if (RescueManager.Instance != null)
        {
            totalVictimsRescued = RescueManager.Instance.GetVictimsRescued();
        }
    }
    
    /// <summary>
    /// Called when a victim enters the safe zone
    /// </summary>
    private void OnTriggerEnter2D(Collider2D other)
    {
        Victim victim = other.GetComponent<Victim>();
        if (victim == null || !victim.IsRescued()) return;
        
        // Check if victim has already reached safe zone (prevent double processing)
        if (!victim.HasReachedSafeZone())
        {
            // Add points for reaching safe zone (before marking as reached)
            if (ScoreManager.Instance != null)
            {
                ScoreManager.Instance.AddSafeZonePoints();
            }
            
            // Mark victim as reached safe zone (this will disable collider and destroy victim)
            victim.OnReachedSafeZone();
            
            // Increment count
            victimsInSafeZone++;
            
            Debug.Log($"SafeZone: Victim reached safe zone! Total in zone: {victimsInSafeZone}");
        }
    }
    
    /// <summary>
    /// Check if all rescued victims have reached the safe zone
    /// </summary>
    public bool HaveAllVictimsReached()
    {
        if (!requireAllVictims) return true; // Not required, always return true
        
        if (RescueManager.Instance != null)
        {
            int totalRescued = RescueManager.Instance.GetVictimsRescued();
            return victimsInSafeZone >= totalRescued && totalRescued > 0;
        }
        
        return true;
    }
    
    /// <summary>
    /// Get number of victims currently in safe zone
    /// </summary>
    public int GetVictimsInSafeZone()
    {
        return victimsInSafeZone;
    }
    
    /// <summary>
    /// Get total number of rescued victims
    /// </summary>
    public int GetTotalVictimsRescued()
    {
        if (RescueManager.Instance != null)
        {
            return RescueManager.Instance.GetVictimsRescued();
        }
        return 0;
    }
    
    /// <summary>
    /// Reset safe zone counts (called when starting new level)
    /// </summary>
    public void ResetSafeZone()
    {
        victimsInSafeZone = 0;
        totalVictimsRescued = 0;
    }
    
    /// <summary>
    /// Update check radius (called when checkRadius changes in Inspector)
    /// </summary>
    private void OnValidate()
    {
        if (zoneCollider != null)
        {
            zoneCollider.radius = checkRadius;
        }
    }
    
    private void OnDrawGizmos()
    {
        if (!showGizmos) return;
        
        // Draw safe zone radius
        Gizmos.color = gizmoColor;
        Gizmos.DrawWireSphere(transform.position, checkRadius);
        
        // Draw center
        Gizmos.color = Color.green;
        Gizmos.DrawWireSphere(transform.position, 0.2f);
    }
    
    private void OnDrawGizmosSelected()
    {
        if (!showGizmos) return;
        
        // Draw filled circle when selected
        Gizmos.color = new Color(gizmoColor.r, gizmoColor.g, gizmoColor.b, gizmoColor.a * 0.5f);
        Gizmos.DrawSphere(transform.position, checkRadius);
    }
}

