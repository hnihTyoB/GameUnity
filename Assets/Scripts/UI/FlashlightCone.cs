using System.Collections;
using UnityEngine;
using UnityEngine.Rendering.Universal;

/// <summary>
/// Flashlight with CONE-SHAPED light (uses Freeform Light2D)
/// For Unity 6 / URP 17+
/// </summary>
public class FlashlightCone : MonoBehaviour, IWeapon
{
    [SerializeField] private WeaponInfo weaponInfo;
    
    [Header("Flashlight Settings")]
    [SerializeField] private Light2D flashlightLight; // Light2D component (Freeform)
    [SerializeField] private float lightIntensity = 1.5f;
    [SerializeField] private float coneLength = 8f; // Length of cone
    [SerializeField] private float coneWidth = 4f; // Width at end of cone
    
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
            SetupConeLight();
            flashlightLight.enabled = false; // Start with light OFF
        }
    }

    private void SetupConeLight()
    {
        // Configure Light2D as Freeform to create cone shape
        flashlightLight.lightType = Light2D.LightType.Freeform;
        flashlightLight.intensity = lightIntensity;
        flashlightLight.falloffIntensity = 0.5f;
        
        // Set color to warm white/yellow for flashlight feel
        flashlightLight.color = new Color(1f, 0.95f, 0.8f); // Warm white
        
        // Note: shapePath is read-only in Unity 6, so we'll use a different approach
        // The cone shape will be created manually in the Unity Editor
    }

    private void Update()
    {
        // Make light follow mouse direction
        if (isLightOn && flashlightLight != null)
        {
            RotateLightTowardsMouse();
        }
    }

    public void Attack()
    {
        // If trying to turn ON, check battery
        if (!isLightOn)
        {
            if (BatteryManager.Instance != null && !BatteryManager.Instance.HasEnoughBattery(0.1f))
            {
                Debug.Log("Not enough battery to use flashlight!");
                return;
            }
        }
        
        // Toggle light ON/OFF when "attack"
        ToggleLight();
    }

    private void ToggleLight()
    {
        isLightOn = !isLightOn;
        
        Debug.Log($"Flashlight toggled: {(isLightOn ? "ON" : "OFF")}");
        
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
        flashlightLight.transform.rotation = Quaternion.Euler(0, 0, angle);
    }

    public WeaponInfo GetWeaponInfo()
    {
        return weaponInfo;
    }

    // Public method to turn light on/off
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

    // Update cone size dynamically (optional)
    public void UpdateConeSize(float length, float width)
    {
        coneLength = length;
        coneWidth = width;
        // Note: In Unity 6, shapePath is read-only, so manual setup is required
        Debug.Log($"Cone size updated: Length={length}, Width={width}");
    }
}

