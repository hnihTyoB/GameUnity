using System.Collections;
using UnityEngine;
using UnityEngine.Rendering.Universal;

public class Flashlight : MonoBehaviour, IWeapon
{
    [SerializeField] private WeaponInfo weaponInfo;
    
    [Header("Flashlight Settings")]
    [SerializeField] private Light2D flashlightLight; // Light2D component
    [SerializeField] private float lightIntensity = 1.5f;
    [SerializeField] private float lightRadius = 8f;
    [SerializeField] private float falloffIntensity = 0.5f; // Light falloff (0-1)
    
    [Header("Audio (Optional)")]
    [SerializeField] private AudioClip toggleSound;
    
    private bool isLightOn = false;
    private Animator myAnimator;
    private AudioSource audioSource;
    readonly int TOGGLE_HASH = Animator.StringToHash("Toggle");

    private void Awake()
    {
        myAnimator = GetComponent<Animator>();
        audioSource = GetComponent<AudioSource>();
        
        // Setup light if assigned
        if (flashlightLight != null)
        {
            SetupLight();
            flashlightLight.enabled = false; // Start with light OFF
        }
    }

    private void SetupLight()
    {
        // Configure Light2D as point light (Unity 6 API)
        flashlightLight.lightType = Light2D.LightType.Point;
        flashlightLight.intensity = lightIntensity;
        flashlightLight.pointLightOuterRadius = lightRadius;
        flashlightLight.falloffIntensity = falloffIntensity;
        
        // Optional: Set color to warm white/yellow for flashlight feel
        flashlightLight.color = new Color(1f, 0.95f, 0.8f); // Warm white
    }

    private void Update()
    {
        // Make light follow mouse direction (like ActiveWeapon does)
        if (isLightOn && flashlightLight != null)
        {
            RotateLightTowardsMouse();
        }
    }

    public void Attack()
    {
        // Check battery before toggling light
        if (BatteryManager.Instance != null && !BatteryManager.Instance.HasEnoughBattery(1))
        {
            Debug.Log("Not enough battery to use flashlight!");
            return;
        }
        
        // Toggle light ON/OFF when "attack" (left click or equip)
        ToggleLight();
    }

    private void ToggleLight()
    {
        isLightOn = !isLightOn;
        
        // Toggle light state
        if (flashlightLight != null)
        {
            flashlightLight.enabled = isLightOn;
        }
        
        // Start/stop battery drain based on light state
        if (BatteryManager.Instance != null)
        {
            if (isLightOn)
            {
                BatteryManager.Instance.StartBatteryDrain();
            }
            else
            {
                BatteryManager.Instance.StopBatteryDrain();
            }
        }
        
        // Play animation if exists
        if (myAnimator != null)
        {
            myAnimator.SetTrigger(TOGGLE_HASH);
        }
        
        // Play toggle sound
        if (audioSource != null && toggleSound != null)
        {
            audioSource.PlayOneShot(toggleSound);
        }
    }

    private void RotateLightTowardsMouse()
    {
        // Get mouse position in world space
        Vector3 mousePos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        mousePos.z = 0f;
        
        // Calculate direction from flashlight to mouse
        Vector2 direction = (mousePos - flashlightLight.transform.position).normalized;
        
        // Calculate angle and rotate light
        float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;
        flashlightLight.transform.rotation = Quaternion.Euler(0, 0, angle - 90f); // -90 to adjust for sprite orientation
    }

    public WeaponInfo GetWeaponInfo()
    {
        return weaponInfo;
    }

    // Public method to turn light on/off (for external control)
    public void SetLightState(bool state)
    {
        isLightOn = state;
        if (flashlightLight != null)
        {
            flashlightLight.enabled = state;
        }
    }

    public bool IsLightOn()
    {
        return isLightOn;
    }

    private void OnDisable()
    {
        // Turn off light when weapon is unequipped
        if (flashlightLight != null)
        {
            flashlightLight.enabled = false;
        }
        
        // Stop battery drain when unequipped
        if (BatteryManager.Instance != null)
        {
            BatteryManager.Instance.StopBatteryDrain();
        }
        
        isLightOn = false;
    }
}

