using UnityEngine;

public class ShadowGhostDrops : MonoBehaviour
{
    [Header("Drop Items")]
    [SerializeField] private GameObject lightFragmentPrefab;
    [SerializeField] private GameObject batteryPrefab;
    [SerializeField] private GameObject goldCoinPrefab;
    
    [Header("Drop Rates (%)")]
    [SerializeField] private float lightFragmentDropRate = 25f;
    [SerializeField] private float batteryDropRate = 5f;
    [SerializeField] private float goldCoinDropRate = 50f;
    
    public void DropItems()
    {
        Vector3 dropPosition = transform.position;
        
        // Shadow now ONLY drops battery (100% chance)
        if (batteryPrefab != null)
        {
            Instantiate(batteryPrefab, dropPosition, Quaternion.identity);
            Debug.Log($"Shadow dropped battery at {dropPosition}");
        }
        else
        {
            Debug.LogWarning("ShadowGhostDrops: Battery prefab is not assigned!");
        }
    }
}

