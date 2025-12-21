using System.Collections;
using UnityEngine;


public class Victim : MonoBehaviour
{
    [Header("Roaming Settings")]
    [SerializeField] private float roamChangeInterval = 2f;
    [SerializeField] private float moveSpeed = 1.5f; 
    
    [Header("Rescue Settings")]
    [SerializeField] private float rescueRange = 2f; 
    [SerializeField] private KeyCode rescueKey = KeyCode.E;
    [SerializeField] private GameObject rescuePrompt; 
    
    [Header("Visual Feedback")]
    [SerializeField] private GameObject victimIndicator; 
    
    private Rigidbody2D rb;
    private SpriteRenderer spriteRenderer;
    private Animator animator;
    private Vector2 roamDirection;
    private bool isRescued = false;
    private bool isRescuing = false;
    private bool hasReachedSafeZone = false; 
    private VictimFollowPlayer followScript;
    private VictimSlowDebuff slowDebuff;
    private float roamTimer;
    private bool wasInRange = false; 
    private float currentMoveSpeed; 
    private float baseMoveSpeed; 
    
    private void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        spriteRenderer = GetComponent<SpriteRenderer>();
        animator = GetComponent<Animator>();
        followScript = GetComponent<VictimFollowPlayer>();
        slowDebuff = GetComponent<VictimSlowDebuff>();
        
        baseMoveSpeed = moveSpeed;
        currentMoveSpeed = moveSpeed;
        
        if (gameObject.name.Contains("Player"))
        {
            gameObject.name = gameObject.name.Replace("Player", "Victim");
        }
        
        PlayerController playerController = GetComponent<PlayerController>();
        if (playerController != null)
        {
            Destroy(playerController); 
        }
        
        ActiveWeapon activeWeapon = GetComponent<ActiveWeapon>();
        if (activeWeapon != null)
        {
            Destroy(activeWeapon);
        }
        
        ActiveInventory activeInventory = GetComponent<ActiveInventory>();
        if (activeInventory != null)
        {
            Destroy(activeInventory);
        }
        
        if (followScript != null)
        {
            followScript.enabled = false; 
        }
    }

    private void Start()
    {
        roamTimer = roamChangeInterval;
        GetRoamingDirection();
        
        if (rescuePrompt != null)
        {
            rescuePrompt.SetActive(false);
        }
        
        if (victimIndicator != null)
        {
            victimIndicator.SetActive(true);
        }
    }

    private void Update()
    {
        if (isRescued) return; 
        
        CheckRescueInteraction();
        
        roamTimer -= Time.deltaTime;
        if (roamTimer <= 0)
        {
            GetRoamingDirection();
            roamTimer = roamChangeInterval;
        }
    }

    private void FixedUpdate()
    {
        if (isRescued) return;
        
        rb.MovePosition(rb.position + roamDirection * (currentMoveSpeed * Time.fixedDeltaTime));
        
        if (roamDirection.x < 0)
        {
            spriteRenderer.flipX = true;
        }
        else if (roamDirection.x > 0)
        {
            spriteRenderer.flipX = false;
        }
        
        if (animator != null)
        {
            animator.SetFloat("moveX", roamDirection.x);
            animator.SetFloat("moveY", roamDirection.y);
        }
    }

    private void GetRoamingDirection()
    {
        roamDirection = new Vector2(Random.Range(-1f, 1f), Random.Range(-1f, 1f)).normalized;
    }

    private void CheckRescueInteraction()
    {
        if (PlayerController.Instance == null) return;
        if (isRescued) return; 
        
        float distanceToPlayer = Vector2.Distance(transform.position, PlayerController.Instance.transform.position);
        bool inRange = distanceToPlayer <= rescueRange;
        
        if (inRange)
        {
            
            if (!wasInRange)
            {
                wasInRange = true;
            }
        
            if (rescuePrompt != null && !rescuePrompt.activeSelf)
            {
                rescuePrompt.SetActive(true);
            }
            
            if (Input.GetKeyDown(rescueKey) && !isRescued && !isRescuing)
            {
                isRescuing = true;
                RescueVictim();
                return;
            }
        }
        else
        {
            if (wasInRange)
            {
                wasInRange = false;
            }
            
            if (rescuePrompt != null && rescuePrompt.activeSelf)
            {
                rescuePrompt.SetActive(false);
            }
        }
    }

    private void RescueVictim()
    {
        if (isRescued)
        {
            return; 
        }
        
        isRescued = true;
        
        if (rescuePrompt != null)
        {
            rescuePrompt.SetActive(false);
        }
        
        if (victimIndicator != null)
        {
            victimIndicator.SetActive(false);
        }
        
        roamDirection = Vector2.zero;
        
        if (followScript != null)
        {
            followScript.enabled = true;
        }
        
        if (RescueManager.Instance != null)
        {
            RescueManager.Instance.OnVictimRescued();
        }
    }

    public bool IsRescued()
    {
        return isRescued;
    }

    public bool HasReachedSafeZone()
    {
        return hasReachedSafeZone;
    }
    
    public void OnReachedSafeZone()
    {
        if (hasReachedSafeZone) return; 
        
        hasReachedSafeZone = true;
        Debug.Log($"Victim {gameObject.name} reached safe zone!");
        
        if (followScript != null)
        {
            followScript.enabled = false;
        }
        
        if (rb != null)
        {
            rb.linearVelocity = Vector2.zero;
        }
        
        Collider2D collider = GetComponent<Collider2D>();
        if (collider != null)
        {
            collider.enabled = false;
        }
        
        if (spriteRenderer != null)
        {
            spriteRenderer.color = new Color(spriteRenderer.color.r, spriteRenderer.color.g, spriteRenderer.color.b, 0.5f);
        }
        
        Destroy(gameObject, 1f); 
    }

    public void ApplySlow()
    {
        if (slowDebuff != null && !isRescued)
        {
            slowDebuff.ApplySlow(baseMoveSpeed);
        }
    }
    
    public void SetMoveSpeed(float speed)
    {
        currentMoveSpeed = speed;
    }
    
    public float GetBaseMoveSpeed()
    {
        return baseMoveSpeed;
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.green;
        Gizmos.DrawWireSphere(transform.position, rescueRange);
    }
}

