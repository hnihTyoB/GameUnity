using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GrapeLandSplatter : MonoBehaviour
{
    [SerializeField] private float stunDuration = 0.8f; 
    private SpriteFade spriteFade;
    private bool hasHitTarget = false;

    private void Awake() {
        spriteFade = GetComponent<SpriteFade>();
    }

    private void Start() {
        StartCoroutine(spriteFade.SlowFadeRoutine());

        Invoke("DisableCollider", 0.2f);
    }

    private void OnTriggerEnter2D(Collider2D other) {
     
        if (hasHitTarget) { return; }
        
  
        PlayerController playerController = other.gameObject.GetComponent<PlayerController>();
        if (playerController != null)
        {
            hasHitTarget = true;
        
            playerController.ApplyStun(stunDuration);
            
          
            if (SFXManager.Instance != null)
            {
                SFXManager.Instance.PlayDamageTakenSound();
            }
            
   
            ScreenShakeManager.Instance?.ShakeScreen();
            return;
        }
        
        
        Victim victim = other.gameObject.GetComponent<Victim>();
        if (victim != null && !victim.IsRescued())
        {
            hasHitTarget = true;
           
            victim.ApplySlow();
            
           
        }
    }

    private void DisableCollider() {
        GetComponent<CapsuleCollider2D>().enabled = false;
    }
}
