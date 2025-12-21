using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyHealth : MonoBehaviour
{
    [SerializeField] private int startingHealth = 3;
    [SerializeField] private GameObject deathVFXPrefab;
    [SerializeField] private float knockBackThrust = 15f;

    private int currentHealth;
    private int baseStartingHealth; 
    private Knockback knockback;
    private Flash flash;
    
    private void Awake()
    {
        flash = GetComponent<Flash>();
        knockback = GetComponent<Knockback>();
        
    
        baseStartingHealth = startingHealth;
    }

    private void Start()
    {

        ApplyDifficultyMultiplier();
    }
    

    private void ApplyDifficultyMultiplier()
    {
        float multiplier = DifficultyManager.GetDifficultyMultiplier();
        int adjustedHealth = Mathf.RoundToInt(baseStartingHealth * multiplier);
        currentHealth = adjustedHealth;
        
    
    }
    
 
    public void UpdateDifficultyMultiplier()
    {
        
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
