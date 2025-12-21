using System.Collections;
using UnityEngine;


public class ShadowGhost2 : MonoBehaviour, IEnemy
{
    [Header("Blocking Settings")]
    [SerializeField] private float blockRange = 3.0f; // Tầm phát hiện để block
    [SerializeField] private float blockDuration = 1.5f; // Thời gian Shadow Ghost 2 chuyển đỏ
    [SerializeField] private float blockCooldown = 5.0f; // Cooldown giữa các lần block
    [SerializeField] private float playerFlashDuration = 0.5f; // Flash effect trên player khi bị đẩy
    
    [Header("Movement Settings")]
    [SerializeField] private float normalSpeed = 1.5f; // Tốc độ bình thường
    [SerializeField] private float blockSpeed = 0.8f; // Tốc độ khi block (chậm hơn)
    [SerializeField] private float dashSpeed = 4.0f; // Tốc độ khi lao vào player
    [SerializeField] private float dashDuration = 1.0f; // Thời gian dash
    
    [Header("Visual Effects")]
    [SerializeField] private ParticleSystem smokeEffect;
    [SerializeField] private SpriteRenderer spriteRenderer;
    [SerializeField] private Color shadowColor = new Color(0.2f, 0.1f, 0.3f, 1f); // Màu tím đậm hơn
    
    [Header("Blocking Visual")]
    [SerializeField] private GameObject blockEffect; // Effect khi block
    [SerializeField] private Color blockColor = new Color(0.8f, 0.2f, 0.2f, 0.7f); // Màu đỏ khi block
    
    private EnemyPathFinding enemyPathfinding;
    private EnemyAI enemyAI;
    private bool isBlocking = false;
    private bool canBlock = true;
    private bool isDashing = false;
    private Vector2 blockPosition;
    private Coroutine blockCoroutine;
    private Coroutine dashCoroutine;
    private ParticleSystem spawnedSmoke;
    private Transform lockedTarget; 
    private LightFearBehavior lightFear; 
    

    private float baseNormalSpeed;
    private float baseBlockSpeed;
    private float baseDashSpeed;
    
    private void Awake()
    {
        enemyPathfinding = GetComponent<EnemyPathFinding>();
        enemyAI = GetComponent<EnemyAI>();
        spriteRenderer = GetComponent<SpriteRenderer>();
        lightFear = GetComponent<LightFearBehavior>();
        
       
        if (spriteRenderer != null)
        {
            spriteRenderer.color = shadowColor;
        }
        
        // Lưu giá trị gốc
        baseNormalSpeed = normalSpeed;
        baseBlockSpeed = blockSpeed;
        baseDashSpeed = dashSpeed;
        
        // Áp dụng difficulty
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
        
        // Speed: Easy (0.7x) = chậm hơn, Hard (1.5x) = nhanh hơn
        normalSpeed = baseNormalSpeed * multiplier;
        blockSpeed = baseBlockSpeed * multiplier;
        dashSpeed = baseDashSpeed * multiplier;
        
        Debug.Log($"ShadowGhost2: Difficulty applied - Normal: {normalSpeed:F2}, Block: {blockSpeed:F2}, Dash: {dashSpeed:F2}");
    }
    
   
    private void OnDifficultyChanged(DifficultyManager.Difficulty newDifficulty)
    {
        ApplyDifficultySettings();
        
        // Cập nhật speed nếu không đang dash/block
        if (!isDashing && !isBlocking && enemyPathfinding != null)
        {
            enemyPathfinding.SetSpeed(normalSpeed);
        }
        // Nếu đang block, cập nhật blockSpeed
        else if (isBlocking && enemyPathfinding != null)
        {
            enemyPathfinding.SetSpeed(blockSpeed);
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
            main.startSize = 0.4f; // Lớn hơn Shadow Ghost 1
            main.startSpeed = 0.3f; // Chậm hơn
            main.maxParticles = 15;
            
            var emission = spawnedSmoke.emission;
            emission.rateOverTime = 3f; // Ít hơn Shadow Ghost 1
            
            spawnedSmoke.Play();
        }
    }
    
    private void Update()
    {
   
    }
    
    private void StartBlocking()
    {
        if (isBlocking) return;
        
        isBlocking = true;
        canBlock = false;
        
     
        blockPosition = transform.position;
        
       
        if (spriteRenderer != null)
        {
            spriteRenderer.color = blockColor;
        }
        

        if (enemyPathfinding != null)
        {
            enemyPathfinding.SetSpeed(blockSpeed);
        }
        

        blockCoroutine = StartCoroutine(BlockingRoutine());
    }
    
    private IEnumerator BlockingRoutine()
    {
     
        yield return new WaitForSeconds(blockDuration);
        
    
        StopBlocking();
        
 
        yield return new WaitForSeconds(blockCooldown);
        
      
        canBlock = true;
    }
    
    private void StopBlocking()
    {
        isBlocking = false;
        
     
        if (spriteRenderer != null)
        {
            spriteRenderer.color = shadowColor;
        }
        
   
        if (enemyPathfinding != null)
        {
            enemyPathfinding.ResetSpeed();
        }
        
  
    }
    
    public void Attack()
    {
    
        if (lightFear != null && lightFear.IsFleeing())
        {
            return;
        }
        

        if (!isDashing && !isBlocking)
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
            Vector2 dashDirection = (targetPosition - (Vector2)transform.position).normalized;
            
      
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
            
            elapsedTime += Time.deltaTime;
            yield return null;
        }
        

    if (lockedTarget != null)
    {
        float distanceToTarget = Vector2.Distance(transform.position, lockedTarget.position);

        if (distanceToTarget <= blockRange && canBlock && lockedTarget.CompareTag("Player"))
        {
            StartBlocking();
        }
    }
    
   
    if (enemyPathfinding != null)
    {
        enemyPathfinding.ResetSpeed();
    }
    
    isDashing = false;
    lockedTarget = null;
}
    
    private void OnDestroy()
    {
    
        if (spawnedSmoke != null)
        {
            Destroy(spawnedSmoke.gameObject);
        }
        

        if (blockCoroutine != null)
        {
            StopCoroutine(blockCoroutine);
        }
        
        if (dashCoroutine != null)
        {
            StopCoroutine(dashCoroutine);
        }
    }
    
    private void OnCollisionEnter2D(Collision2D collision)
    {
  
    }

    private void OnCollisionStay2D(Collision2D collision)
    {
  
    }
    
  
    public bool IsBlocking()
    {
        return isBlocking;
    }
    
    public bool IsDashing()
    {
        return isDashing;
    }
    
    public Vector2 GetBlockPosition()
    {
        return blockPosition;
    }
    
    public float GetBlockRange()
    {
        return blockRange;
    }
}
