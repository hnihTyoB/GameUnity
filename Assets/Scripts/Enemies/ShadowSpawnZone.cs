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
    [SerializeField] private float spawnCheckRadius = 1.0f; // Bán kính kiểm tra vị trí spawn (shadow collider ~0.84, nên dùng 1.0-1.5)
    
    /// <summary>
    /// Lấy một vị trí ngẫu nhiên hợp lệ trong zone
    /// </summary>
    public Vector2 GetRandomValidPosition()
    {
        int maxAttempts = 50; // Tăng số lần thử lên 50
        
        for (int i = 0; i < maxAttempts; i++)
        {
            Vector2 randomPos = GetRandomPositionInZone();
            
            if (IsPositionValid(randomPos))
            {
                return randomPos;
            }
        }
        
        // Nếu không tìm được vị trí hợp lệ, trả về Vector2.zero và skip spawn
        Debug.LogWarning($"ShadowSpawnZone '{name}': Không tìm được vị trí hợp lệ. Zone có thể quá chật.");
        return Vector2.zero;
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
    
    public LayerMask GetObstacleLayer()
    {
        return obstacleLayer;
    }
    
    public float GetSpawnCheckRadius()
    {
        return spawnCheckRadius;
    }
    
    /// <summary>
    /// Validate zone setup (gọi từ Editor hoặc runtime để check)
    /// </summary>
    public bool ValidateSetup(out string errorMessage)
    {
        errorMessage = "";
        
        // Check 1: Obstacle Layer có được set không
        if (obstacleLayer.value == 0)
        {
            errorMessage = $"⚠️ Zone '{name}': Obstacle Layer chưa được set! Hãy chọn layers cần tránh (Ground, Wall, v.v.)";
            return false;
        }
        
        // Check 2: Zone size hợp lý không
        if (zoneSize.x < 2f || zoneSize.y < 2f)
        {
            errorMessage = $"⚠️ Zone '{name}': Zone size quá nhỏ ({zoneSize}). Nên ít nhất 5x5.";
            return false;
        }
        
        // Check 3: Spawn check radius hợp lý không
        if (spawnCheckRadius < 0.8f)
        {
            errorMessage = $"⚠️ Zone '{name}': Spawn Check Radius quá nhỏ ({spawnCheckRadius}). Shadow collider ~0.84, nên dùng 1.0-1.5.";
            return false;
        }
        
        // Check 4: Test vài vị trí xem có tìm được không
        int validPositions = 0;
        for (int i = 0; i < 10; i++)
        {
            Vector2 testPos = GetRandomPositionInZone();
            if (IsPositionValid(testPos))
            {
                validPositions++;
            }
        }
        
        if (validPositions == 0)
        {
            errorMessage = $"❌ Zone '{name}': KHÔNG tìm được vị trí hợp lệ nào! Zone có thể bị block hoàn toàn hoặc setup sai.";
            return false;
        }
        
        if (validPositions < 3)
        {
            errorMessage = $"⚠️ Zone '{name}': Chỉ tìm được {validPositions}/10 vị trí hợp lệ. Zone có thể quá chật.";
            return false;
        }
        
        errorMessage = $"✓ Zone '{name}': Setup OK! Tìm được {validPositions}/10 vị trí hợp lệ.";
        return true;
    }
}

