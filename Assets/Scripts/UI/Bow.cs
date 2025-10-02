using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Bow : MonoBehaviour, IWeapon
{
    [SerializeField] private WeaponInfo weaponInfo;
    public void Attack()
    {
        // myAnimator.SetTrigger(FIRE_HASH);
        // GameObject newArrow = Instantiate(arrowPrefab, arrowSpawnPoint.position, ActiveWeapon.Instance.transform.rotation);
        // newArrow.GetComponent<Projectile>().UpdateProjectileRange(weaponInfo.weaponRange);
        Debug.Log("Bow Attack");
    }
    public WeaponInfo GetWeaponInfo()
    {
        return weaponInfo;
    }
}
