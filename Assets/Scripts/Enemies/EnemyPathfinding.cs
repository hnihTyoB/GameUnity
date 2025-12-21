using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyPathFinding : MonoBehaviour
{
    [SerializeField] private float moveSpeed = 2f;

    private Rigidbody2D rb;
    private Vector2 moveDir;
    private Knockback knockback;
    private SpriteRenderer spriteRenderer;
    private float currentSpeed;
    private float baseSpeed;
    private float baseMoveSpeed; 
    private float speedMultiplier = 1f; 
    private bool isMovementEnabled = true; 
    
    private void Awake()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
        knockback = GetComponent<Knockback>();
        rb = GetComponent<Rigidbody2D>();
        

        baseMoveSpeed = moveSpeed;
        
        
        ApplyDifficultySettings();
    }
    

    private void ApplyDifficultySettings()
    {
        float multiplier = DifficultyManager.GetDifficultyMultiplier();
        baseSpeed = baseMoveSpeed * multiplier;
        currentSpeed = baseSpeed;
        
        Debug.Log($"EnemyPathFinding: Difficulty applied - Speed: {baseSpeed:F2} (base: {baseMoveSpeed:F2}, multiplier: {multiplier:F2}x)");
    }
    

    public void ApplyDifficultyMultiplier(float multiplier)
    {
        baseSpeed = baseMoveSpeed * multiplier;
      
        if (speedMultiplier == 1f)
        {
            currentSpeed = baseSpeed;
        }
    }

    private void FixedUpdate()
    {
        if (knockback.GettingKnockedBack || !isMovementEnabled) { return; }
        rb.MovePosition(rb.position + moveDir * (currentSpeed * speedMultiplier * Time.fixedDeltaTime));
        if (moveDir.x < 0) { spriteRenderer.flipX = true; }
        else if (moveDir.x > 0) { spriteRenderer.flipX = false; }
    }

    public void MoveTo(Vector2 targetPosition)
    {
        moveDir = targetPosition;
    }
    
    public void SetSpeed(float speed)
    {
        currentSpeed = speed;
    }
    
    public void ResetSpeed()
    {
        currentSpeed = baseSpeed;
    }
    

    public void SetSpeedMultiplier(float multiplier)
    {
        speedMultiplier = multiplier;
    }
    
   
    public void StopMoving()
    {
        isMovementEnabled = false;
        moveDir = Vector2.zero;
    }
    
    
    public void ResumeMoving()
    {
        isMovementEnabled = true;
    }
    
 
    public float GetMoveSpeed()
    {
        return currentSpeed;
    }
    
  
    public float GetBaseSpeed()
    {
        return baseSpeed;
    }
}
