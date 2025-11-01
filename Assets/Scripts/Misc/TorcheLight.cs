using UnityEngine;
using UnityEngine.Rendering.Universal;

/// <summary>
/// Manages Torche light source for Shadow Ghost fear behavior
/// </summary>
public class TorcheLight : MonoBehaviour
{
    [Header("Light Settings")]
    [SerializeField] private Light2D lightComponent;
    [SerializeField] private float lightRadius = 2.22f; // Default radius from prefab
    [SerializeField] private bool isActive = true;
    
    [Header("Debug")]
    [SerializeField] private bool showDebugGizmo = false;
    [SerializeField] private Color gizmoColor = Color.yellow;

    private void Awake()
    {
        // Auto-find Light2D if not assigned
        if (lightComponent == null)
        {
            lightComponent = GetComponentInChildren<Light2D>();
        }
        
        // Get radius from Light2D component if available
        if (lightComponent != null)
        {
            lightRadius = lightComponent.pointLightOuterRadius;
        }
    }

    public float GetLightRadius()
    {
        return lightRadius;
    }

    public bool IsLightActive()
    {
        if (lightComponent != null)
        {
            return lightComponent.enabled && isActive;
        }
        return isActive;
    }

    public void SetLightActive(bool active)
    {
        isActive = active;
        if (lightComponent != null)
        {
            lightComponent.enabled = active;
        }
    }

    public void ToggleLight()
    {
        SetLightActive(!isActive);
    }

    private void OnDrawGizmosSelected()
    {
        if (showDebugGizmo)
        {
            Gizmos.color = gizmoColor;
            Gizmos.DrawWireSphere(transform.position, lightRadius);
        }
    }
}
