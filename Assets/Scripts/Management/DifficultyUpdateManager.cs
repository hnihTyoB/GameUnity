using UnityEngine;
using System.Collections.Generic;


public class DifficultyUpdateManager : MonoBehaviour
{
    private void OnEnable()
    {
        DifficultyManager.OnDifficultyChanged += UpdateAllEnemies;
    }
    
    private void OnDisable()
    {
        DifficultyManager.OnDifficultyChanged -= UpdateAllEnemies;
    }
    
  
    private void UpdateAllEnemies(DifficultyManager.Difficulty newDifficulty)
    {
        float multiplier = DifficultyManager.GetDifficultyMultiplier();
        
     
        EnemyHealth[] allEnemies = FindObjectsOfType<EnemyHealth>();
        EnemyPathFinding[] allEnemyPaths = FindObjectsOfType<EnemyPathFinding>();
        
      
        foreach (EnemyHealth enemy in allEnemies)
        {
            if (enemy != null)
            {
                enemy.UpdateDifficultyMultiplier();
            }
        }
        
        
        foreach (EnemyPathFinding enemyPath in allEnemyPaths)
        {
            if (enemyPath != null)
            {
                enemyPath.ApplyDifficultyMultiplier(multiplier);
            }
        }
        
        Debug.Log($"DifficultyUpdateManager: Updated {allEnemies.Length} enemies to {newDifficulty} (multiplier: {multiplier:F2}x)");
    }
}

