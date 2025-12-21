using UnityEngine;
using UnityEngine.Rendering.Universal;


public class TorcheLight : MonoBehaviour
{
    [Header("Light Settings")]
    [SerializeField] private Light2D lightComponent;
    [SerializeField] private float lightRadius = 2.22f;
    [SerializeField] private bool isActive = true;
    
    [Header("Debug")]
    [SerializeField] private bool showDebugGizmo = false;
    [SerializeField] private Color gizmoColor = Color.yellow;

    private void Awake()
    {
    
        if (lightComponent == null)
        {
            lightComponent = GetComponentInChildren<Light2D>();
        }
        
     
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
