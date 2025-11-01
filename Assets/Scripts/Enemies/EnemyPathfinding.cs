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
    private float speedMultiplier = 1f; // For flashlight slow effect
    private bool isMovementEnabled = true; // For flashlight stun effect
    private void Awake()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
        knockback = GetComponent<Knockback>();
        rb = GetComponent<Rigidbody2D>();
        baseSpeed = moveSpeed;
        currentSpeed = moveSpeed;
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
    
    /// <summary>
    /// Set speed multiplier for effects like flashlight slow
    /// </summary>
    public void SetSpeedMultiplier(float multiplier)
    {
        speedMultiplier = multiplier;
    }
    
    /// <summary>
    /// Stop enemy movement completely (for stun)
    /// </summary>
    public void StopMoving()
    {
        isMovementEnabled = false;
        moveDir = Vector2.zero;
    }
    
    /// <summary>
    /// Resume enemy movement (after stun)
    /// </summary>
    public void ResumeMoving()
    {
        isMovementEnabled = true;
    }
    
    /// <summary>
    /// Get current move speed
    /// </summary>
    public float GetMoveSpeed()
    {
        return currentSpeed;
    }
    
    /// <summary>
    /// Get base move speed
    /// </summary>
    public float GetBaseSpeed()
    {
        return baseSpeed;
    }
}
