using UnityEngine;
using TMPro;
using UnityEngine.UI;


public class WeaponCooldownUI : MonoBehaviour
{
    [Header("UI References")]
    [SerializeField] private TextMeshProUGUI cooldownText;
    [SerializeField] private Image cooldownOverlay; 
    [SerializeField] private Image weaponIcon; 
    
    private float currentCooldown = 0f;
    private float maxCooldown = 0f;
    private bool isOnCooldown = false;
    
    private void Awake()
    {
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
    
    public void StartCooldown(float cooldownDuration)
    {
        Debug.Log($"WeaponCooldownUI ({gameObject.name}): StartCooldown called with duration={cooldownDuration:F1}s");
        
        maxCooldown = cooldownDuration;
        currentCooldown = cooldownDuration;
        isOnCooldown = true;
        
        if (cooldownText != null)
        {
            cooldownText.gameObject.SetActive(true);
            Debug.Log($"WeaponCooldownUI ({gameObject.name}): Cooldown text activated");
        }
        else
        {
            Debug.LogError($"WeaponCooldownUI ({gameObject.name}): cooldownText is NULL!");
        }
        
        if (cooldownOverlay != null)
        {
            cooldownOverlay.gameObject.SetActive(true);
            Debug.Log($"WeaponCooldownUI ({gameObject.name}): Cooldown overlay activated");
        }
        else
        {
            Debug.LogError($"WeaponCooldownUI ({gameObject.name}): cooldownOverlay is NULL!");
        }
    }
    
    private void UpdateCooldownDisplay()
    {
        currentCooldown -= Time.deltaTime;
        
        if (currentCooldown <= 0f)
        {
   
            EndCooldown();
            return;
        }
        
 
        if (cooldownText != null)
        {
            cooldownText.text = Mathf.Ceil(currentCooldown).ToString("F0");
        }
        

        if (cooldownOverlay != null && maxCooldown > 0f)
        {
            float fillAmount = currentCooldown / maxCooldown;
            Color overlayColor = cooldownOverlay.color;
            overlayColor.a = fillAmount * 0.7f;
            cooldownOverlay.color = overlayColor;
        }
    }
    
    private void EndCooldown()
    {
        isOnCooldown = false;
        currentCooldown = 0f;
        
        if (cooldownText != null)
        {
            cooldownText.gameObject.SetActive(false);
        }
        
        if (cooldownOverlay != null)
        {
            cooldownOverlay.gameObject.SetActive(false);
        }
    }
    
  
    public bool IsOnCooldown()
    {
        return isOnCooldown;
    }

    public float GetRemainingCooldown()
    {
        return currentCooldown;
    }
}

