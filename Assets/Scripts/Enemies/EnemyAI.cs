using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyAI : MonoBehaviour
{
    [SerializeField] private float roamChangeDirFloat = 2f;
    [SerializeField] private float attackRange = 0f;
    [SerializeField] private MonoBehaviour enemyType;
    [SerializeField] private float attackCooldown = 2f;
    [SerializeField] private bool stopMovingWhileAttacking = false;
    [SerializeField] private float preferredAttackDistance = 0f; // 0 = move to target, >0 = keep this distance

    private bool canAttack = true;
    private enum State
    {
        Roaming,
        Attacking
    }
    private Vector2 roamPosition;
    private float timeRoaming = 0f;
    private State state;
    private EnemyPathFinding enemyPathfinding;
    

    private Transform currentTarget;
    private bool isTargetLocked = false;
    
    private void Awake()
    {
        enemyPathfinding = GetComponent<EnemyPathFinding>();
        state = State.Roaming;
    }
    private void Start()
    {
        roamPosition = GetRoamingPosition();
    }
    private void Update()
    {
        MovementStateControl();
    }
    private void MovementStateControl() {
        switch (state)
        {
            default:
            case State.Roaming:
                Roaming();
            break;

            case State.Attacking:
                Attacking();
            break;
        }
    }
    private void Roaming() {
        timeRoaming += Time.deltaTime;

        enemyPathfinding.MoveTo(roamPosition);


        if (!isTargetLocked && canAttack)
        {
            currentTarget = FindNearestTarget();
        }

    
        if (currentTarget != null)
        {
            float distanceToTarget = Vector2.Distance(transform.position, currentTarget.position);
            if (distanceToTarget < attackRange)
            {
                state = State.Attacking;
            }
        }

        if (timeRoaming > roamChangeDirFloat) {
            roamPosition = GetRoamingPosition();
        }
    }
    private void Attacking() {
        float distanceToTarget = currentTarget != null ? Vector2.Distance(transform.position, currentTarget.position) : float.MaxValue;
        
        if (currentTarget == null || distanceToTarget > attackRange)
        {
            Animator animator = GetComponent<Animator>();
            if (animator != null)
            {
                animator.ResetTrigger("Attack");
                animator.SetFloat("moveX", 0);
                animator.SetFloat("moveY", 0);
            }
            
            currentTarget = null;
            state = State.Roaming;
            return; 
        }

    
        if (!stopMovingWhileAttacking && currentTarget != null) {
        
            if (preferredAttackDistance > 0f)
            {
                float tolerance = 0.5f;
                
    
                if (distanceToTarget > preferredAttackDistance + tolerance)
                {
                    Vector2 directionToTarget = (currentTarget.position - transform.position).normalized;
                    enemyPathfinding.MoveTo(directionToTarget);
                }
             
                else
                {
                    enemyPathfinding.MoveTo(Vector2.zero);
                }
            }
            else
            {
           
                Vector2 directionToTarget = (currentTarget.position - transform.position).normalized;
                enemyPathfinding.MoveTo(directionToTarget);
            }
        }

     
        if (attackRange != 0 && canAttack && currentTarget != null) {
      
            float currentDistance = Vector2.Distance(transform.position, currentTarget.position);
            
         
            if (currentDistance <= attackRange)
            {
                
                canAttack = false;
                LockTarget(); 
                (enemyType as IEnemy).Attack();

                if (stopMovingWhileAttacking) {
                    enemyPathfinding.StopMoving();
                }

                StartCoroutine(AttackCooldownRoutine());
            }
        
        }
    }
    private IEnumerator AttackCooldownRoutine() {
        yield return new WaitForSeconds(attackCooldown);
        canAttack = true;
        UnlockTarget();
    }
    private Vector2 GetRoamingPosition()
    {
        timeRoaming = 0f;
        return new Vector2(Random.Range(-1f, 1f), Random.Range(-1f, 1f)).normalized;
    }
    
 
    private Transform FindNearestTarget()
    {
        Transform nearestTarget = null;
        float nearestDistance = float.MaxValue;
        
   
        if (PlayerController.Instance != null)
        {
            float distanceToPlayer = Vector2.Distance(transform.position, PlayerController.Instance.transform.position);
            if (distanceToPlayer < nearestDistance)
            {
                nearestDistance = distanceToPlayer;
                nearestTarget = PlayerController.Instance.transform;
            }
        }
        
  
        Victim[] victims = FindObjectsOfType<Victim>();
        foreach (Victim victim in victims)
        {
     
            if (!victim.IsRescued())
            {
                float distanceToVictim = Vector2.Distance(transform.position, victim.transform.position);
                if (distanceToVictim < nearestDistance)
                {
                    nearestDistance = distanceToVictim;
                    nearestTarget = victim.transform;
                }
            }
        }
        
        return nearestTarget;
    }
    
    
    public void LockTarget()
    {
        isTargetLocked = true;
    }
    
  
    public void UnlockTarget()
    {
        isTargetLocked = false;
    }
    
  
    public Transform GetCurrentTarget()
    {
        return currentTarget;
    }
}
