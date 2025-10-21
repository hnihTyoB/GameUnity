using System.Collections;
using UnityEngine;

/// <summary>
/// Battery pickup item - adds battery power to player
/// </summary>
public class Battery : MonoBehaviour
{
    [Header("Battery Settings")]
    [SerializeField] private int batteryAmount = 1; // How much battery this pickup gives
    [SerializeField] private float pickupRange = 1.5f; // Range to detect player
    [SerializeField] private float moveSpeed = 2f; // Speed when moving towards player
    
    [Header("Visual Effects")]
    [SerializeField] private GameObject pickupEffect; // Particle effect when picked up
    [SerializeField] private AudioClip pickupSound; // Sound when picked up
    
    [Header("Animation")]
    [SerializeField] private float bobHeight = 0.3f; // How high the battery bobs
    [SerializeField] private float bobSpeed = 2f; // Speed of bobbing animation
    
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
        // Start bobbing animation
        StartCoroutine(BobbingAnimation());
    }
    
    private void Update()
    {
        if (isPickedUp) return;
        
        // Check if player is in range
        if (PlayerController.Instance != null)
        {
            float distanceToPlayer = Vector2.Distance(transform.position, PlayerController.Instance.transform.position);
            
            if (distanceToPlayer <= pickupRange)
            {
                // Start moving towards player
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
        
        // Move towards player
        while (Vector2.Distance(transform.position, playerTransform.position) > 0.1f)
        {
            Vector2 direction = (playerTransform.position - transform.position).normalized;
            transform.position += (Vector3)direction * moveSpeed * Time.deltaTime;
            yield return null;
        }
        
        // Pick up the battery
        PickupBattery();
    }
    
    private void PickupBattery()
    {
        // Add battery to player
        if (BatteryManager.Instance != null)
        {
            BatteryManager.Instance.AddBattery(batteryAmount);
        }
        
        // Play pickup effects
        PlayPickupEffects();
        
        // Destroy the battery
        Destroy(gameObject);
    }
    
    private void PlayPickupEffects()
    {
        // Play sound
        if (audioSource != null && pickupSound != null)
        {
            audioSource.PlayOneShot(pickupSound);
        }
        
        // Spawn pickup effect
        if (pickupEffect != null)
        {
            GameObject effect = Instantiate(pickupEffect, transform.position, Quaternion.identity);
            Destroy(effect, 2f); // Clean up after 2 seconds
        }
    }
    
    private void OnDrawGizmosSelected()
    {
        // Draw pickup range in editor
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, pickupRange);
    }
}
