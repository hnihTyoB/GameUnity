using UnityEngine;
using System.Collections.Generic;

/// <summary>
/// Quản lý việc cập nhật difficulty cho tất cả enemy trong scene khi difficulty thay đổi
/// </summary>
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
    
    /// <summary>
    /// Cập nhật difficulty cho tất cả enemy trong scene
    /// </summary>
    private void UpdateAllEnemies(DifficultyManager.Difficulty newDifficulty)
    {
        float multiplier = DifficultyManager.GetDifficultyMultiplier();
        
        // Tìm tất cả enemy trong scene
        EnemyHealth[] allEnemies = FindObjectsOfType<EnemyHealth>();
        EnemyPathFinding[] allEnemyPaths = FindObjectsOfType<EnemyPathFinding>();
        
        // Cập nhật health (chỉ áp dụng nếu enemy còn full health)
        foreach (EnemyHealth enemy in allEnemies)
        {
            if (enemy != null)
            {
                enemy.UpdateDifficultyMultiplier();
            }
        }
        
        // Cập nhật speed
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

