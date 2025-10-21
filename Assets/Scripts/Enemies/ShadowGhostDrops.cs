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
        
        // Check for Light Fragment drop (25%)
        float lightFragmentRoll = Random.Range(0f, 100f);
        if (lightFragmentRoll <= lightFragmentDropRate && lightFragmentPrefab != null)
        {
            Instantiate(lightFragmentPrefab, dropPosition, Quaternion.identity);
            return; // Drop only one type of special item
        }
        
        // Check for Battery drop (5%)
        float batteryRoll = Random.Range(0f, 100f);
        if (batteryRoll <= batteryDropRate && batteryPrefab != null)
        {
            Instantiate(batteryPrefab, dropPosition, Quaternion.identity);
            return; // Drop only one type of special item
        }
        
        // Otherwise, chance to drop gold coins (50%)
        float goldRoll = Random.Range(0f, 100f);
        if (goldRoll <= goldCoinDropRate && goldCoinPrefab != null)
        {
            int randomAmountOfGold = Random.Range(1, 3);
            
            for (int i = 0; i < randomAmountOfGold; i++)
            {
                Vector3 offset = new Vector3(Random.Range(-0.3f, 0.3f), Random.Range(-0.3f, 0.3f), 0);
                Instantiate(goldCoinPrefab, dropPosition + offset, Quaternion.identity);
            }
        }
    }
}

