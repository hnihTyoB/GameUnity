using System.Collections;
using UnityEngine;

public class Battery : MonoBehaviour
{
    [Header("Battery Settings")]
    [SerializeField] private int batteryAmount = 1; 
    [SerializeField] private float pickupRange = 1.5f; 
    [SerializeField] private float moveSpeed = 2f; 
    
    [Header("Visual Effects")]
    [SerializeField] private GameObject pickupEffect; 
    [SerializeField] private AudioClip pickupSound;
    
    [Header("Animation")]
    [SerializeField] private float bobHeight = 0.3f; 
    [SerializeField] private float bobSpeed = 2f; 
    
    private bool isPickedUp = false;
    private Transform playerTransform;
    private Vector3 startPosition;
    private AudioSource audioSource;
    private SpriteRenderer spriteRenderer;
    
    private void Awake()
    {
        audioSource = GetComponent<AudioSource>();
        spriteRenderer = GetComponent<SpriteRenderer>();
        startPosition = transform.position;
    }
    
    private void Start()
    {

        StartCoroutine(BobbingAnimation());
    }
    
    private void Update()
    {
        if (isPickedUp) return;
        
    
        if (PlayerController.Instance != null)
        {
            float distanceToPlayer = Vector2.Distance(transform.position, PlayerController.Instance.transform.position);
            
            if (distanceToPlayer <= pickupRange)
            {
              
                StartCoroutine(MoveTowardsPlayer());
            }
        }
    }
    
    private IEnumerator BobbingAnimation()
    {
        while (!isPickedUp)
        {
            float newY = startPosition.y + Mathf.Sin(Time.time * bobSpeed) * bobHeight;
            transform.position = new Vector3(transform.position.x, newY, transform.position.z);
            yield return null;
        }
    }
    
    private IEnumerator MoveTowardsPlayer()
    {
        isPickedUp = true;
        playerTransform = PlayerController.Instance.transform;
        
        while (Vector2.Distance(transform.position, playerTransform.position) > 0.1f)
        {
            Vector2 direction = (playerTransform.position - transform.position).normalized;
            transform.position += (Vector3)direction * moveSpeed * Time.deltaTime;
            yield return null;
        }
        
        PickupBattery();
    }
    
    private void PickupBattery()
    {
        Debug.Log($"Battery pickup triggered! Amount: {batteryAmount}");
        
        if (BatteryManager.Instance != null)
        {
            BatteryManager.Instance.AddBattery(batteryAmount);
            Debug.Log($"Battery added to BatteryManager");
        }
        else
        {
            Debug.LogError("BatteryManager.Instance is NULL! Cannot add battery!");
        }
        
        if (SFXManager.Instance != null)
        {
            SFXManager.Instance.PlayBatterySound();
        }
        
        PlayPickupEffects();
        
        Destroy(gameObject);
    }
    
    private void PlayPickupEffects()
    {
     
        if (pickupEffect != null)
        {
            GameObject effect = Instantiate(pickupEffect, transform.position, Quaternion.identity);
            Destroy(effect, 2f); 
        }
    }
    
    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, pickupRange);
    }
}
