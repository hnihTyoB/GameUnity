using UnityEngine;


public class SlowAuraEffect : MonoBehaviour
{
    [Header("Aura Settings")]
    [Tooltip("Dùng SpriteRenderer với sprite hình tròn để có hiệu ứng đẹp")]
    [SerializeField] private SpriteRenderer auraRenderer;
    [SerializeField] private float rotationSpeed = 30f;
    [SerializeField] private float pulseSpeed = 2f;
    [SerializeField] private float minAlpha = 0.3f;
    [SerializeField] private float maxAlpha = 0.7f;
    [SerializeField] private Color auraColor = new Color(0.3f, 0.5f, 0.8f, 0.5f); 
    
    private float pulseTimer = 0f;

    private void Awake()
    {
        if (auraRenderer == null)
        {
            auraRenderer = GetComponent<SpriteRenderer>();
        }
        
        if (auraRenderer != null)
        {
     
            if (auraRenderer.sprite == null)
            {
              
                auraRenderer.enabled = false;
            }
            else
            {
                auraRenderer.color = auraColor;
                auraRenderer.sortingOrder = 100; 
                auraRenderer.drawMode = SpriteDrawMode.Simple; 
            }
        }
        
        Collider2D col = GetComponent<Collider2D>();
        if (col != null)
        {
            Destroy(col);
        }
    }

    private void Update()
    {
    
        if (PlayerController.Instance != null)
        {
            transform.position = PlayerController.Instance.transform.position;
        }
 
        transform.Rotate(Vector3.forward, rotationSpeed * Time.deltaTime);
 
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

