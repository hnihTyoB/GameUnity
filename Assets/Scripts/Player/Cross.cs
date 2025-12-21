using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Cross : MonoBehaviour, IWeapon
{
    [SerializeField] private GameObject slashAnimPrefab;
    [SerializeField] private Transform slashAnimSpawnPoint;
    [SerializeField] private float crossAttackCD = .5f;
    [SerializeField] private WeaponInfo weaponInfo;

    private Transform weaponCollider;

    private Animator myAnimator;

    private GameObject slashAnim;
    private void Awake()
    {
        myAnimator = GetComponent<Animator>();
    }
    private void Start()
    {
        weaponCollider = PlayerController.Instance.GetWeaponCollider();
        slashAnimSpawnPoint = GameObject.Find("SlashSpawnPoint").transform;
    }
    private void Update()
    {
        if (Time.timeScale == 0f) return;
        
        MouseFollowWithOffset();
    }

    public WeaponInfo GetWeaponInfo()
    {
        return weaponInfo;
    }

    public void Attack()
    {
        myAnimator.SetTrigger("Attack");
        weaponCollider.gameObject.SetActive(true);
        slashAnim = Instantiate(slashAnimPrefab, slashAnimSpawnPoint.position, Quaternion.identity);
        slashAnim.transform.parent = this.transform.parent;
        
        PlayHitSoundSafe();
    }

    public void DoneAttackingAnimEvent()
    {
        weaponCollider.gameObject.SetActive(false);
    }
    public void SwingUpFlipAnimEvent()
    {
        slashAnim.gameObject.transform.rotation = Quaternion.Euler(-180, 0, 0);

        if (PlayerController.Instance.FacingLeft)
        {
            slashAnim.GetComponent<SpriteRenderer>().flipX = true;
        }
    }

    public void SwingDownFlipAnimEvent() {
        slashAnim.gameObject.transform.rotation = Quaternion.Euler(0, 0, 0);

        if (PlayerController.Instance.FacingLeft)
        {
            slashAnim.GetComponent<SpriteRenderer>().flipX = true;
        }
    }
    private void MouseFollowWithOffset()
    {
        Vector3 mousePos = Input.mousePosition;
        Vector3 playerScreenPoint = Camera.main.WorldToScreenPoint(PlayerController.Instance.transform.position);

        float angle = Mathf.Atan2(mousePos.y, mousePos.x) * Mathf.Rad2Deg;

        if (mousePos.x < playerScreenPoint.x)
        {
            ActiveWeapon.Instance.transform.rotation = Quaternion.Euler(0, -180, angle);
            weaponCollider.transform.rotation = Quaternion.Euler(0, -180, 0);
        }
        else
        {
            ActiveWeapon.Instance.transform.rotation = Quaternion.Euler(0, 0, angle);
            weaponCollider.transform.rotation = Quaternion.Euler(0, 0, 0);
        }
    }
    
    private void PlayHitSoundSafe()
    {
        SFXManager sfxManager = SFXManager.Instance;
        
        if (sfxManager == null)
        {
            sfxManager = FindObjectOfType<SFXManager>();
        }
        

        if (sfxManager == null)
        {
            GameObject sfxManagerObj = new GameObject("SFX Manager (Auto-Created)");
            sfxManager = sfxManagerObj.AddComponent<SFXManager>();
            Debug.LogWarning("Cross: SFXManager not found! Created automatically. Please assign hit.wav in Inspector for proper setup.");
        }
        
        if (sfxManager != null)
        {
            sfxManager.PlayHitSound();
        }
        else
        {
            Debug.LogError("Cross: Failed to create or find SFXManager! Hit sound will not play.");
        }
    }
}

