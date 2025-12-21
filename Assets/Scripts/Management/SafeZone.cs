using UnityEngine;


public class SafeZone : MonoBehaviour
{
    [Header("Safe Zone Settings")]
    [SerializeField] private float checkRadius = 2f;
    [SerializeField] private bool requireAllVictims = false; 
    
    [Header("Visual Feedback")]
    [SerializeField] private Color gizmoColor = new Color(0f, 1f, 0f, 0.3f); 
    [SerializeField] private bool showGizmos = true;
    
    [Header("Statistics")]
    private int victimsInSafeZone = 0;
    private int totalVictimsRescued = 0;
    
    private CircleCollider2D zoneCollider;
    
    private void Awake()
    {
       
        zoneCollider = GetComponent<CircleCollider2D>();
        if (zoneCollider == null)
        {
            zoneCollider = gameObject.AddComponent<CircleCollider2D>();
        }
        
   
        zoneCollider.isTrigger = true;
        zoneCollider.radius = checkRadius;
    }
    
    private void Start()
    {
   
        victimsInSafeZone = 0;
        totalVictimsRescued = 0;
    }
    
    private void Update()
    {

        if (RescueManager.Instance != null)
        {
            totalVictimsRescued = RescueManager.Instance.GetVictimsRescued();
        }
    }
    

    private void OnTriggerEnter2D(Collider2D other)
    {
        Victim victim = other.GetComponent<Victim>();
        if (victim == null || !victim.IsRescued()) return;
        
    
        if (!victim.HasReachedSafeZone())
        {
            if (ScoreManager.Instance != null)
            {
                ScoreManager.Instance.AddSafeZonePoints();
            }
            
            victim.OnReachedSafeZone();
            
            victimsInSafeZone++;
            
        }
    }
    
   
    public bool HaveAllVictimsReached()
    {
        if (!requireAllVictims) return true; 
        
        if (RescueManager.Instance != null)
        {
            int totalRescued = RescueManager.Instance.GetVictimsRescued();
            return victimsInSafeZone >= totalRescued && totalRescued > 0;
        }
        
        return true;
    }
    

    public int GetVictimsInSafeZone()
    {
        return victimsInSafeZone;
    }
    

    public int GetTotalVictimsRescued()
    {
        if (RescueManager.Instance != null)
        {
            return RescueManager.Instance.GetVictimsRescued();
        }
        return 0;
    }
    
 
    public void ResetSafeZone()
    {
        victimsInSafeZone = 0;
        totalVictimsRescued = 0;
    }
    

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
        
        Gizmos.color = gizmoColor;
        Gizmos.DrawWireSphere(transform.position, checkRadius);
        
        Gizmos.color = Color.green;
        Gizmos.DrawWireSphere(transform.position, 0.2f);
    }
    
    private void OnDrawGizmosSelected()
    {
        if (!showGizmos) return;
        
        Gizmos.color = new Color(gizmoColor.r, gizmoColor.g, gizmoColor.b, gizmoColor.a * 0.5f);
        Gizmos.DrawSphere(transform.position, checkRadius);
    }
}

