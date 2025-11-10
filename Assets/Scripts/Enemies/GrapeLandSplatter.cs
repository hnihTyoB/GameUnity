using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GrapeLandSplatter : MonoBehaviour
{
    [SerializeField] private float stunDuration = 0.8f; // Stun duration in seconds
    private SpriteFade spriteFade;
    private bool hasHitTarget = false; // Prevent multiple hits

    private void Awake() {
        spriteFade = GetComponent<SpriteFade>();
    }

    private void Start() {
        StartCoroutine(spriteFade.SlowFadeRoutine());

        Invoke("DisableCollider", 0.2f);
    }

    private void OnTriggerEnter2D(Collider2D other) {
        // Only hit target once
        if (hasHitTarget) { return; }
        
        // Check if hit player
        PlayerController playerController = other.gameObject.GetComponent<PlayerController>();
        if (playerController != null)
        {
            hasHitTarget = true;
            // Apply stun to player
            playerController.ApplyStun(stunDuration);
            
            // Visual/audio feedback
            ScreenShakeManager.Instance?.ShakeScreen();
            return;
        }
        
        // Check if hit victim
        Victim victim = other.gameObject.GetComponent<Victim>();
        if (victim != null && !victim.IsRescued())
        {
            hasHitTarget = true;
            // Apply slow to victim (victims don't have stun, only slow)
            victim.ApplySlow();
            
            // No screen shake for victim hits (only player hits shake screen)
        }
    }

    private void DisableCollider() {
        GetComponent<CapsuleCollider2D>().enabled = false;
    }
}
