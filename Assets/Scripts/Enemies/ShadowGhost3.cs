using System.Collections;
using UnityEngine;

public class ShadowGhost3 : MonoBehaviour, IEnemy
{
    [Header("Movement Settings")]
    [SerializeField] private float normalSpeed = 2.5f; 
    
    [Header("Attack Settings")]
    [SerializeField] private float dashSpeed = 4.5f;
    [SerializeField] private float dashDuration = 1.0f;
    [SerializeField] private float visionReductionRange = 1.5f;
    
    [Header("Visual Effects")]
    [SerializeField] private ParticleSystem smokeEffect;
    [SerializeField] private SpriteRenderer spriteRenderer;
    [SerializeField] private Color shadowColor = new Color(0.4f, 0.2f, 0.5f, 1f); 
    
    private EnemyPathFinding enemyPathfinding;
    private EnemyAI enemyAI;
    private bool isDashing = false;
    private Vector2 dashDirection;
    private Collider2D myCollider;
    private ParticleSystem spawnedSmoke;
    private Transform lockedTarget; 
    private LightFearBehavior lightFear; 
    

    private float baseNormalSpeed;
    private float baseDashSpeed;

    private void Awake()
    {
        enemyPathfinding = GetComponent<EnemyPathFinding>();
        enemyAI = GetComponent<EnemyAI>();
        spriteRenderer = GetComponent<SpriteRenderer>();
        myCollider = GetComponent<Collider2D>();
        lightFear = GetComponent<LightFearBehavior>();
        
  
        if (spriteRenderer != null)
        {
            spriteRenderer.color = shadowColor;
        }
        
       
        baseNormalSpeed = normalSpeed;
        baseDashSpeed = dashSpeed;
   
        ApplyDifficultySettings();
    }
    
    private void OnEnable()
    {

        DifficultyManager.OnDifficultyChanged += OnDifficultyChanged;
    }
    
    private void OnDisable()
    {
    
        DifficultyManager.OnDifficultyChanged -= OnDifficultyChanged;
    }
    
   
    private void ApplyDifficultySettings()
    {
        float multiplier = DifficultyManager.GetDifficultyMultiplier();
        
    
        normalSpeed = baseNormalSpeed * multiplier;
        dashSpeed = baseDashSpeed * multiplier;
        
    
    }
    
    
    private void OnDifficultyChanged(DifficultyManager.Difficulty newDifficulty)
    {
        ApplyDifficultySettings();
        
      
        if (!isDashing && enemyPathfinding != null)
        {
            enemyPathfinding.SetSpeed(normalSpeed);
        }
    }

    private void Start()
    {
     
        if (enemyPathfinding != null)
        {
            enemyPathfinding.SetSpeed(normalSpeed);
        }
        
    
        if (smokeEffect != null && spawnedSmoke == null)
        {
           
            spawnedSmoke = Instantiate(smokeEffect, transform);
            spawnedSmoke.transform.localPosition = Vector3.zero;
            spawnedSmoke.transform.localRotation = Quaternion.Euler(-90, 0, 0);
            
            var main = spawnedSmoke.main;
            main.loop = true;
            main.startSize = 0.3f;
            main.startSpeed = 0.5f;
            main.maxParticles = 20;
            main.startColor = new Color(0.5f, 0.2f, 0.6f, 0.5f);
            
            var emission = spawnedSmoke.emission;
            emission.rateOverTime = 5f;
            
            spawnedSmoke.Play();
        }
    }
    
    private void OnDestroy()
    {
        
        if (spawnedSmoke != null)
        {
            Destroy(spawnedSmoke.gameObject);
        }
    }

    public void Attack()
    {
       
        if (lightFear != null && lightFear.IsFleeing())
        {
            return;
        }
        
        if (!isDashing)
        {
           
            if (enemyAI != null)
            {
                lockedTarget = enemyAI.GetCurrentTarget();
            }
            
           
            if (lockedTarget != null && lockedTarget.CompareTag("Player"))
            {
                if (SFXManager.Instance != null)
                {
                    SFXManager.Instance.PlayGhostSound();
                }
            }
            
            StartCoroutine(DashAttackRoutine());
        }
    }

    private IEnumerator DashAttackRoutine()
    {
        isDashing = true;
        
      
        if (lockedTarget == null)
        {
            isDashing = false;
            yield break;
        }
        
      
        if (enemyPathfinding != null)
        {
            enemyPathfinding.SetSpeed(dashSpeed);
        }
        
        float elapsedTime = 0f;
        
        while (elapsedTime < dashDuration && lockedTarget != null)
        {
         
            Vector2 targetPosition = lockedTarget.position;
            dashDirection = (targetPosition - (Vector2)transform.position).normalized;
            
            if (spriteRenderer != null)
            {
                if (dashDirection.x < 0)
                {
                    spriteRenderer.flipX = true;
                }
                else
                {
                    spriteRenderer.flipX = false;
                }
            }
            
            if (enemyPathfinding != null)
            {
                enemyPathfinding.MoveTo(dashDirection);
            }
            
            float distanceToTarget = Vector2.Distance(transform.position, targetPosition);
            if (distanceToTarget <= visionReductionRange)
            {
                ApplyDebuffToTarget();
            }
            
        elapsedTime += Time.deltaTime;
        yield return null;
    }
    
 
    if (enemyPathfinding != null)
    {
        enemyPathfinding.ResetSpeed();
    }
    
    isDashing = false;
    lockedTarget = null; 
}

    private void ApplyDebuffToTarget()
    {
        if (lockedTarget == null) return;
        

        if (lockedTarget.CompareTag("Player"))
        {
            if (VisionDebuff.Instance != null)
            {
                VisionDebuff.Instance.ApplyVisionReduction();
            }
        }
       
        else if (lockedTarget.CompareTag("Victim"))
        {
            Victim victim = lockedTarget.GetComponent<Victim>();
            if (victim != null)
            {
                victim.ApplySlow();
            }
        }
    }
    
    private void ApplyVisionReduction()
    {
        if (VisionDebuff.Instance != null)
        {
            VisionDebuff.Instance.ApplyVisionReduction();
        }
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
      
        if (collision.gameObject.CompareTag("Player"))
        {
            ApplyVisionReduction();
        }
      
        else if (collision.gameObject.CompareTag("Victim"))
        {
            Victim victim = collision.gameObject.GetComponent<Victim>();
            if (victim != null)
            {
                victim.ApplySlow();
            }
        }
    }

    private void OnCollisionStay2D(Collision2D collision)
    {
        
        if (collision.gameObject.CompareTag("Player"))
        {
            ApplyVisionReduction();
        }
       
        else if (collision.gameObject.CompareTag("Victim"))
        {
            Victim victim = collision.gameObject.GetComponent<Victim>();
            if (victim != null)
            {
                victim.ApplySlow();
            }
        }
    }

    public bool IsDashing()
    {
        return isDashing;
    }
}

