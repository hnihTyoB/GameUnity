using UnityEngine;
using UnityEditor;

/// <summary>
/// Custom Editor cho ShadowSpawnManager
/// Thêm các buttons tiện ích trong Inspector
/// </summary>
[CustomEditor(typeof(ShadowSpawnManager))]
public class ShadowSpawnManagerEditor : Editor
{
    public override void OnInspectorGUI()
    {
        // Vẽ Inspector mặc định
        DrawDefaultInspector();
        
        ShadowSpawnManager manager = (ShadowSpawnManager)target;
        
        // Separator
        EditorGUILayout.Space(10);
        EditorGUILayout.LabelField("Runtime Controls", EditorStyles.boldLabel);
        
        // Chỉ hiển thị buttons khi đang chạy
        if (Application.isPlaying)
        {
            EditorGUILayout.BeginHorizontal();
            
            // Spawn button
            if (GUILayout.Button("Spawn Shadow Now", GUILayout.Height(30)))
            {
                GameObject spawned = manager.SpawnRandomShadow();
                if (spawned != null)
                {
                    Debug.Log($"✓ Spawned: {spawned.name}");
                }
            }
            
            EditorGUILayout.EndHorizontal();
            
            EditorGUILayout.BeginHorizontal();
            
            // Start/Stop buttons
            if (GUILayout.Button("Start Auto Spawn", GUILayout.Height(25)))
            {
                manager.StartSpawning();
            }
            
            if (GUILayout.Button("Stop Auto Spawn", GUILayout.Height(25)))
            {
                manager.StopSpawning();
            }
            
            EditorGUILayout.EndHorizontal();
            
            // Destroy all button (màu đỏ)
            GUI.backgroundColor = Color.red;
            if (GUILayout.Button("Destroy All Shadows", GUILayout.Height(30)))
            {
                if (EditorUtility.DisplayDialog(
                    "Xác nhận", 
                    "Bạn có chắc muốn xóa TẤT CẢ shadows?", 
                    "Có", 
                    "Không"))
                {
                    manager.DestroyAllShadows();
                    Debug.Log("✓ Đã xóa tất cả shadows");
                }
            }
            GUI.backgroundColor = Color.white;
            
            // Hiển thị thông tin
            EditorGUILayout.Space(10);
            EditorGUILayout.LabelField("Status", EditorStyles.boldLabel);
            EditorGUILayout.LabelField($"Active Shadows: {manager.GetActiveShadowCount()}");
            EditorGUILayout.LabelField($"Can Spawn More: {(manager.CanSpawnMore() ? "Yes" : "No")}");
        }
        else
        {
            EditorGUILayout.HelpBox("Runtime controls chỉ khả dụng khi đang Play mode.", MessageType.Info);
        }
        
        // Setup Guide
        EditorGUILayout.Space(10);
        EditorGUILayout.LabelField("Quick Setup Guide", EditorStyles.boldLabel);
        EditorGUILayout.HelpBox(
            "1. Assign Shadow Prefabs (Shadow Ghost 1, 2, 3)\n" +
            "2. Set Max Shadows và Spawn Interval\n" +
            "3. Adjust Shadow Type Weights\n" +
            "4. Tạo ShadowSpawnZone objects trong scene\n" +
            "5. Assign Spawn Zones hoặc để trống (auto find)\n" +
            "6. Check Auto Spawn để tự động spawn", 
            MessageType.None);
    }
}

