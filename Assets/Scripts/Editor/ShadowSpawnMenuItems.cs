using UnityEngine;
using UnityEditor;

/// <summary>
/// Menu items để nhanh chóng tạo Shadow Spawn objects
/// </summary>
public class ShadowSpawnMenuItems
{
    [MenuItem("GameObject/Shadow Spawn/Create Spawn Zone", false, 10)]
    private static void CreateSpawnZone()
    {
        // Tạo GameObject với ShadowSpawnZone
        GameObject spawnZone = new GameObject("Shadow Spawn Zone");
        spawnZone.AddComponent<ShadowSpawnZone>();
        
        // Set position tại vị trí đang chọn hoặc center
        if (Selection.activeTransform != null)
        {
            spawnZone.transform.position = Selection.activeTransform.position;
        }
        else
        {
            spawnZone.transform.position = Vector3.zero;
        }
        
        // Select object vừa tạo
        Selection.activeGameObject = spawnZone;
        
        // Focus vào object trong Scene view
        if (SceneView.lastActiveSceneView != null)
        {
            SceneView.lastActiveSceneView.FrameSelected();
        }
        
        Debug.Log($"✓ Created Shadow Spawn Zone at {spawnZone.transform.position}");
        EditorUtility.DisplayDialog(
            "Setup Reminder",
            "Nhớ set Obstacle Layer mask trong Inspector!\n\n" +
            "Layers nên include: Ground, Wall, Obstacle, etc.",
            "OK");
    }
    
    [MenuItem("GameObject/Shadow Spawn/Create Spawn Manager", false, 11)]
    private static void CreateSpawnManager()
    {
        // Kiểm tra xem đã có manager chưa
        ShadowSpawnManager existingManager = Object.FindObjectOfType<ShadowSpawnManager>();
        if (existingManager != null)
        {
            bool replace = EditorUtility.DisplayDialog(
                "Spawn Manager Exists",
                $"Đã có ShadowSpawnManager trong scene: {existingManager.name}\n\n" +
                "Bạn có muốn select object đó không?",
                "Select Existing",
                "Create New Anyway");
            
            if (replace)
            {
                Selection.activeGameObject = existingManager.gameObject;
                if (SceneView.lastActiveSceneView != null)
                {
                    SceneView.lastActiveSceneView.FrameSelected();
                }
                return;
            }
        }
        
        // Tạo GameObject với ShadowSpawnManager
        GameObject manager = new GameObject("Shadow Spawn Manager");
        manager.AddComponent<ShadowSpawnManager>();
        
        // Select object vừa tạo
        Selection.activeGameObject = manager;
        
        Debug.Log("✓ Created Shadow Spawn Manager");
        EditorUtility.DisplayDialog(
            "Setup Reminder",
            "Nhớ assign Shadow Prefabs trong Inspector!\n\n" +
            "Prefabs cần assign:\n" +
            "- Shadow Ghost 1\n" +
            "- Shadow Ghost 2\n" +
            "- Shadow Ghost 3\n\n" +
            "Sau đó tạo hoặc assign Spawn Zones.",
            "OK");
    }
    
    [MenuItem("GameObject/Shadow Spawn/Setup Complete System", false, 12)]
    private static void SetupCompleteSystem()
    {
        bool confirm = EditorUtility.DisplayDialog(
            "Setup Complete Shadow Spawn System",
            "Tạo:\n" +
            "✓ 1 Shadow Spawn Manager\n" +
            "✓ 3 Shadow Spawn Zones (sample)\n\n" +
            "Bạn vẫn cần assign prefabs và configure settings sau!",
            "Create",
            "Cancel");
        
        if (!confirm) return;
        
        // 1. Tạo Manager
        GameObject manager = new GameObject("Shadow Spawn Manager");
        manager.AddComponent<ShadowSpawnManager>();
        
        // 2. Tạo 3 sample spawn zones
        GameObject zone1 = new GameObject("Shadow Spawn Zone 1");
        zone1.AddComponent<ShadowSpawnZone>();
        zone1.transform.position = new Vector3(0, 0, 0);
        
        GameObject zone2 = new GameObject("Shadow Spawn Zone 2");
        zone2.AddComponent<ShadowSpawnZone>();
        zone2.transform.position = new Vector3(15, 0, 0);
        
        GameObject zone3 = new GameObject("Shadow Spawn Zone 3");
        zone3.AddComponent<ShadowSpawnZone>();
        zone3.transform.position = new Vector3(-15, 0, 0);
        
        // 3. Tạo parent folder
        GameObject shadowSpawnFolder = new GameObject("=== SHADOW SPAWN SYSTEM ===");
        manager.transform.SetParent(shadowSpawnFolder.transform);
        zone1.transform.SetParent(shadowSpawnFolder.transform);
        zone2.transform.SetParent(shadowSpawnFolder.transform);
        zone3.transform.SetParent(shadowSpawnFolder.transform);
        
        // Select folder
        Selection.activeGameObject = shadowSpawnFolder;
        
        Debug.Log("✓ Created complete Shadow Spawn System with 1 Manager and 3 Zones");
        EditorUtility.DisplayDialog(
            "System Created!",
            "✓ Shadow Spawn System đã được tạo!\n\n" +
            "Next steps:\n" +
            "1. Di chuyển Spawn Zones đến vị trí mong muốn\n" +
            "2. Assign Shadow Prefabs trong Manager\n" +
            "3. Set Obstacle Layer trong từng Zone\n" +
            "4. Adjust settings (max shadows, intervals, etc.)\n\n" +
            "Xem file SHADOW_SPAWN_SETUP_GUIDE.md để biết chi tiết!",
            "OK");
    }
}

