using UnityEngine;


public class ShadowSpawnZone : MonoBehaviour
{
    [Header("Spawn Points")]
    [Tooltip("Danh sách các điểm spawn cố định. Tạo Empty GameObjects làm spawn points.")]
    [SerializeField] private Transform[] spawnPoints;
    
    [Header("Spawn Restrictions")]
    [SerializeField] private float minDistanceFromPlayer = 5f;
    [SerializeField] private float minDistanceFromVictim = 2.5f;
    
    [Header("Gizmos")]
    [SerializeField] private Color gizmoColor = new Color(0.5f, 0f, 0.5f, 0.8f);
    [SerializeField] private bool showGizmos = true;
    

    public Vector2 GetRandomValidPosition()
    {
        if (spawnPoints == null || spawnPoints.Length == 0)
        {
            return Vector2.zero;
        }
        
        int maxAttempts = Mathf.Min(spawnPoints.Length * 2, 20);
        
        for (int i = 0; i < maxAttempts; i++)
        {
            Transform randomPoint = spawnPoints[Random.Range(0, spawnPoints.Length)];
            
            if (randomPoint == null) continue;
            
            Vector2 position = randomPoint.position;
            
            if (IsPositionValid(position))
            {
                return position;
            }
        }
        
        return Vector2.zero;
    }
    

    private bool IsPositionValid(Vector2 position)
    {
        
        if (PlayerController.Instance != null)
        {
            float distanceToPlayer = Vector2.Distance(position, PlayerController.Instance.transform.position);
            if (distanceToPlayer < minDistanceFromPlayer)
            {
                return false;
            }
        }
        
  
        Victim[] victims = FindObjectsOfType<Victim>();
        foreach (Victim victim in victims)
        {
            if (victim != null && !victim.IsRescued())
            {
                float distanceToVictim = Vector2.Distance(position, victim.transform.position);
                if (distanceToVictim < minDistanceFromVictim)
                {
                    return false;
                }
            }
        }
        
        return true;
    }
    
    private void OnDrawGizmos()
    {
        if (!showGizmos || spawnPoints == null) return;
        

        Gizmos.color = gizmoColor;
        Gizmos.DrawWireSphere(transform.position, 0.5f);
        

        foreach (Transform point in spawnPoints)
        {
            if (point == null) continue;
            
            Gizmos.color = Color.cyan;
            Gizmos.DrawSphere(point.position, 0.3f);
            
            Gizmos.color = gizmoColor;
            Gizmos.DrawLine(transform.position, point.position);
        }
    }
    
    private void OnDrawGizmosSelected()
    {
        if (spawnPoints == null) return;
        
  
        Gizmos.color = new Color(1f, 1f, 0f, 0.1f);
        foreach (Transform point in spawnPoints)
        {
            if (point == null) continue;
            Gizmos.DrawWireSphere(point.position, minDistanceFromPlayer);
        }
    }
    
    public int GetSpawnPointCount()
    {
        return spawnPoints != null ? spawnPoints.Length : 0;
    }
}
