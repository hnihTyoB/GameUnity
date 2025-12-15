using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ActiveInventory : Singleton<ActiveInventory>
{
    private int activeSlotIndexNum = 0;
    private PlayerControls playerControls;

    protected override void Awake()
    {
        base.Awake();

        playerControls = new PlayerControls();
    }

private void Start()
{
    playerControls.Inventory.Slot1.performed += _ => ToggleActiveSlot(1);
    playerControls.Inventory.Slot2.performed += _ => ToggleActiveSlot(2);
    playerControls.Inventory.Slot3.performed += _ => ToggleActiveSlot(3);
}

    private void OnEnable()
    {
        if (playerControls != null)
        {
            playerControls.Enable();
        }
    }
    
    private void OnDisable()
    {
        // Check null to prevent NullReferenceException when Singleton destroys old instance
        if (playerControls != null)
        {
            playerControls.Disable();
        }
    }
    
    /// <summary>
    /// Disable inventory input (called when game is paused, e.g., EndLevelUI)
    /// </summary>
    public void DisableInput()
    {
        if (playerControls != null)
        {
            playerControls.Disable();
        }
    }
    
    /// <summary>
    /// Enable inventory input (called when game is resumed)
    /// </summary>
    public void EnableInput()
    {
        if (playerControls != null)
        {
            playerControls.Enable();
        }
    }
    
    public void EquipStartingWeapon() {
        ToggleActiveHighlight(0);
    }

    private void ToggleActiveSlot(int numValue)
    {
        // Only allow slot 1 (Flashlight) and slot 2 (Shield)
        // Block all other weapon slots
        if (numValue == 1 || numValue == 2 || numValue == 3)
        {
            // Check if pressing the same slot again (to activate skill like Shield)
            if (activeSlotIndexNum == numValue - 1)
            {
                // Player pressed the same slot - try to activate weapon skill
                TryActivateWeaponSkill();
            }
            else
            {
                // Switch to different slot
                ToggleActiveHighlight(numValue - 1);
            }
        }
        else
        {
            Debug.Log($"ActiveInventory: Weapon slot {numValue} is disabled. Only Flashlight (1) and Shield (2) are available.");
        }
    }
    
    /// <summary>
    /// Try to activate active weapon's skill (e.g., Shield activation)
    /// </summary>
    private void TryActivateWeaponSkill()
    {
        if (ActiveWeapon.Instance.CurrentActiveWeapon != null)
        {
            // Try to cast to IWeapon and call Attack
            IWeapon weapon = ActiveWeapon.Instance.CurrentActiveWeapon as IWeapon;
            if (weapon != null)
            {
                weapon.Attack();
                Debug.Log($"ActiveInventory: Activated weapon skill for slot {activeSlotIndexNum}");
            }
        }
    }
    private void ToggleActiveHighlight(int indexNum)
    {
        activeSlotIndexNum = indexNum;

        foreach (Transform inventorySlot in this.transform)
        {
            inventorySlot.GetChild(0).gameObject.SetActive(false);
        }

        this.transform.GetChild(indexNum).GetChild(0).gameObject.SetActive(true);
        
        // Check if slot has weapon before changing
        Transform childTransform = transform.GetChild(indexNum);
        InventorySlot slotComponent = childTransform.GetComponentInChildren<InventorySlot>();
        
        if (slotComponent != null && slotComponent.GetWeaponInfo() != null)
        {
            ChangeActiveWeapon();
        }
        else
        {
            Debug.LogWarning($"ActiveInventory: Slot {indexNum} has no weapon assigned. Cannot equip.");
        }
    }

    private void ChangeActiveWeapon()
    {
        // Debug.Log(transform.GetChild(activeSlotIndexNum).GetComponent<InventorySlot>().GetWeaponInfo().weaponPrefab.name);
        
        // Handle current weapon before switching
        if (ActiveWeapon.Instance.CurrentActiveWeapon != null)
        {
            // Check if current weapon is Shield - don't destroy it, just hide it
            Shield currentShield = ActiveWeapon.Instance.CurrentActiveWeapon.GetComponent<Shield>();
            if (currentShield != null)
            {
                // Shield persists - just hide sprite (cooldown continues in background)
                currentShield.OnWeaponUnequipped();
                Debug.Log("ActiveInventory: Shield unequipped (hidden, cooldown continues)");
            }
            else
            {
                // Other weapons can be destroyed normally
                Destroy(ActiveWeapon.Instance.CurrentActiveWeapon.gameObject);
            }
        }

        Transform childTransform = transform.GetChild(activeSlotIndexNum);
        InventorySlot inventorySlot = childTransform.GetComponentInChildren<InventorySlot>();
        
        if (inventorySlot == null)
        {
            Debug.LogError($"ActiveInventory: No InventorySlot found in child {activeSlotIndexNum}");
            ActiveWeapon.Instance.WeaponNull();
            return;
        }
        
        WeaponInfo weaponInfo = inventorySlot.GetWeaponInfo();
        
        if (weaponInfo == null) {
            Debug.LogError($"ActiveInventory: WeaponInfo is null in slot {activeSlotIndexNum}");
            ActiveWeapon.Instance.WeaponNull();
            return;
        }
        
        if (weaponInfo.weaponPrefab == null)
        {
            Debug.LogError($"ActiveInventory: weaponPrefab is null for weapon in slot {activeSlotIndexNum}");
            ActiveWeapon.Instance.WeaponNull();
            return;
        }
        
        GameObject weaponToSpawn = weaponInfo.weaponPrefab;
        
        // Check if we're equipping Shield and it already exists
        Shield existingShield = FindObjectOfType<Shield>();
        if (weaponToSpawn.GetComponent<Shield>() != null && existingShield != null)
        {
            // Shield already exists - just show it again
            existingShield.OnWeaponEquipped();
            ActiveWeapon.Instance.NewWeapon(existingShield.GetComponent<MonoBehaviour>());
            Debug.Log("ActiveInventory: Shield re-equipped (shown again)");
            return;
        }

        // Spawn new weapon normally
        GameObject newWeapon = Instantiate(weaponToSpawn, ActiveWeapon.Instance.transform);

        // ActiveWeapon.Instance.transform.rotation = Quaternion.Euler(0, 0, 0);
        // newWeapon.transform.parent = ActiveWeapon.Instance.transform;

        ActiveWeapon.Instance.NewWeapon(newWeapon.GetComponent<MonoBehaviour>());
    }
}
