using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class AreaExit : MonoBehaviour
{
    [SerializeField] private string sceneToLoad;
    [SerializeField] private string sceneTransitionName;
    private float waitToLoadTime = 1f;
    private bool hasTriggered = false; // Prevent multiple triggers

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.gameObject.GetComponent<PlayerController>() && !hasTriggered)
        {
            hasTriggered = true;
            
            // Check current scene name
            string currentSceneName = UnityEngine.SceneManagement.SceneManager.GetActiveScene().name;
            
            // Only show EndLevelUI in Scene2 (or last scene)
            if (currentSceneName == "Scene2")
            {
                // Store scene to load for EndLevelUI
                if (!string.IsNullOrEmpty(sceneToLoad))
                {
                    EndLevelUI.SetNextScene(sceneToLoad);
                }
                
                // Find EndLevelUI in scene if it exists
                EndLevelUI endLevelUI = FindObjectOfType<EndLevelUI>();
                if (endLevelUI == null)
                {
                    Debug.LogError("AreaExit: EndLevelUI not found in scene! Please add EndLevelUI component to a GameObject in Scene2.");
                }
                else
                {
                    Debug.Log("AreaExit: Found EndLevelUI in scene");
                }
                
                // Trigger level complete (EndLevelUI will handle displaying results and scene transition)
                if (ScoreManager.Instance != null)
                {
                    ScoreManager.Instance.OnLevelComplete();
                }
                else
                {
                    Debug.LogError("AreaExit: ScoreManager.Instance is null!");
                }
                
                // Note: EndLevelUI will pause the game and show results
                // Player can then choose to continue (load next scene) or go to menu
            }
            else
            {
                // Scene1: Just load next scene without showing EndLevelUI
            SceneManagement.Instance.SetTransitionName(sceneTransitionName);
            UIFade.Instance.FadeToBlack();
            StartCoroutine(LoadSceneRoutine());
        }
    }
    }
    
    private IEnumerator LoadSceneRoutine()
    {
        while (waitToLoadTime >= 0) 
        {
            waitToLoadTime -= Time.deltaTime;
            yield return null;
        }

        UnityEngine.SceneManagement.SceneManager.LoadScene(sceneToLoad);
    }
    
}
