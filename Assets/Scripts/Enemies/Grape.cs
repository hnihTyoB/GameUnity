using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Grape : MonoBehaviour, IEnemy
{
    [SerializeField] private GameObject grapeProjectilePrefab;
    [SerializeField] private float maxAttackRange = 5f; // Max range to spawn projectile

    private Animator myAnimator;
    private SpriteRenderer spriteRenderer;
    private EnemyAI enemyAI;

    readonly int ATTACK_HASH = Animator.StringToHash("Attack");

    private void Awake() {
        myAnimator = GetComponent<Animator>();
        spriteRenderer = GetComponent<SpriteRenderer>();
        enemyAI = GetComponent<EnemyAI>();
    }

    public void Attack() {
        myAnimator.SetTrigger(ATTACK_HASH);

        if (transform.position.x - PlayerController.Instance.transform.position.x < 0) {
            spriteRenderer.flipX = false;
        } else {
            spriteRenderer.flipX = true;
        }
    }

    public void SpawnProjectileAnimEvent() {
        // Check if player is still in range before spawning projectile
        if (PlayerController.Instance != null)
        {
            float distanceToPlayer = Vector2.Distance(transform.position, PlayerController.Instance.transform.position);
            
            if (distanceToPlayer <= maxAttackRange)
            {
                Debug.Log($"{gameObject.name}: Spawning projectile. Distance={distanceToPlayer:F2}");
                Instantiate(grapeProjectilePrefab, transform.position, Quaternion.identity);
            }
            else
            {
                Debug.Log($"{gameObject.name}: Projectile spawn CANCELLED - player too far ({distanceToPlayer:F2} > {maxAttackRange})");
            }
        }
    }
}
