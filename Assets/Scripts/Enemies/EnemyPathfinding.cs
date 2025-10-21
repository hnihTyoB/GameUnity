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
        if (knockback.GettingKnockedBack) { return; }
        rb.MovePosition(rb.position + moveDir * (currentSpeed * Time.fixedDeltaTime));
        if (moveDir.x < 0) { spriteRenderer.flipX = true; }
        else if (moveDir.x > 0) { spriteRenderer.flipX = false; }
    }

    public void MoveTo(Vector2 targetPosition)
    {
        moveDir = targetPosition;
    }
    public void StopMoving()
    {
        moveDir = Vector3.zero;
    }
    
    public void SetSpeed(float speed)
    {
        currentSpeed = speed;
    }
    
    public void ResetSpeed()
    {
        currentSpeed = baseSpeed;
    }
}
