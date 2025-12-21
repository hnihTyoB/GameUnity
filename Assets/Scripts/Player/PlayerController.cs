using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerController : Singleton<PlayerController>
{
    public bool FacingLeft { get { return facingLeft; } }

    [SerializeField] private float moveSpeed = 1f;
    [SerializeField] private float dashSpeed = 4f;
    [SerializeField] private TrailRenderer myTrailRenderer;
    [SerializeField] private Transform weaponCollider;

    private PlayerControls playerControls;
    private Vector2 movement;
    private Rigidbody2D rb;
    private Animator myAnimator;
    private SpriteRenderer mySpriteRender;
    private Knockback knockback;
    private float startingMoveSpeed;
    private bool facingLeft = false;
    private bool isDashing = false;
    
    private bool isSlowedByDebuff = false;
    private float slowMultiplier = 1f;
    
    private bool isStunned = false;
    private Coroutine stunCoroutine;
    [SerializeField] private Color stunColor = new Color(0.8f, 0f, 1f, 1f); 
    private Color originalColor;

    protected override void Awake()
    {
        base.Awake();

        playerControls = new PlayerControls();
        rb = GetComponent<Rigidbody2D>();
        myAnimator = GetComponent<Animator>();
        mySpriteRender = GetComponent<SpriteRenderer>();
        knockback = GetComponent<Knockback>();
        
        originalColor = mySpriteRender.color;
    }
    private void Start()
    {
        playerControls.Combat.Dash.performed += _ => Dash();

        startingMoveSpeed = moveSpeed;
        ActiveInventory.Instance.EquipStartingWeapon();
    }
    private void OnEnable()
    {
        playerControls.Enable();
    }
    private void OnDisable() {
        playerControls.Disable();
    }
    
    public void DisableInput()
    {
        if (playerControls != null)
        {
            playerControls.Disable();
        }
    }
    
    public void EnableInput()
    {
        if (playerControls != null)
        {
            playerControls.Enable();
        }
    }

    private void Update()
    {
        PlayerInput();
    }

    private void FixedUpdate()
    {
        AdjustPlayerFacingDirection();
        Move();
    }
    public Transform GetWeaponCollider()
    {
        return weaponCollider;
    }

    private void PlayerInput()
    {
        movement = playerControls.Movement.Move.ReadValue<Vector2>();

        myAnimator.SetFloat("moveX", movement.x);
        myAnimator.SetFloat("moveY", movement.y);
    }

    private void Move()
    {
        if (knockback.GettingKnockedBack || PlayerHealth.Instance.isDead || isStunned) { return; }
        float currentSpeed = moveSpeed * slowMultiplier;
        rb.MovePosition(rb.position + movement * (currentSpeed * Time.fixedDeltaTime));
    }

    private void AdjustPlayerFacingDirection()
    {
        Vector3 mousePos = Input.mousePosition;
        Vector3 playerScreenPoint = Camera.main.WorldToScreenPoint(transform.position);

        if (mousePos.x < playerScreenPoint.x)
        {
            mySpriteRender.flipX = true;
            facingLeft = true;
        }
        else
        {
            mySpriteRender.flipX = false;
            facingLeft = false;
        }
    }
    private void Dash()
    {
        if (!isDashing && Stamina.Instance.CurrentStamina > 0) {
            Stamina.Instance.UseStamina();
            isDashing = true;
            moveSpeed *= dashSpeed;
            myTrailRenderer.emitting = true;
            
            if (SFXManager.Instance != null)
            {
                SFXManager.Instance.PlayDashSound();
            }
            
            StartCoroutine(EndDashRoutine());
        }
    }
    private IEnumerator EndDashRoutine()
    {
        float dashTime = .2f;
        float dashCD = .25f;
        yield return new WaitForSeconds(dashTime);
        moveSpeed = startingMoveSpeed;
        myTrailRenderer.emitting = false;
        yield return new WaitForSeconds(dashCD);
        isDashing = false;
    }

    public void ApplySlowEffect(float slowPercentage)
    {
        isSlowedByDebuff = true;
        slowMultiplier = 1f - slowPercentage; 
    }

    public void RemoveSlowEffect()
    {
        isSlowedByDebuff = false;
        slowMultiplier = 1f;
    }

    public bool IsSlowed()
    {
        return isSlowedByDebuff;
    }

    public void ApplyStun(float stunDuration)
    {
        if (isStunned) { return; }
        
        if (stunCoroutine != null)
        {
            StopCoroutine(stunCoroutine);
        }
        
        stunCoroutine = StartCoroutine(StunRoutine(stunDuration));
    }

    private IEnumerator StunRoutine(float duration)
    {
        isStunned = true;
        
        float elapsed = 0f;
        float blinkInterval = 0.12f; 
        bool isStunColorActive = true;
        
        while (elapsed < duration)
        {
            mySpriteRender.color = isStunColorActive ? stunColor : Color.white;
            isStunColorActive = !isStunColorActive;
            
            yield return new WaitForSeconds(blinkInterval);
            elapsed += blinkInterval;
        }
        
        mySpriteRender.color = originalColor;
        
        isStunned = false;
        stunCoroutine = null;
    }

    public bool IsStunned()
    {
        return isStunned;
    }
}