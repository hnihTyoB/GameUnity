using UnityEngine;

public class PickUpSpawner : MonoBehaviour
{
    [SerializeField] private GameObject goldCoin, battery, staminaGlobe;

    public void DropItems() {
        int randomNum = Random.Range(1, 5);

        if (randomNum == 1 && battery != null) {
            Instantiate(battery, transform.position, Quaternion.identity);
            Debug.Log($"PickUpSpawner: Dropped battery");
        } 

        if (randomNum == 2 && staminaGlobe != null) {
            Instantiate(staminaGlobe, transform.position, Quaternion.identity);
            Debug.Log($"PickUpSpawner: Dropped stamina");
        }

        if (randomNum == 3 && goldCoin != null) {
            int randomAmountOfGold = Random.Range(1, 4);
            
            for (int i = 0; i < randomAmountOfGold; i++)
            {
                Instantiate(goldCoin, transform.position, Quaternion.identity);
            }
            Debug.Log($"PickUpSpawner: Dropped {randomAmountOfGold} gold coins");
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
