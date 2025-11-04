using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Quản lý việc spawn Shadow Ghosts ngẫu nhiên trong game
/// Spawn tại các ShadowSpawnZone được định nghĩa sẵn
/// </summary>
public class ShadowSpawnManager : MonoBehaviour
{
    [Header("Shadow Prefabs")]
    [SerializeField] private GameObject shadowGhost1Prefab;
    [SerializeField] private GameObject shadowGhost2Prefab;
    [SerializeField] private GameObject shadowGhost3Prefab;
    
    [Header("Spawn Settings")]
    [SerializeField] private int maxShadowsInScene = 10; // Số shadow tối đa trong scene
    [SerializeField] private float spawnInterval = 15f; // Thời gian giữa các lần spawn (seconds)
    [SerializeField] private float initialSpawnDelay = 5f; // Delay trước khi spawn lần đầu
    [SerializeField] private bool autoSpawn = true; // Tự động spawn hay spawn theo lệnh
    
    [Header("Shadow Type Weights")]
    [Tooltip("Xác suất spawn Shadow Ghost 1 (0-100)")]
    [SerializeField] private int shadowGhost1Weight = 60;
    [Tooltip("Xác suất spawn Shadow Ghost 2 (0-100)")]
    [SerializeField] private int shadowGhost2Weight = 30;
    [Tooltip("Xác suất spawn Shadow Ghost 3 (0-100)")]
    [SerializeField] private int shadowGhost3Weight = 10;
    
    [Header("Spawn Zones")]
    [SerializeField] private ShadowSpawnZone[] spawnZones; // Các vùng spawn
    
    [Header("Debug")]
    [SerializeField] private bool showDebugLogs = true;
    
    private List<GameObject> activeShadows = new List<GameObject>(); // Track shadows đang active
    private bool isSpawning = false;
    private Coroutine spawnCoroutine;
    
    private void Start()
    {
        // Tự động tìm tất cả spawn zones nếu chưa assign
        if (spawnZones == null || spawnZones.Length == 0)
        {
            spawnZones = FindObjectsOfType<ShadowSpawnZone>();
            
            if (spawnZones.Length == 0)
            {
                Debug.LogError("ShadowSpawnManager: Không tìm thấy ShadowSpawnZone nào! Hãy tạo GameObject với ShadowSpawnZone component.");
                return;
            }
        }
        
        // Bắt đầu auto spawn nếu được bật
        if (autoSpawn)
        {
            StartSpawning();
        }
    }
    
    /// <summary>
    /// Bắt đầu spawn shadows tự động
    /// </summary>
    public void StartSpawning()
    {
        if (!isSpawning)
        {
            isSpawning = true;
            spawnCoroutine = StartCoroutine(AutoSpawnRoutine());
            
            if (showDebugLogs)
            {
                Debug.Log($"ShadowSpawnManager: Bắt đầu auto spawn. Interval: {spawnInterval}s, Max shadows: {maxShadowsInScene}");
            }
        }
    }
    
    /// <summary>
    /// Dừng spawn shadows tự động
    /// </summary>
    public void StopSpawning()
    {
        if (isSpawning && spawnCoroutine != null)
        {
            StopCoroutine(spawnCoroutine);
            isSpawning = false;
            
            if (showDebugLogs)
            {
                Debug.Log("ShadowSpawnManager: Dừng auto spawn.");
            }
        }
    }
    
    /// <summary>
    /// Coroutine spawn shadows tự động
    /// </summary>
    private IEnumerator AutoSpawnRoutine()
    {
        // Delay trước khi spawn lần đầu
        yield return new WaitForSeconds(initialSpawnDelay);
        
        while (isSpawning)
        {
            // Xóa các shadow đã bị destroy khỏi list
            CleanupDestroyedShadows();
            
            // Spawn shadow nếu chưa đạt max
            if (activeShadows.Count < maxShadowsInScene)
            {
                SpawnRandomShadow();
            }
            else if (showDebugLogs)
            {
                Debug.Log($"ShadowSpawnManager: Đã đạt max shadows ({maxShadowsInScene}). Chờ shadows bị tiêu diệt...");
            }
            
            // Chờ đến lần spawn tiếp theo
            yield return new WaitForSeconds(spawnInterval);
        }
    }
    
    /// <summary>
    /// Spawn một shadow ngẫu nhiên tại một spawn zone ngẫu nhiên
    /// </summary>
    public GameObject SpawnRandomShadow()
    {
        if (spawnZones == null || spawnZones.Length == 0)
        {
            Debug.LogError("ShadowSpawnManager: Không có spawn zone nào!");
            return null;
        }
        
        // Kiểm tra đã đạt max shadows chưa
        CleanupDestroyedShadows();
        if (activeShadows.Count >= maxShadowsInScene)
        {
            if (showDebugLogs)
            {
                Debug.LogWarning($"ShadowSpawnManager: Không thể spawn - đã đạt max shadows ({maxShadowsInScene})");
            }
            return null;
        }
        
        // Chọn spawn zone ngẫu nhiên
        ShadowSpawnZone randomZone = spawnZones[Random.Range(0, spawnZones.Length)];
        
        // Lấy vị trí spawn hợp lệ
        Vector2 spawnPosition = randomZone.GetRandomValidPosition();
        
        // KIỂM TRA: Nếu position == Vector2.zero, zone không tìm được vị trí hợp lệ
        if (spawnPosition == Vector2.zero || spawnPosition.magnitude < 0.1f)
        {
            Debug.LogWarning($"ShadowSpawnManager: Zone '{randomZone.name}' không tìm được vị trí hợp lệ. SKIP spawn lần này.");
            return null; // Không spawn, return null
        }
        
        // Chọn loại shadow dựa trên weights
        GameObject shadowPrefab = GetRandomShadowPrefab();
        
        if (shadowPrefab == null)
        {
            Debug.LogError("ShadowSpawnManager: Không có shadow prefab nào được assign!");
            return null;
        }
        
        // DOUBLE CHECK: Kiểm tra lại vị trí trước khi spawn (safety)
        // Dùng chính Obstacle Layer và Spawn Check Radius từ zone
        LayerMask obstacleLayer = randomZone.GetObstacleLayer();
        float checkRadius = randomZone.GetSpawnCheckRadius();
        
        // Double check collision trước khi spawn
        Collider2D blockingCollider = Physics2D.OverlapCircle(spawnPosition, checkRadius, obstacleLayer);
        
        if (blockingCollider != null)
        {
            if (showDebugLogs)
            {
                Debug.LogWarning($"ShadowSpawnManager: Vị trí không an toàn - có obstacle. Skip spawn.");
            }
            return null;
        }
        
        // Spawn shadow
        GameObject spawnedShadow = Instantiate(shadowPrefab, spawnPosition, Quaternion.identity);
        spawnedShadow.name = $"{shadowPrefab.name} (Spawned)";
        
        // Thêm vào list tracking
        activeShadows.Add(spawnedShadow);
        
        if (showDebugLogs)
        {
            Debug.Log($"✓ ShadowSpawnManager: Spawned {spawnedShadow.name} tại {spawnPosition} (Zone: {randomZone.name}). Active shadows: {activeShadows.Count}/{maxShadowsInScene}");
        }
        
        return spawnedShadow;
    }
    
    /// <summary>
    /// Spawn một loại shadow cụ thể tại vị trí cụ thể
    /// </summary>
    public GameObject SpawnShadowAtPosition(GameObject shadowPrefab, Vector2 position)
    {
        if (shadowPrefab == null)
        {
            Debug.LogError("ShadowSpawnManager: Shadow prefab is null!");
            return null;
        }
        
        // Kiểm tra max shadows
        CleanupDestroyedShadows();
        if (activeShadows.Count >= maxShadowsInScene)
        {
            if (showDebugLogs)
            {
                Debug.LogWarning($"ShadowSpawnManager: Không thể spawn - đã đạt max shadows ({maxShadowsInScene})");
            }
            return null;
        }
        
        GameObject spawnedShadow = Instantiate(shadowPrefab, position, Quaternion.identity);
        spawnedShadow.name = $"{shadowPrefab.name} (Manual Spawn)";
        activeShadows.Add(spawnedShadow);
        
        if (showDebugLogs)
        {
            Debug.Log($"ShadowSpawnManager: Manually spawned {spawnedShadow.name} at {position}");
        }
        
        return spawnedShadow;
    }
    
    /// <summary>
    /// Chọn shadow prefab ngẫu nhiên dựa trên weights
    /// </summary>
    private GameObject GetRandomShadowPrefab()
    {
        int totalWeight = shadowGhost1Weight + shadowGhost2Weight + shadowGhost3Weight;
        
        if (totalWeight == 0)
        {
            Debug.LogError("ShadowSpawnManager: Tổng weights = 0! Hãy set weights cho shadow types.");
            return shadowGhost1Prefab; // Fallback
        }
        
        int randomValue = Random.Range(0, totalWeight);
        
        // Shadow Ghost 1
        if (randomValue < shadowGhost1Weight)
        {
            return shadowGhost1Prefab;
        }
        
        // Shadow Ghost 2
        randomValue -= shadowGhost1Weight;
        if (randomValue < shadowGhost2Weight)
        {
            return shadowGhost2Prefab;
        }
        
        // Shadow Ghost 3
        return shadowGhost3Prefab;
    }
    
    /// <summary>
    /// Xóa các shadow đã bị destroy khỏi tracking list
    /// </summary>
    private void CleanupDestroyedShadows()
    {
        activeShadows.RemoveAll(shadow => shadow == null);
    }
    
    /// <summary>
    /// Xóa tất cả shadows đang active
    /// </summary>
    public void DestroyAllShadows()
    {
        foreach (GameObject shadow in activeShadows)
        {
            if (shadow != null)
            {
                Destroy(shadow);
            }
        }
        
        activeShadows.Clear();
        
        if (showDebugLogs)
        {
            Debug.Log("ShadowSpawnManager: Đã xóa tất cả shadows.");
        }
    }
    
    /// <summary>
    /// Lấy số lượng shadows đang active
    /// </summary>
    public int GetActiveShadowCount()
    {
        CleanupDestroyedShadows();
        return activeShadows.Count;
    }
    
    /// <summary>
    /// Kiểm tra có thể spawn thêm shadow không
    /// </summary>
    public bool CanSpawnMore()
    {
        CleanupDestroyedShadows();
        return activeShadows.Count < maxShadowsInScene;
    }
    
    private void OnDestroy()
    {
        // Cleanup khi manager bị destroy
        StopSpawning();
    }
    
    /// <summary>
    /// Helper method để convert LayerMask thành tên layers (for debug)
    /// </summary>
    private string GetLayerMaskNames(LayerMask layerMask)
    {
        List<string> layerNames = new List<string>();
        
        for (int i = 0; i < 32; i++)
        {
            if ((layerMask.value & (1 << i)) != 0)
            {
                string layerName = LayerMask.LayerToName(i);
                if (!string.IsNullOrEmpty(layerName))
                {
                    layerNames.Add(layerName);
                }
            }
        }
        
        return layerNames.Count > 0 ? string.Join(", ", layerNames) : "NONE";
    }
}

