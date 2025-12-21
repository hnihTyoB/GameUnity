using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class ShadowSpawnManager : MonoBehaviour
{
    [Header("Shadow Prefabs")]
    [SerializeField] private GameObject shadowGhost1Prefab;
    [SerializeField] private GameObject shadowGhost2Prefab;
    [SerializeField] private GameObject shadowGhost3Prefab;
    
    [Header("Spawn Settings")]
    [SerializeField] private int maxShadowsInScene = 10;
    [SerializeField] private float spawnInterval = 15f;
    [SerializeField] private float initialSpawnDelay = 5f;
    [SerializeField] private bool autoSpawn = true;
    
    [Header("Shadow Type Weights")]
    [SerializeField] private int shadowGhost1Weight = 60;
    [SerializeField] private int shadowGhost2Weight = 30;
    [SerializeField] private int shadowGhost3Weight = 10;
    
    [Header("Spawn Zones")]
    [SerializeField] private ShadowSpawnZone[] spawnZones;
    
    private List<GameObject> activeShadows = new List<GameObject>();
    private bool isSpawning = false;
    private Coroutine spawnCoroutine;
    
  
    private int baseMaxShadows;
    private float baseSpawnInterval;
    private float baseInitialDelay;
    
    private void Start()
    {

        baseMaxShadows = maxShadowsInScene;
        baseSpawnInterval = spawnInterval;
        baseInitialDelay = initialSpawnDelay;
        
  
        ApplyDifficultySettings();
        
        if (spawnZones == null || spawnZones.Length == 0)
        {
            spawnZones = FindObjectsOfType<ShadowSpawnZone>();
        }
        
        if (autoSpawn)
        {
            StartSpawning();
        }
    }
    
    private void OnEnable()
    {
       
        DifficultyManager.OnDifficultyChanged += OnDifficultyChanged;
    }
    
    private void OnDisable()
    {
    
        DifficultyManager.OnDifficultyChanged -= OnDifficultyChanged;
    }
    
  
    private void ApplyDifficultySettings()
    {
        float multiplier = DifficultyManager.GetDifficultyMultiplier();
        
  
        maxShadowsInScene = Mathf.RoundToInt(baseMaxShadows * multiplier);
        
   
        spawnInterval = baseSpawnInterval / multiplier;
        

        initialSpawnDelay = baseInitialDelay / multiplier;
        
        Debug.Log($"ShadowSpawnManager: Difficulty applied - Max: {maxShadowsInScene}, Interval: {spawnInterval:F1}s, Delay: {initialSpawnDelay:F1}s");
    }
    

    private void OnDifficultyChanged(DifficultyManager.Difficulty newDifficulty)
    {
        
        ApplyDifficultySettings();
        
      
        if (isSpawning)
        {
            StopSpawning();
            StartSpawning();
        }
    }
    
    public void StartSpawning()
    {
        if (!isSpawning)
        {
            isSpawning = true;
            spawnCoroutine = StartCoroutine(AutoSpawnRoutine());
        }
    }
    
    public void StopSpawning()
    {
        if (isSpawning && spawnCoroutine != null)
        {
            StopCoroutine(spawnCoroutine);
            isSpawning = false;
        }
    }
    
    private IEnumerator AutoSpawnRoutine()
    {
        yield return new WaitForSeconds(initialSpawnDelay);
        
        while (isSpawning)
        {
            CleanupDestroyedShadows();
            
            if (activeShadows.Count < maxShadowsInScene)
            {
                SpawnRandomShadow();
            }
            
            yield return new WaitForSeconds(spawnInterval);
        }
    }
    
    public GameObject SpawnRandomShadow()
    {
        if (spawnZones == null || spawnZones.Length == 0) return null;
        if (activeShadows.Count >= maxShadowsInScene) return null;
        

        ShadowSpawnZone randomZone = spawnZones[Random.Range(0, spawnZones.Length)];
        
  
        Vector2 spawnPosition = randomZone.GetRandomValidPosition();
        
        if (spawnPosition == Vector2.zero)
        {
            return null;
        }
        

        GameObject shadowPrefab = GetRandomShadowPrefab();
        
        if (shadowPrefab == null) return null;
        
   
        GameObject spawnedShadow = Instantiate(shadowPrefab, spawnPosition, Quaternion.identity);
        spawnedShadow.name = $"{shadowPrefab.name} (Spawned)";
        
        activeShadows.Add(spawnedShadow);
        return spawnedShadow;
    }
    
    public GameObject SpawnShadowAtPosition(GameObject shadowPrefab, Vector2 position)
    {
        if (shadowPrefab == null) return null;
        if (activeShadows.Count >= maxShadowsInScene) return null;
        
        GameObject spawnedShadow = Instantiate(shadowPrefab, position, Quaternion.identity);
        spawnedShadow.name = $"{shadowPrefab.name} (Manual Spawn)";
        
        activeShadows.Add(spawnedShadow);
        return spawnedShadow;
    }
    
    private GameObject GetRandomShadowPrefab()
    {
        int totalWeight = shadowGhost1Weight + shadowGhost2Weight + shadowGhost3Weight;
        if (totalWeight == 0) return shadowGhost1Prefab;
        
        int randomNumber = Random.Range(0, totalWeight);
        
        if (randomNumber < shadowGhost1Weight)
        {
            return shadowGhost1Prefab;
        }
        else if (randomNumber < shadowGhost1Weight + shadowGhost2Weight)
        {
            return shadowGhost2Prefab;
        }
        else
        {
            return shadowGhost3Prefab;
        }
    }
    
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
    }
    
    private void CleanupDestroyedShadows()
    {
        activeShadows.RemoveAll(shadow => shadow == null);
    }
    
    public int GetActiveShadowCount()
    {
        CleanupDestroyedShadows();
        return activeShadows.Count;
    }
    
    public bool CanSpawnMore()
    {
        CleanupDestroyedShadows();
        return activeShadows.Count < maxShadowsInScene;
    }
    
    private void OnDestroy()
    {
        StopSpawning();
    }
}
