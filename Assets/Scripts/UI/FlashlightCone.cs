using System.Collections;
using UnityEngine;
using UnityEngine.Rendering.Universal;

public class FlashlightCone : MonoBehaviour, IWeapon
{
    [SerializeField] private WeaponInfo weaponInfo;
    
    [Header("Flashlight Settings")]
    [SerializeField] private Light2D flashlightLight;
    [SerializeField] private float lightIntensity = 1.5f;
    [SerializeField] private float coneLength = 8f; 
    [SerializeField] private float coneWidth = 4f; 
    
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
        
       
        if (flashlightLight != null)
        {
            SetupConeLight();
            flashlightLight.enabled = false; 
        }
    }

    private void SetupConeLight()
    {
    
        flashlightLight.lightType = Light2D.LightType.Freeform;
        flashlightLight.intensity = lightIntensity;
        flashlightLight.falloffIntensity = 0.5f;
        
        flashlightLight.color = new Color(1f, 0.95f, 0.8f); 

    }

    private void Update()
    {
        if (Time.timeScale == 0f) return;
        
        if (isLightOn && flashlightLight != null)
        {
            RotateLightTowardsMouse();
        }
    }

    public void Attack()
    {
        if (!isLightOn)
        {
            if (BatteryManager.Instance != null && !BatteryManager.Instance.HasEnoughBattery(0.1f))
            {
                Debug.Log("Not enough battery to use flashlight!");
                return;
            }
        }
        
        ToggleLight();
    }

    private void ToggleLight()
    {
        isLightOn = !isLightOn;
        
        Debug.Log($"Flashlight toggled: {(isLightOn ? "ON" : "OFF")}");
        
        if (flashlightLight != null)
        {
            flashlightLight.enabled = isLightOn;
        }
        
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
        
        if (myAnimator != null)
        {
            myAnimator.SetTrigger(TOGGLE_HASH);
        }
        
        if (SFXManager.Instance != null)
        {
            if (isLightOn)
            {
                SFXManager.Instance.PlayFlashlightSound();
            }
            else
            {
                SFXManager.Instance.StopFlashlightSound();
            }
        }
        
        if (audioSource != null && toggleSound != null)
        {
            audioSource.PlayOneShot(toggleSound);
        }
    }

    private void RotateLightTowardsMouse()
    {
        Vector3 mousePos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        mousePos.z = 0f;
        
        Vector2 direction = (mousePos - flashlightLight.transform.position).normalized;
        
        float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;
        flashlightLight.transform.rotation = Quaternion.Euler(0, 0, angle);
    }

    public WeaponInfo GetWeaponInfo()
    {
        return weaponInfo;
    }

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

    public float GetConeLength()
    {
        return coneLength;
    }

    public float GetConeWidth()
    {
        return coneWidth;
    }

    private void OnDisable()
    {
        if (flashlightLight != null)
        {
            flashlightLight.enabled = false;
        }
        
        if (BatteryManager.Instance != null)
        {
            BatteryManager.Instance.StopBatteryDrain();
        }
        
        if (SFXManager.Instance != null)
        {
            SFXManager.Instance.StopFlashlightSound();
        }
        
        isLightOn = false;
    }

    public void UpdateConeSize(float length, float width)
    {
        coneLength = length;
        coneWidth = width;
    }
}

