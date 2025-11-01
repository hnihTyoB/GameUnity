using UnityEngine;
using TMPro;
using UnityEngine.UI;

/// <summary>
/// Manages cooldown display for weapon slots
/// Shows cooldown timer text and overlay on weapon icon
/// </summary>
public class WeaponCooldownUI : MonoBehaviour
{
    [Header("UI References")]
    [SerializeField] private TextMeshProUGUI cooldownText;
    [SerializeField] private Image cooldownOverlay; // Dark overlay when on cooldown
    [SerializeField] private Image weaponIcon; // The weapon icon image
    
    private float currentCooldown = 0f;
    private float maxCooldown = 0f;
    private bool isOnCooldown = false;
    
    private void Awake()
    {
        // Hide cooldown UI initially
        if (cooldownText != null)
        {
            cooldownText.gameObject.SetActive(false);
        }
        
        if (cooldownOverlay != null)
        {
            cooldownOverlay.gameObject.SetActive(false);
        }
    }
    
    private void Update()
    {
        if (isOnCooldown)
        {
            UpdateCooldownDisplay();
        }
    }
    
    /// <summary>
    /// Start cooldown timer
    /// </summary>
    public void StartCooldown(float cooldownDuration)
    {
        maxCooldown = cooldownDuration;
        currentCooldown = cooldownDuration;
        isOnCooldown = true;
        
        // Show cooldown UI
        if (cooldownText != null)
        {
            cooldownText.gameObject.SetActive(true);
        }
        
        if (cooldownOverlay != null)
        {
            cooldownOverlay.gameObject.SetActive(true);
        }
    }
    
    /// <summary>
    /// Update cooldown display every frame
    /// </summary>
    private void UpdateCooldownDisplay()
    {
        currentCooldown -= Time.deltaTime;
        
        if (currentCooldown <= 0f)
        {
            // Cooldown finished
            EndCooldown();
            return;
        }
        
        // Update text - show remaining time
        if (cooldownText != null)
        {
            cooldownText.text = Mathf.Ceil(currentCooldown).ToString("F0");
        }
        
        // Update overlay opacity based on remaining cooldown
        if (cooldownOverlay != null && maxCooldown > 0f)
        {
            float fillAmount = currentCooldown / maxCooldown;
            Color overlayColor = cooldownOverlay.color;
            overlayColor.a = fillAmount * 0.7f; // Max 70% opacity
            cooldownOverlay.color = overlayColor;
        }
    }
    
    /// <summary>
    /// End cooldown and hide UI
    /// </summary>
    private void EndCooldown()
    {
        isOnCooldown = false;
        currentCooldown = 0f;
        
        // Hide cooldown UI
        if (cooldownText != null)
        {
            cooldownText.gameObject.SetActive(false);
        }
        
        if (cooldownOverlay != null)
        {
            cooldownOverlay.gameObject.SetActive(false);
        }
    }
    
    /// <summary>
    /// Check if weapon is currently on cooldown
    /// </summary>
    public bool IsOnCooldown()
    {
        return isOnCooldown;
    }
    
    /// <summary>
    /// Get remaining cooldown time
    /// </summary>
    public float GetRemainingCooldown()
    {
        return currentCooldown;
    }
}

