using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
public class PlayerHealth : Singleton<PlayerHealth>
{
    public bool isDead { get; private set; }
    [SerializeField] private int maxHealth = 3;
    [SerializeField] private float knockBackThrustAmount = 10f;
    [SerializeField] private float damageRecoveryTime = 1f;
    private Slider healthSlider;
    private int currentHealth;
    private bool canTakeDamage = true;
    private Knockback knockback;
    private Flash flash;
    private float baseDamageRecoveryTime; // Lưu giá trị gốc
    const string HEALTH_SLIDER_TEXT = "Battery Slider";

    const string TOWN_TEXT = "Scene1";
    readonly int DEATH_HASH = Animator.StringToHash("Death");

    protected override void Awake()
    {
        base.Awake();

        flash = GetComponent<Flash>();
        knockback = GetComponent<Knockback>();
        
        // Lưu giá trị gốc
        baseDamageRecoveryTime = damageRecoveryTime;
    }
    private void Start()
    {
        isDead = false;
        currentHealth = maxHealth;

        UpdateHealthSlider();
    }
    private void OnCollisionStay2D(Collision2D other)
    {
        EnemyAI enemy = other.gameObject.GetComponent<EnemyAI>();

        if (enemy)
        {
            bool isShadow = other.gameObject.GetComponent<ShadowGhost>() != null ||
                           other.gameObject.GetComponent<ShadowGhost2>() != null ||
                           other.gameObject.GetComponent<ShadowGhost3>() != null;
            
            if (isShadow)
            {
                // Shadow: no damage, no knockback - chỉ slow effect
                return;
            }
            
            NonDamagingEnemy nonDamaging = other.gameObject.GetComponent<NonDamagingEnemy>();
            if (nonDamaging == null)
            {
                TakeDamage(1, other.transform);
            }
            else
            {
                // NonDamagingEnemy (not Shadow): no damage, but HAS knockback
                ApplyKnockbackOnly(other.transform);
            }
        }
    }
    
    private void OnTriggerStay2D(Collider2D other)
    {
        EnemyAI enemy = other.gameObject.GetComponent<EnemyAI>();

        if (enemy)
        {
            bool isShadow = other.gameObject.GetComponent<ShadowGhost>() != null ||
                           other.gameObject.GetComponent<ShadowGhost2>() != null ||
                           other.gameObject.GetComponent<ShadowGhost3>() != null;
            
            if (isShadow)
            {
                // Shadow: no damage, no knockback - chỉ slow effect
                return;
            }
            
            NonDamagingEnemy nonDamaging = other.gameObject.GetComponent<NonDamagingEnemy>();
            if (nonDamaging == null)
            {
                TakeDamage(1, other.transform);
            }
            else
            {
                // NonDamagingEnemy (not Shadow): no damage, but HAS knockback
                ApplyKnockbackOnly(other.transform);
            }
        }
    }
    public void HealPlayer()
    {
        if (BatteryManager.Instance != null)
        {
            BatteryManager.Instance.AddBattery(1);
        }
    }
    public void TakeDamage(int damageAmount, Transform hitTransform)
    {
        if (!canTakeDamage) { return; }

        ScreenShakeManager.Instance.ShakeScreen();
        knockback.GetKnockedBack(hitTransform, knockBackThrustAmount);
        
        if (SFXManager.Instance != null)
        {
            SFXManager.Instance.PlayDamageTakenSound();
        }
        
        StartCoroutine(flash.FlashRoutine());
        canTakeDamage = false;
        currentHealth -= damageAmount;
        StartCoroutine(DamageRecoveryRoutine());
        UpdateHealthSlider();
        CheckIfPlayerDeath();
    }
    private void CheckIfPlayerDeath()
    {
        if (currentHealth <= 0 && !isDead)
        {
            isDead = true;
            Destroy(ActiveWeapon.Instance.gameObject);
            currentHealth = 0;
            GetComponent<Animator>().SetTrigger(DEATH_HASH);
            StartCoroutine(DeathLoadSceneRoutine());
        }
    }
    private IEnumerator DeathLoadSceneRoutine()
    {
        float adjustedTime = GetAdjustedRecoveryTime();
        yield return new WaitForSeconds(adjustedTime);
        canTakeDamage = true;
    }

    private IEnumerator DamageRecoveryRoutine()
    {
        float adjustedTime = GetAdjustedRecoveryTime();
        yield return new WaitForSeconds(adjustedTime);
        canTakeDamage = true;
    }
    
    /// <summary>
    /// Lấy recovery time đã điều chỉnh theo difficulty
    /// Easy: Hồi nhanh hơn (0.7x thời gian)
    /// Hard: Hồi chậm hơn (1.5x thời gian)
    /// </summary>
    private float GetAdjustedRecoveryTime()
    {
        float multiplier = DifficultyManager.GetDifficultyMultiplier();
        return baseDamageRecoveryTime * multiplier;
    }

    private void ApplyKnockbackOnly(Transform hitTransform)
    {
        if (!canTakeDamage) { return; }

        knockback.GetKnockedBack(hitTransform, knockBackThrustAmount);
        
        if (SFXManager.Instance != null)
        {
            SFXManager.Instance.PlayDamageTakenSound();
        }
        
        StartCoroutine(flash.FlashRoutine());
        canTakeDamage = false;
        StartCoroutine(DamageRecoveryRoutine());
    }

    private void UpdateHealthSlider() {
        if (healthSlider == null) {
            GameObject sliderObject = GameObject.Find(HEALTH_SLIDER_TEXT);
            if (sliderObject != null) {
                healthSlider = sliderObject.GetComponent<Slider>();
            }
        }

        if (healthSlider != null) {
            healthSlider.maxValue = maxHealth;
            healthSlider.value = currentHealth;
        }
    }
}