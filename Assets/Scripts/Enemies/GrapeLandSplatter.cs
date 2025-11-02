using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GrapeLandSplatter : MonoBehaviour
{
    [SerializeField] private float stunDuration = 0.8f; // Stun duration in seconds
    private SpriteFade spriteFade;
    private bool hasHitPlayer = false; // Prevent multiple hits

    private void Awake() {
        spriteFade = GetComponent<SpriteFade>();
    }

    private void Start() {
        StartCoroutine(spriteFade.SlowFadeRoutine());

        Invoke("DisableCollider", 0.2f);
    }

    private void OnTriggerEnter2D(Collider2D other) {
        // Only hit player once
        if (hasHitPlayer) { return; }
        
        PlayerController playerController = other.gameObject.GetComponent<PlayerController>();
        if (playerController != null)
        {
            hasHitPlayer = true;
            // Apply stun instead of damage
            playerController.ApplyStun(stunDuration);
            
            // Optional: Add visual/audio feedback here
            ScreenShakeManager.Instance?.ShakeScreen();
        }
    }

    private void DisableCollider() {
        GetComponent<CapsuleCollider2D>().enabled = false;
    }
}
