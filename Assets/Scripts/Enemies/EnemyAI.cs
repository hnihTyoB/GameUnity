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
    
    // Target locking system
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

        // Only find new target if not locked
        if (!isTargetLocked && canAttack)
        {
            currentTarget = FindNearestTarget();
        }

        // Check if target is in attack range
        if (currentTarget != null && Vector2.Distance(transform.position, currentTarget.position) < attackRange) {
            state = State.Attacking;
        }

        if (timeRoaming > roamChangeDirFloat) {
            roamPosition = GetRoamingPosition();
        }
    }
    private void Attacking() {
        // Check if target is still valid and in range
        if (currentTarget == null || Vector2.Distance(transform.position, currentTarget.position) > attackRange)
        {
            currentTarget = null;
            state = State.Roaming;
        }

        if (attackRange != 0 && canAttack && currentTarget != null) {

            canAttack = false;
            LockTarget(); // Lock target when starting attack
            (enemyType as IEnemy).Attack();

            if (stopMovingWhileAttacking) {
                enemyPathfinding.StopMoving();
            } else {
                enemyPathfinding.MoveTo(roamPosition);
            }

            StartCoroutine(AttackCooldownRoutine());
        }
    }
    private IEnumerator AttackCooldownRoutine() {
        yield return new WaitForSeconds(attackCooldown);
        canAttack = true;
        UnlockTarget(); // Unlock target after cooldown, allowing new target selection
    }
    private Vector2 GetRoamingPosition()
    {
        timeRoaming = 0f;
        return new Vector2(Random.Range(-1f, 1f), Random.Range(-1f, 1f)).normalized;
    }
    
    /// <summary>
    /// Find nearest target (Player or Victim) within attack range
    /// </summary>
    private Transform FindNearestTarget()
    {
        Transform nearestTarget = null;
        float nearestDistance = float.MaxValue;
        
        // Check Player
        if (PlayerController.Instance != null)
        {
            float distanceToPlayer = Vector2.Distance(transform.position, PlayerController.Instance.transform.position);
            if (distanceToPlayer < nearestDistance)
            {
                nearestDistance = distanceToPlayer;
                nearestTarget = PlayerController.Instance.transform;
            }
        }
        
        // Check all Victims
        Victim[] victims = FindObjectsOfType<Victim>();
        foreach (Victim victim in victims)
        {
            // Only target victims that are not rescued yet
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
    
    /// <summary>
    /// Lock current target to prevent switching during attack
    /// </summary>
    public void LockTarget()
    {
        isTargetLocked = true;
    }
    
    /// <summary>
    /// Unlock target to allow new target selection
    /// </summary>
    public void UnlockTarget()
    {
        isTargetLocked = false;
    }
    
    /// <summary>
    /// Get current locked target
    /// </summary>
    public Transform GetCurrentTarget()
    {
        return currentTarget;
    }
}
