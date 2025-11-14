using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyHealth : MonoBehaviour
{
    [SerializeField] private int startingHealth = 3;
    [SerializeField] private GameObject deathVFXPrefab;
    [SerializeField] private float knockBackThrust = 15f;

    private int currentHealth;
    private int baseStartingHealth; // Lưu giá trị gốc
    private Knockback knockback;
    private Flash flash;
    
    private void Awake()
    {
        flash = GetComponent<Flash>();
        knockback = GetComponent<Knockback>();
        
        // Lưu giá trị gốc
        baseStartingHealth = startingHealth;
    }

    private void Start()
    {
        // Áp dụng difficulty multiplier vào health
        ApplyDifficultyMultiplier();
    }
    
    /// <summary>
    /// Áp dụng difficulty multiplier vào enemy health
    /// </summary>
    private void ApplyDifficultyMultiplier()
    {
        float multiplier = DifficultyManager.GetDifficultyMultiplier();
        int adjustedHealth = Mathf.RoundToInt(baseStartingHealth * multiplier);
        currentHealth = adjustedHealth;
        
        Debug.Log($"EnemyHealth: Difficulty applied - Health: {currentHealth} (base: {baseStartingHealth}, multiplier: {multiplier:F2}x)");
    }
    
    /// <summary>
    /// Public method để cập nhật difficulty trong runtime (nếu enemy chưa bị damage)
    /// </summary>
    public void UpdateDifficultyMultiplier()
    {
        // Chỉ áp dụng nếu enemy còn full health (chưa bị damage)
        if (currentHealth >= baseStartingHealth || currentHealth == Mathf.RoundToInt(baseStartingHealth * DifficultyManager.GetDifficultyMultiplier()))
        {
            ApplyDifficultyMultiplier();
        }
    }

    public void TakeDamage(int damage)
    {
        currentHealth -= damage;
        knockback.GetKnockedBack(PlayerController.Instance.transform, knockBackThrust);
        StartCoroutine(flash.FlashRoutine());
        
        // Add hit penalty to score
        Debug.Log($"EnemyHealth: TakeDamage called on {gameObject.name}, ScoreManager.Instance = {(ScoreManager.Instance != null ? "EXISTS" : "NULL")}");
        if (ScoreManager.Instance != null)
        {
            ScoreManager.Instance.AddHitPoints();
        }
        else
        {
            Debug.LogError("EnemyHealth: ScoreManager.Instance is NULL! Cannot add hit penalty.");
        }
        
        StartCoroutine(CheckDetectDeathRoutine());
    }

    private IEnumerator CheckDetectDeathRoutine()
    {
        yield return new WaitForSeconds(flash.GetRestoreMatTime());
        DetectDeath();
    }

    public void DetectDeath() {
        if (currentHealth <= 0) {
            // Add kill penalty to score (before destroying)
            Debug.Log($"EnemyHealth: Enemy {gameObject.name} died, ScoreManager.Instance = {(ScoreManager.Instance != null ? "EXISTS" : "NULL")}");
            if (ScoreManager.Instance != null)
            {
                ScoreManager.Instance.AddKillPoints();
            }
            else
            {
                Debug.LogError("EnemyHealth: ScoreManager.Instance is NULL! Cannot add kill penalty.");
            }
            
            if (deathVFXPrefab != null)
            {
                Instantiate(deathVFXPrefab, transform.position, Quaternion.identity);
            }
            
            PickUpSpawner pickupSpawner = GetComponent<PickUpSpawner>();
            if (pickupSpawner != null)
            {
                pickupSpawner.DropItems();
            }
            
            Destroy(gameObject);
        }
    }
}
