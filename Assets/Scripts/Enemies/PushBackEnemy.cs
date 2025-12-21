using UnityEngine;


public class PushBackEnemy : MonoBehaviour
{
    [Header("Knockback Settings")]
    [SerializeField] private float pushBackForce = 50f;
    [SerializeField] private float pushBackCooldown = 0.3f; 
    
    private float lastPushBackTime = -999f;
    
    private void OnCollisionStay2D(Collision2D collision)
    {
   
        if ((collision.gameObject.CompareTag("Player") || collision.gameObject.CompareTag("Victim")) 
            && Time.time >= lastPushBackTime + pushBackCooldown)
        {
        
            Rigidbody2D targetRb = collision.gameObject.GetComponent<Rigidbody2D>();
            
            if (targetRb != null)
            {
              
                Vector2 pushDirection = (collision.transform.position - transform.position).normalized;
                
             
                targetRb.AddForce(pushDirection * pushBackForce, ForceMode2D.Impulse);
                
                lastPushBackTime = Time.time;
                
            
            }
        }
    }
}

