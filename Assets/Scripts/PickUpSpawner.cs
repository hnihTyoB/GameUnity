using UnityEngine;

public class PickUpSpawner : MonoBehaviour
{
    [SerializeField] private GameObject goldCoin, battery, staminaGlobe;

    public void DropItems() {
        float multiplier = DifficultyManager.GetDifficultyMultiplier();
        
        // Điều chỉnh probability dựa trên difficulty
        // Easy (0.7x): Drop nhiều hơn (giảm chance không drop)
        // Hard (1.5x): Drop ít hơn (tăng chance không drop)
        float noDropChance = 0.25f * multiplier; // Base 25%
        
        // Random từ 0-1
        float randomValue = Random.Range(0f, 1f);
        
        if (randomValue < noDropChance)
        {
            // Không drop gì
            Debug.Log($"PickUpSpawner: No drop (Difficulty: {multiplier:F2}x, NoDropChance: {noDropChance:P0})");
            return;
        }
        
        // Nếu có drop, random chọn loại item
        int itemType = Random.Range(1, 4); // 1, 2, hoặc 3
        
        if (itemType == 1 && battery != null) {
            Instantiate(battery, transform.position, Quaternion.identity);
            Debug.Log($"PickUpSpawner: Dropped battery (Difficulty: {multiplier:F2}x)");
        } 
        else if (itemType == 2 && staminaGlobe != null) {
            Instantiate(staminaGlobe, transform.position, Quaternion.identity);
            Debug.Log($"PickUpSpawner: Dropped stamina (Difficulty: {multiplier:F2}x)");
        }
        else if (itemType == 3 && goldCoin != null) {
            int randomAmountOfGold = Random.Range(1, 4);
            for (int i = 0; i < randomAmountOfGold; i++)
            {
                Instantiate(goldCoin, transform.position, Quaternion.identity);
            }
            Debug.Log($"PickUpSpawner: Dropped {randomAmountOfGold} gold coins (Difficulty: {multiplier:F2}x)");
        }
        
        // If nothing assigned, log warning
        if (battery == null && staminaGlobe == null && goldCoin == null)
        {
            Debug.LogWarning($"PickUpSpawner on {gameObject.name}: No items assigned! Cannot drop anything.");
        }
    }
    
    /// <summary>
    /// Drop only battery (for Shadow enemies)
    /// </summary>
    public void DropBatteryOnly()
    {
        if (battery != null)
        {
            Instantiate(battery, transform.position, Quaternion.identity);
            Debug.Log($"PickUpSpawner: Dropped battery (Shadow kill)");
        }
        else
        {
            Debug.LogWarning($"PickUpSpawner on {gameObject.name}: Battery prefab not assigned!");
        }
    }
}
