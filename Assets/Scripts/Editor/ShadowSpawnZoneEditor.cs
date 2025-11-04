using UnityEngine;
using UnityEditor;

/// <summary>
/// Custom Editor cho ShadowSpawnZone
/// Thêm các buttons tiện ích và visualizations
/// </summary>
[CustomEditor(typeof(ShadowSpawnZone))]
public class ShadowSpawnZoneEditor : Editor
{
    private const int PREVIEW_POINTS = 10;
    
    public override void OnInspectorGUI()
    {
        // Vẽ Inspector mặc định
        DrawDefaultInspector();
        
        ShadowSpawnZone zone = (ShadowSpawnZone)target;
        
        // Separator
        EditorGUILayout.Space(10);
        EditorGUILayout.LabelField("Zone Tools", EditorStyles.boldLabel);
        
        // Test Valid Position button
        if (GUILayout.Button("Test Get Valid Position", GUILayout.Height(30)))
        {
            Vector2 validPos = zone.GetRandomValidPosition();
            Debug.Log($"✓ Valid position: {validPos}");
            
            // Highlight vị trí trong Scene view
            if (SceneView.lastActiveSceneView != null)
            {
                SceneView.lastActiveSceneView.ShowNotification(
                    new GUIContent($"Valid Position: {validPos}"), 
                    2f);
            }
        }
        
        // Preview Multiple Positions
        if (GUILayout.Button($"Preview {PREVIEW_POINTS} Random Positions", GUILayout.Height(25)))
        {
            Debug.Log($"=== Testing {PREVIEW_POINTS} random positions in {zone.name} ===");
            int validCount = 0;
            
            for (int i = 0; i < PREVIEW_POINTS; i++)
            {
                Vector2 pos = zone.GetRandomValidPosition();
                if (pos != Vector2.zero)
                {
                    validCount++;
                    Debug.Log($"  {i+1}. Valid position: {pos}");
                }
            }
            
            Debug.Log($"✓ Found {validCount}/{PREVIEW_POINTS} valid positions");
        }
        
        // Info Box
        EditorGUILayout.Space(10);
        EditorGUILayout.LabelField("Zone Info", EditorStyles.boldLabel);
        EditorGUILayout.HelpBox(
            $"Zone Size: {zone.GetZoneSize()}\n" +
            $"Min Distance: {zone.GetMinDistanceFromPlayer()}\n" +
            $"Center: {zone.transform.position}", 
            MessageType.Info);
        
        // Setup reminder
        EditorGUILayout.Space(5);
        EditorGUILayout.HelpBox(
            "⚠ Nhớ set Obstacle Layer mask để tránh spawn vào tường!", 
            MessageType.Warning);
    }
    
    // Vẽ handles trong Scene view
    private void OnSceneGUI()
    {
        ShadowSpawnZone zone = (ShadowSpawnZone)target;
        
        // Vẽ label cho zone
        Handles.Label(
            zone.transform.position + Vector3.up * 2f,
            $"Shadow Spawn Zone\n{zone.name}",
            EditorStyles.whiteLargeLabel);
    }
}

