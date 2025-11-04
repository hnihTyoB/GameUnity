using UnityEngine;

/// <summary>
/// Định nghĩa vùng spawn cho Shadow Ghosts
/// Vẽ gizmo trong editor để dễ dàng xem và chỉnh sửa vùng spawn
/// </summary>
public class ShadowSpawnZone : MonoBehaviour
{
    [Header("Spawn Zone Settings")]
    [SerializeField] private Vector2 zoneSize = new Vector2(10f, 10f);
    [SerializeField] private Color gizmoColor = new Color(0.5f, 0f, 0.5f, 0.3f); // Purple transparent
    
    [Header("Spawn Restrictions")]
    [SerializeField] private LayerMask obstacleLayer; // Layers to avoid (walls, obstacles)
    [SerializeField] private float minDistanceFromPlayer = 5f; // Khoảng cách tối thiểu từ player
    [SerializeField] private float spawnCheckRadius = 0.5f; // Bán kính kiểm tra vị trí spawn
    
    /// <summary>
    /// Lấy một vị trí ngẫu nhiên hợp lệ trong zone
    /// </summary>
    public Vector2 GetRandomValidPosition()
    {
        int maxAttempts = 30; // Số lần thử tối đa để tránh vòng lặp vô hạn
        
        for (int i = 0; i < maxAttempts; i++)
        {
            Vector2 randomPos = GetRandomPositionInZone();
            
            if (IsPositionValid(randomPos))
            {
                return randomPos;
            }
        }
        
        // Nếu không tìm được vị trí hợp lệ, trả về center của zone (fallback)
        Debug.LogWarning($"ShadowSpawnZone '{name}': Không tìm được vị trí hợp lệ sau {maxAttempts} lần thử. Sử dụng center.");
        return (Vector2)transform.position;
    }
    
    /// <summary>
    /// Lấy vị trí ngẫu nhiên bất kỳ trong zone (không kiểm tra)
    /// </summary>
    private Vector2 GetRandomPositionInZone()
    {
        float randomX = Random.Range(-zoneSize.x / 2f, zoneSize.x / 2f);
        float randomY = Random.Range(-zoneSize.y / 2f, zoneSize.y / 2f);
        
        Vector2 localPos = new Vector2(randomX, randomY);
        Vector2 worldPos = (Vector2)transform.position + localPos;
        
        return worldPos;
    }
    
    /// <summary>
    /// Kiểm tra xem vị trí có hợp lệ để spawn không
    /// </summary>
    private bool IsPositionValid(Vector2 position)
    {
        // 1. Kiểm tra va chạm với tường/obstacles
        Collider2D hitCollider = Physics2D.OverlapCircle(position, spawnCheckRadius, obstacleLayer);
        if (hitCollider != null)
        {
            return false; // Vị trí bị chặn bởi obstacle
        }
        
        // 2. Kiểm tra khoảng cách từ player
        if (PlayerController.Instance != null)
        {
            float distanceToPlayer = Vector2.Distance(position, PlayerController.Instance.transform.position);
            if (distanceToPlayer < minDistanceFromPlayer)
            {
                return false; // Quá gần player
            }
        }
        
        // 3. Kiểm tra khoảng cách từ victims (tùy chọn)
        Victim[] victims = FindObjectsOfType<Victim>();
        foreach (Victim victim in victims)
        {
            if (!victim.IsRescued())
            {
                float distanceToVictim = Vector2.Distance(position, victim.transform.position);
                if (distanceToVictim < minDistanceFromPlayer * 0.5f) // 50% minDistance cho victim
                {
                    return false; // Quá gần victim
                }
            }
        }
        
        return true; // Vị trí hợp lệ
    }
    
    /// <summary>
    /// Vẽ gizmo trong Scene view để hiển thị spawn zone
    /// </summary>
    private void OnDrawGizmos()
    {
        // Vẽ vùng spawn
        Gizmos.color = gizmoColor;
        Gizmos.DrawCube(transform.position, new Vector3(zoneSize.x, zoneSize.y, 0.1f));
        
        // Vẽ viền
        Gizmos.color = new Color(gizmoColor.r, gizmoColor.g, gizmoColor.b, 1f);
        Gizmos.DrawWireCube(transform.position, new Vector3(zoneSize.x, zoneSize.y, 0.1f));
    }
    
    /// <summary>
    /// Vẽ gizmo khi được chọn trong Scene view
    /// </summary>
    private void OnDrawGizmosSelected()
    {
        // Vẽ min distance from player
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, minDistanceFromPlayer);
        
        // Vẽ spawn check radius tại một số vị trí mẫu
        Gizmos.color = Color.green;
        for (int i = 0; i < 5; i++)
        {
            Vector2 samplePos = GetRandomPositionInZone();
            Gizmos.DrawWireSphere(samplePos, spawnCheckRadius);
        }
    }
    
    public Vector2 GetZoneSize()
    {
        return zoneSize;
    }
    
    public float GetMinDistanceFromPlayer()
    {
        return minDistanceFromPlayer;
    }
}

