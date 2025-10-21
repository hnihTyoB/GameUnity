using UnityEngine;

/// <summary>
/// Aura effect cho slow debuff
/// LƯU Ý: Để có aura hình tròn đẹp, hãy dùng sprite hình tròn!
/// Trong Unity: Create > 2D > Sprites > Circle hoặc dùng sprite có sẵn
/// </summary>
public class SlowAuraEffect : MonoBehaviour
{
    [Header("Aura Settings")]
    [Tooltip("Dùng SpriteRenderer với sprite hình tròn để có hiệu ứng đẹp")]
    [SerializeField] private SpriteRenderer auraRenderer;
    [SerializeField] private float rotationSpeed = 30f;
    [SerializeField] private float pulseSpeed = 2f;
    [SerializeField] private float minAlpha = 0.3f;
    [SerializeField] private float maxAlpha = 0.7f;
    [SerializeField] private Color auraColor = new Color(0.3f, 0.5f, 0.8f, 0.5f); // Light blue
    
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
                Debug.LogWarning("SlowAuraEffect: No sprite assigned! Aura will be invisible.");
                // Disable renderer if no sprite (tránh hiển thị ô vuông mặc định)
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
        
        // Hide any collider (shouldn't have one, but just in case)
        Collider2D col = GetComponent<Collider2D>();
        if (col != null)
        {
            Destroy(col); // Remove collider if exists
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

