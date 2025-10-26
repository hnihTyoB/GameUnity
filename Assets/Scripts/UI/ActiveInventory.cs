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
        playerControls.Inventory.Keyboard.performed += ctx => ToggleActiveSlot((int)ctx.ReadValue<float>());
    }

    private void OnEnable()
    {
        playerControls.Enable();
    }
    
    public void EquipStartingWeapon() {
        ToggleActiveHighlight(0);
    }

    private void ToggleActiveSlot(int numValue)
    {
        // Only allow slot 1 (Flashlight) and slot 2 (Shield)
        // Block all other weapon slots
        if (numValue == 1 || numValue == 2)
        {
            ToggleActiveHighlight(numValue - 1);
        }
        else
        {
            Debug.Log($"ActiveInventory: Weapon slot {numValue} is disabled. Only Flashlight (1) and Shield (2) are available.");
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
        if (ActiveWeapon.Instance.CurrentActiveWeapon != null)
        {
            Destroy(ActiveWeapon.Instance.CurrentActiveWeapon.gameObject);
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

        GameObject newWeapon = Instantiate(weaponToSpawn, ActiveWeapon.Instance.transform);

        // ActiveWeapon.Instance.transform.rotation = Quaternion.Euler(0, 0, 0);
        // newWeapon.transform.parent = ActiveWeapon.Instance.transform;

        ActiveWeapon.Instance.NewWeapon(newWeapon.GetComponent<MonoBehaviour>());
    }
}
