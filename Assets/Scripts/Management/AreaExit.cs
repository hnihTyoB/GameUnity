using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class AreaExit : MonoBehaviour
{
    [SerializeField] private string sceneToLoad;
    [SerializeField] private string sceneTransitionName;
    [SerializeField] private string endExitTag = "AreaEnd";
    private float waitToLoadTime = 1f;
    private bool hasTriggered = false; // Prevent multiple triggers

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (hasTriggered) return;
        if (other.gameObject.GetComponent<PlayerController>() == null) return;

        hasTriggered = true;

        // If this door is tagged as the end-game exit, trigger level complete
        if (gameObject.CompareTag(endExitTag))
        {
            if (ScoreManager.Instance != null)
            {
                ScoreManager.Instance.OnLevelComplete();
                Debug.Log("AreaExit: End-game door triggered - OnLevelComplete called");
            }
            else
            {
                Debug.LogError("AreaExit: ScoreManager.Instance is null!");
            }
            return;
        }

        // Normal scene transition (non end-game door)
        if (SceneManagement.Instance != null)
        {
            SceneManagement.Instance.SetTransitionName(sceneTransitionName);
        }
        else
        {
            Debug.LogWarning("AreaExit: SceneManagement.Instance is null.");
        }

        if (UIFade.Instance != null)
        {
            UIFade.Instance.FadeToBlack();
        }
        else
        {
            Debug.LogWarning("AreaExit: UIFade.Instance is null.");
        }

        StartCoroutine(LoadSceneRoutine());
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
