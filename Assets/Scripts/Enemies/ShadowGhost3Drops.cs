using UnityEngine;

/// <summary>
/// Custom item drops cho Shadow Ghost 3 - "Bóng đen cô lập"
/// </summary>
public class ShadowGhost3Drops : MonoBehaviour
{
    [Header("Drop Prefabs")]
    [SerializeField] private GameObject lightFragmentPrefab; // Tinh thể ánh sáng
    [SerializeField] private GameObject batteryPrefab; // Pin đèn
    [SerializeField] private GameObject goldCoinPrefab;
    
    [Header("Drop Rates")]
    [SerializeField] private int lightFragmentDropRate = 25; // 25% - giống Shadow Ghost 1 & 2
    [SerializeField] private int batteryDropRate = 5; // 5% - giống Shadow Ghost 1 & 2
    [SerializeField] private int goldCoinDropRate = 50; // 50% - giống Shadow Ghost 1 & 2
    
    private void Start()
    {
        // ShadowGhost3Drops sẽ được gọi từ PickUpSpawner
        // Không cần subscribe event
    }
    
    public void SpawnDropsOnDeath()
    {
        // Method này sẽ được gọi từ PickUpSpawner
        SpawnDrops();
    }
    
    private void SpawnDrops()
    {
        // Shadow Ghost 3 now ONLY drops battery (100% chance)
        if (batteryPrefab != null)
        {
            SpawnItem(batteryPrefab);
            Debug.Log($"Shadow Ghost 3 dropped battery");
        }
        else
        {
            Debug.LogWarning("ShadowGhost3Drops: Battery prefab is not assigned!");
        }
    }
    
    private void SpawnItem(GameObject itemPrefab)
    {
        if (itemPrefab != null)
        {
            Vector3 spawnPosition = transform.position + new Vector3(
                Random.Range(-0.5f, 0.5f),
                Random.Range(-0.5f, 0.5f),
                0
            );
            
            Instantiate(itemPrefab, spawnPosition, Quaternion.identity);
        }
    }
}

