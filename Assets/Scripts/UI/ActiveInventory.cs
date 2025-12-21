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

        if (playerControls != null)
        {
            playerControls.Disable();
        }
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
    
    public void EquipStartingWeapon() {
        ToggleActiveHighlight(0);
    }

    private void ToggleActiveSlot(int numValue)
    {

        if (numValue == 1 || numValue == 2 || numValue == 3)
        {
            if (activeSlotIndexNum == numValue - 1)
            {
                TryActivateWeaponSkill();
            }
            else
            {
                ToggleActiveHighlight(numValue - 1);
            }
        }
        else
        {
            Debug.Log($"ActiveInventory: Weapon slot {numValue} is disabled. Only Flashlight (1) and Shield (2) are available.");
        }
    }
    
    private void TryActivateWeaponSkill()
    {
        if (ActiveWeapon.Instance.CurrentActiveWeapon != null)
        {
            IWeapon weapon = ActiveWeapon.Instance.CurrentActiveWeapon as IWeapon;
            if (weapon != null)
            {
                weapon.Attack();
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
        if (ActiveWeapon.Instance.CurrentActiveWeapon != null)
        {
            Shield currentShield = ActiveWeapon.Instance.CurrentActiveWeapon.GetComponent<Shield>();
            if (currentShield != null)
            {
                currentShield.OnWeaponUnequipped();
            }
            else
            {
                Destroy(ActiveWeapon.Instance.CurrentActiveWeapon.gameObject);
            }
        }

        Transform childTransform = transform.GetChild(activeSlotIndexNum);
        InventorySlot inventorySlot = childTransform.GetComponentInChildren<InventorySlot>();
        
        if (inventorySlot == null)
        {
            ActiveWeapon.Instance.WeaponNull();
            return;
        }
        
        WeaponInfo weaponInfo = inventorySlot.GetWeaponInfo();
        
        if (weaponInfo == null) {
            ActiveWeapon.Instance.WeaponNull();
            return;
        }
        
        if (weaponInfo.weaponPrefab == null)
        {
            ActiveWeapon.Instance.WeaponNull();
            return;
        }
        
        GameObject weaponToSpawn = weaponInfo.weaponPrefab;
        Shield existingShield = FindObjectOfType<Shield>();
        if (weaponToSpawn.GetComponent<Shield>() != null && existingShield != null)
        {
            existingShield.OnWeaponEquipped();
            ActiveWeapon.Instance.NewWeapon(existingShield.GetComponent<MonoBehaviour>());
            return;
        }
        GameObject newWeapon = Instantiate(weaponToSpawn, ActiveWeapon.Instance.transform);
        ActiveWeapon.Instance.NewWeapon(newWeapon.GetComponent<MonoBehaviour>());
    }
}
