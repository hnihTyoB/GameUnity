using UnityEngine;

/// <summary>
/// Aura effect cho vision debuff - Purple isolation aura
/// </summary>
public class IsolationAuraEffect : MonoBehaviour
{
    [Header("Aura Settings")]
    [Tooltip("Dùng SpriteRenderer với sprite hình tròn để có hiệu ứng đẹp")]
    [SerializeField] private SpriteRenderer auraRenderer;
    [SerializeField] private float rotationSpeed = 25f; // Slightly slower than slow aura
    [SerializeField] private float pulseSpeed = 2.5f; // Slightly faster pulse
    [SerializeField] private float minAlpha = 0.4f;
    [SerializeField] private float maxAlpha = 0.8f;
    [SerializeField] private Color auraColor = new Color(0.5f, 0.2f, 0.6f, 0.6f); // Purple
    
    private float pulseTimer = 0f;

    private void Awake()
    {
        if (auraRenderer == null)
        {
            auraRenderer = GetComponent<SpriteRenderer>();
        }
        
        if (auraRenderer != null)
        {
            // Check if sprite is assigned
            if (auraRenderer.sprite == null)
            {
                Debug.LogWarning("IsolationAuraEffect: No sprite assigned! Aura will be invisible.");
                // Disable renderer if no sprite
                auraRenderer.enabled = false;
            }
            else
            {
                auraRenderer.color = auraColor;
                // Ensure proper rendering
                auraRenderer.sortingOrder = 100; // Above player
                auraRenderer.drawMode = SpriteDrawMode.Simple; // Not sliced
            }
        }
        
        // Remove any collider (shouldn't have one)
        Collider2D col = GetComponent<Collider2D>();
        if (col != null)
        {
            Destroy(col);
        }
    }

    private void Update()
    {
        // Always follow player position
        if (PlayerController.Instance != null)
        {
            transform.position = PlayerController.Instance.transform.position;
        }
        
        // Rotate the aura
        transform.Rotate(Vector3.forward, rotationSpeed * Time.deltaTime);
        
        // Pulse the aura alpha
        pulseTimer += Time.deltaTime * pulseSpeed;
        float alpha = Mathf.Lerp(minAlpha, maxAlpha, (Mathf.Sin(pulseTimer) + 1f) / 2f);
        
        if (auraRenderer != null)
        {
            Color currentColor = auraRenderer.color;
            currentColor.a = alpha;
            auraRenderer.color = currentColor;
        }
    }

    private void OnEnable()
    {
        // Reset position when enabled
        pulseTimer = 0f;
        if (PlayerController.Instance != null)
        {
            transform.position = PlayerController.Instance.transform.position;
        }
    }

    private void OnDisable()
    {
        pulseTimer = 0f;
    }
}

