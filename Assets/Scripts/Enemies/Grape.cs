using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Grape : MonoBehaviour, IEnemy
{
    [SerializeField] private GameObject grapeProjectilePrefab;
    [SerializeField] private float maxAttackRange = 5f; 

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

    
        Transform currentTarget = enemyAI.GetCurrentTarget();
        if (currentTarget != null)
        {
            if (transform.position.x - currentTarget.position.x < 0) {
                spriteRenderer.flipX = false;
            } else {
                spriteRenderer.flipX = true;
            }
        }
    }

    public void SpawnProjectileAnimEvent() {
     
        Transform currentTarget = enemyAI.GetCurrentTarget();
        
        if (currentTarget != null)
        {
            float distanceToTarget = Vector2.Distance(transform.position, currentTarget.position);
            
            if (distanceToTarget <= maxAttackRange)
            {
             
                if (currentTarget.CompareTag("Player"))
                {
                    if (SFXManager.Instance != null)
                    {
                        SFXManager.Instance.PlayShootSound();
                    }
                }
                
      
                GameObject projectile = Instantiate(grapeProjectilePrefab, transform.position, Quaternion.identity);
                GrapeProjectile grapeProj = projectile.GetComponent<GrapeProjectile>();
                if (grapeProj != null)
                {
                    grapeProj.SetTarget(currentTarget.position);
                }
            }
        }
    }
}
