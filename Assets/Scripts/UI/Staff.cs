using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Staff : MonoBehaviour, IWeapon
{
    public void Attack()
    {
        // myAnimator.SetTrigger(FIRE_HASH);
        // GameObject newArrow = Instantiate(arrowPrefab, arrowSpawnPoint.position, ActiveWeapon.Instance.transform.rotation);
        // newArrow.GetComponent<Projectile>().UpdateProjectileRange(weaponInfo.weaponRange);
        Debug.Log("Staff Attack");
        ActiveWeapon.Instance.ToggleIsAttacking(false);
    }
}
