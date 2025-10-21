using UnityEngine;

/// <summary>
/// PushBackEnemy - Enemy đẩy lùi player nhưng không gây damage
/// Dùng cho Shadow Ghost 2 (Bóng đen bắt nạt)
/// Sử dụng AddForce trực tiếp thay vì Knockback component
/// </summary>
public class PushBackEnemy : MonoBehaviour
{
    [Header("Knockback Settings")]
    [SerializeField] private float pushBackForce = 50f; // Lực đẩy
    [SerializeField] private float pushBackCooldown = 0.3f; // Cooldown giữa các lần đẩy
    
    private float lastPushBackTime = -999f;
    
    private void OnCollisionStay2D(Collision2D collision)
    {
        // Check if colliding with player
        if (collision.gameObject.CompareTag("Player") && Time.time >= lastPushBackTime + pushBackCooldown)
        {
            // Get player's Rigidbody2D
            Rigidbody2D playerRb = collision.gameObject.GetComponent<Rigidbody2D>();
            
            if (playerRb != null)
            {
                // Calculate push direction (away from this enemy)
                Vector2 pushDirection = (collision.transform.position - transform.position).normalized;
                
                // Apply force directly to player
                playerRb.AddForce(pushDirection * pushBackForce, ForceMode2D.Impulse);
                
                lastPushBackTime = Time.time;
                
                Debug.Log($"PushBackEnemy: Pushed player with force {pushBackForce}");
            }
        }
    }
}

