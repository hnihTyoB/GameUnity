using UnityEngine;


public class ShadowGhost2Drops : MonoBehaviour
{
    [Header("Drop Prefabs")]
    [SerializeField] private GameObject lightFragmentPrefab; // Tinh thể ánh sáng
    [SerializeField] private GameObject batteryPrefab; // Pin đèn
    [SerializeField] private GameObject goldCoinPrefab;
    
    [Header("Drop Rates")]
    [SerializeField] private int lightFragmentDropRate = 25; // 25% - giống Shadow Ghost 1
    [SerializeField] private int batteryDropRate = 5; // 5% - giống Shadow Ghost 1
    [SerializeField] private int goldCoinDropRate = 50; // 50% - giống Shadow Ghost 1
    
    private void Start()
    {

    }
    
    public void SpawnDropsOnDeath()
    {
  
        SpawnDrops();
    }
    
    private void SpawnDrops()
    {
      
        if (batteryPrefab != null)
        {
            SpawnItem(batteryPrefab);
           
        }
        else
        {
            Debug.LogWarning("ShadowGhost2Drops: Battery prefab is not assigned!");
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
